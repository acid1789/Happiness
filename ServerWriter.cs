using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HappinessNetwork;
using NetworkCore;

namespace Happiness
{
    public class ServerWriter
    {
        Thread _thread;
        Mutex _jobLock;
        List<SW_Job> _pendingJobs;
        bool _wantExit;

        public delegate void JobCompleteDelegate(string info);

        public ServerWriter()
        {
            _pendingJobs = new List<SW_Job>();
            _jobLock = new Mutex();

            _thread = new Thread(new ThreadStart(ServerWriterThreadProc));
            _thread.Name = "Server Writer";
            _wantExit = false;
            _thread.Start();
        }

        public void Shutdown()
        {
            if (_thread != null && _thread.IsAlive)
            {
                _wantExit = true;
                _thread.Join();
            }
            _thread = null;
        }

        void ServerWriterThreadProc()
        {
            List<SW_Job> runningJobs = new List<SW_Job>();
            while (!_wantExit)
            {
                if (_pendingJobs.Count > 0)
                {
                    _jobLock.WaitOne();
                    SW_Job[] jobs = _pendingJobs.ToArray();
                    _pendingJobs.Clear();
                    _jobLock.ReleaseMutex();

                    foreach (SW_Job job in jobs)
                    {
                        job.Start();
                        runningJobs.Add(job);
                    }
                }

                List<SW_Job> finishedJobs = new List<SW_Job>();
                foreach (SW_Job job in runningJobs)
                {
                    if( !job.Update() )
                        finishedJobs.Add(job);
                }

                foreach (SW_Job job in finishedJobs)
                {
                    runningJobs.Remove(job);                    
                }

                Thread.Sleep(50);
            }
        }

        void AddJob(SW_Job job)
        {
            _jobLock.WaitOne();
            _pendingJobs.Add(job);
            _jobLock.ReleaseMutex();
        }

        #region Public interface
        public void SaveTutorialData(ulong tutorialData, string authToken, DateTime timeStamp)
        {
            AddJob(new SW_SaveTutorialDataJob(tutorialData, authToken, timeStamp));
        }

        public void SavePuzzleData(GameInfo gi, int tower, int floor, double completionTime, bool noExpBonus, JobCompleteDelegate jobCompleteCB)
        {
            AddJob(new SW_SavePuzzleJob(gi, tower, floor, (float)completionTime, noExpBonus, jobCompleteCB));
        }

        public void SpendCoins(string authToken, int coinCount, int spentOn, GameInfo gi)
        {
            AddJob(new SW_SpendCoinsJob(authToken, coinCount, spentOn, gi));
        }

        public SW_RequestProductsJob RequestProducts()
        {
            SW_RequestProductsJob j = new SW_RequestProductsJob();
            AddJob(j);
            return j;
        }

        public SW_VerifyPurchaseJob WaitForPurchaseComplete()
        {
            SW_VerifyPurchaseJob j = new SW_VerifyPurchaseJob();
            AddJob(j);
            return j;
        }
        #endregion
    }

    public abstract class SW_Job
    {
        protected HClient _client;
        string _jobName;

        protected SW_Job(string jobName)
        {
            _jobName = jobName;
        }

        public virtual void Start()
        {
            // Connect to the server
            _client = new HClient(_jobName);
            DoConnect();
        }

        public abstract bool Update();
        
        protected void DoConnect()
        {
            _client.Connect(HClient.ServerAddress, HClient.ServerPort);
        }
    }

    class SW_SaveTutorialDataJob : SW_Job
    {
        static DateTime _lastSentTime;

        ulong _tutorialData;
        string _authToken;
        DateTime _timeStamp;
        public SW_SaveTutorialDataJob(ulong tutorialData, string authToken, DateTime timeStamp) : base("SW_SaveTutorialDataJob_" + tutorialData)
        {
            _tutorialData = tutorialData;
            _authToken = authToken;
            _timeStamp = timeStamp;
        }

        public override void Start()
        {
            if( (_timeStamp - _lastSentTime).TotalSeconds > 0 )
                base.Start();
        }

        public override bool Update()
        {
            if (_client != null && _client.Connected)
            {
                if ((_timeStamp - _lastSentTime).TotalSeconds > 0)
                {
                    _client.SendTutorialData(_tutorialData, _authToken);
                    _lastSentTime = _timeStamp;                    
                }
                _client.Close();
            }

            // Return false here to be done with this job            
            return false;
        }
    }

    class SW_SavePuzzleJob : SW_Job
    {
        GameInfo _gi;
        int _tower;
        int _floor;
        float _completionTime;
        bool _noExpBonus;

        bool _waitingForResponse;
        ServerWriter.JobCompleteDelegate _jobsDone;

        public SW_SavePuzzleJob(GameInfo gi, int tower, int floor, float completionTime, bool noExpBonus, ServerWriter.JobCompleteDelegate jobFinishedCB) : base("SW_SavePuzzleJob")
        {
            _gi = gi;
            _tower = tower;
            _floor = floor;
            _completionTime = completionTime;
            _jobsDone = jobFinishedCB;
            _noExpBonus = noExpBonus;
        }

        public override void Start()
        {
            base.Start();
            _client.OnGameDataResponse += _client_OnGameDataResponse;
        }

        private void _client_OnGameDataResponse(object sender, GameDataArgs e)
        {
            _gi.GameData = e;
            GameInfoValidator.Instance.Save(_gi);            
            _waitingForResponse = false;
        }

        public override bool Update()
        {
            if (!_client.Connected)
            {
                // Attempt to connect again
                DoConnect();
                if (_client.Connected && _waitingForResponse)
                {
                    // Resubmit
                    _waitingForResponse = false;
                }
            }
            else
            {
                if (!_waitingForResponse)
                {
                    _client.PuzzleComplete(_gi.AuthString, _tower, _floor, _completionTime, _noExpBonus);
                    _waitingForResponse = true;
                }
                else
                {
                    _client.Update();
                    if (!_waitingForResponse)
                    {
                        // Trigger job complete
                        _jobsDone(null);

                        // Close the connection
                        _client.Close();

                        // Finish the job
                        return false;
                    }
                }
            }

            // Keep working til we get a response
            return true;
        }
    }

    class SW_SpendCoinsJob : SW_Job
    {
        string _authToken;
        int _coinCount;
        int _spentOn;
        GameInfo _gi;

        bool _waitingForResponse;

        public SW_SpendCoinsJob(string authToken, int coinCount, int spentOn, GameInfo gi) : base("SW_SpendCoinsJob")
        {
            _authToken = authToken;
            _coinCount = coinCount;
            _spentOn = spentOn;
            _gi = gi;
            _waitingForResponse = false;
        }

        public override void Start()
        {
            base.Start();
            _client.OnHardCurrencyUpdate += _client_OnHardCurrencyUpdate;
        }

        private void _client_OnHardCurrencyUpdate(object sender, EventArgs e)
        {
            _waitingForResponse = false;
            _gi.VipData = _client.VipInfo;
            _gi.HardCurrency = _client.HardCurrency;            
        }

        public override bool Update()
        {
            if (!_client.Connected)
            {
                _client.Connect(HClient.ServerAddress, HClient.ServerPort);
                if (_client.Connected && _waitingForResponse)
                {
                    // Already sent the spend, but had to reconnect
                    // Request the balance here
                    _client.RequestCoinBalance(_authToken);
                }
            }
            else
            {
                if (!_waitingForResponse)
                {
                    _client.SpendCoins(_authToken, _coinCount, _spentOn);
                    _waitingForResponse = true;
                }
                else
                {
                    _client.Update();
                    if (!_waitingForResponse)
                    {
                        _client.Close();
                        return false;
                    }
                }
            }

            return true;
        }
    }
    
    public class SW_RequestProductsJob : SW_Job
    {
        bool _requested;
        bool _finished;
        bool _destroy;

        public bool Finished { get { return _finished; } }
        public GlobalProduct[] Products { get; set; }

        public SW_RequestProductsJob() : base("SW_RequestProductsJob")
        {
            _destroy = false;
        }

        public void Destroy()
        {
            _destroy = true;
        }

        public override void Start()
        {
            _finished = false;
            _requested = false;
            base.Start();
            _client.OnProductsResponse += _client_OnProductsResponse;  
        }

        private void _client_OnProductsResponse(GlobalProduct[] obj)
        {
            Products = obj;
            _finished = true;
        }

        public override bool Update()
        {
            if (!_finished)
            {
                if (!_client.Connected)
                {
                    _client.Connect(HClient.ServerAddress, HClient.ServerPort);
                    if (_client.Connected)
                        _requested = false;
                }

                if (_client.Connected)
                {
                    if (!_requested)
                    {
                        _client.RequestProducts();
                        _requested = true;
                    }

                    _client.Update();
                }
            }

            if( _destroy )
                return false;            

            return true;
        }
    }

    public class SW_VerifyPurchaseJob : SW_Job
    {
        bool _finished;
        bool _destroy;

        public bool Finished { get { return _finished; } }

        public SW_VerifyPurchaseJob() : base("SW_RequestProductsJob")
        {
        }

        public override void Start()
        {
            base.Start();
            _client.OnHardCurrencyUpdate += _client_OnHardCurrencyUpdate;
            _finished = false;

            if (_client.Connected)
                _client.SetAccountId(Happiness.Game.AccountId);
        }

        public void Destroy()
        {
            _destroy = true;
        }

        private void _client_OnHardCurrencyUpdate(object sender, EventArgs e)
        {
            HClient hc = (HClient)sender;
            Happiness.Game.TheGameInfo.VipData = hc.VipInfo;
            Happiness.Game.TheGameInfo.HardCurrency = hc.HardCurrency;
            _finished = true;            
        }

        public override bool Update()
        {
            if (!_finished)
            {
                if (!_client.Connected)
                {
                    _client.Connect(HClient.ServerAddress, HClient.ServerPort);
                    if( _client.Connected )
                        _client.SetAccountId(Happiness.Game.AccountId);
                }

                if(_client.Connected)
                    _client.Update();
            }

            if( _destroy && _finished )
                return false;


            return true;
        }
    }
}
