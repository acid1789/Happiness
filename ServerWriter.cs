using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HappinessNetwork;

namespace Happiness
{
    public class ServerWriter
    {
        Thread _thread;
        Mutex _jobLock;
        List<SW_Job> _pendingJobs;
        bool _wantExit;

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

        public void SavePuzzleData(string authToken, int tower, int floor, double completionTime)
        {
            AddJob(new SW_SavePuzzleJob(authToken, tower, floor, (float)completionTime));
        }
        #endregion
    }

    abstract class SW_Job
    {
        protected HClient _client;

        public virtual void Start()
        {
            // Connect to the server
            _client = new HClient();
            _client.Connect(HClient.ServerAddress, HClient.ServerPort);
        }

        public abstract bool Update();
    }

    class SW_SaveTutorialDataJob : SW_Job
    {
        static DateTime _lastSentTime;

        ulong _tutorialData;
        string _authToken;
        DateTime _timeStamp;
        public SW_SaveTutorialDataJob(ulong tutorialData, string authToken, DateTime timeStamp)
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
            if ((_timeStamp - _lastSentTime).TotalSeconds > 0)
            {
                if (_client.Connected)
                {
                    _client.SendTutorialData(_tutorialData, _authToken);
                    _lastSentTime = _timeStamp;
                    _client.Close();
                }
            }

            // Return false here to be done with this job
            return false;
        }
    }

    class SW_SavePuzzleJob : SW_Job
    {
        string _authToken;
        int _tower;
        int _floor;
        float _completionTime;

        public SW_SavePuzzleJob(string authToken, int tower, int floor, float completionTime)
        {
            _authToken = authToken;
            _tower = tower;
            _floor = floor;
            _completionTime = completionTime;
        }

        public override bool Update()
        {
            if( !_client.Connected )
                return false;

            _client.PuzzleComplete(_authToken, _tower, _floor, _completionTime);
            _client.Close();
            return false;
        }
    }
}
