using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using HappinessNetwork;

namespace Happiness
{
    class GameInfoValidator
    {
        public enum LoadStatus
        {
            Idle,
            Loading,
            FetchingFromServer,
            ServerUnreachable,
            ServerDeniedAccess,
            ServerFetchComplete,
            LoadedNoValidation,
            NoFile,
            OAuth,
            OAuthFailed
        }

        const string LocalFileName = "hgi.dat";
        
        Task _loadThread;
        LoadStatus _loadStatus;
        public GameInfo m_GameInfo;

        string _userName;
        string _passWord;
        bool _accountCreate;
        int _oauthMode;

        static GameInfoValidator s_instance;

        public static GameInfoValidator Instance { get { if( s_instance == null ) s_instance = new GameInfoValidator(); return s_instance; } }

        GameInfoValidator()
        {
            if( s_instance != null )
                throw new Exception("Only one GameInfoValidator is allowed");
            s_instance = this;

            _loadThread = null;
            _loadStatus = LoadStatus.Idle;
        }

        public void Reset()
        {
            if (_loadThread != null)
            {
                _loadThread = null;
            }
            m_GameInfo = null;
            _loadStatus = LoadStatus.Idle;
        }

        public void StartOAuth()
        {
            _loadStatus = LoadStatus.OAuth;
        }

        public void FinishOAuth(string email, string uniqueId, bool google)
        {
            if (email == null)
                _loadStatus = LoadStatus.OAuthFailed;
            else
            {
                // Sign in with the provided oauth credentials
                RequestFromServer(email, uniqueId, true, google ? 1 : 2);
            }
        }

        public void RequestFromServer(string username, string password, bool createMode, int oauthMode = 0)
        {
            _userName = username;
            _passWord = password;
            _accountCreate = createMode;
            _oauthMode = oauthMode;
            StartThread(ServerRequestFunc);
        }

        public void BeginLoadFromDisk()
        {            
            StartThread(LoadThreadFunc);            
        }

        public void Save(GameInfo gi)
        {
            m_GameInfo = gi;
            SaveToDisk();
        }

        public void DeleteLocalFile()
        {
            string localFile = GetUserLocalFile();
            if( FileManager.Instance.Exists(localFile) )
                FileManager.Instance.Delete(localFile);
        }

        string GetUserLocalFile()
        {
            string localHappinessDir = FileManager.Instance.HappinessPath;
            FileManager.Instance.CreateDirectory(localHappinessDir);
            return localHappinessDir + LocalFileName;
        }

        void SaveToDisk()
        {
            MemoryStream fs = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(GameInfo.GameInfoVersion);
            m_GameInfo.Save(bw);
            FileManager.Instance.WriteAllBytes(GetUserLocalFile(), fs.ToArray());
            bw.Dispose();
        }

        void StartThread(System.Action ThreadFunction)
        {
            if (_loadStatus != LoadStatus.Loading &&
                _loadStatus != LoadStatus.FetchingFromServer)
            {
                _loadThread = Task.Run(ThreadFunction);
            }
        }

        void LoadThreadFunc()
        {
            _loadStatus = LoadStatus.Loading;

            // Check for the local file
            if (FileManager.Instance.Exists(GetUserLocalFile()))
            {
                // Read the file
                byte[] data = FileManager.Instance.ReadAllBytes(GetUserLocalFile());
                MemoryStream fs = new MemoryStream(data);
                BinaryReader br = new BinaryReader(fs);

                int version = br.ReadInt32();
                m_GameInfo = new GameInfo();
                m_GameInfo.Load(br, version);
                br.Dispose();

                // Hash the data
                m_GameInfo.GenerateHash();

                // Validate with the server
                if (m_GameInfo.AuthString != null)
                    ValidateWithServer();
                else
                    _loadStatus = LoadStatus.LoadedNoValidation;
            }
            else
                _loadStatus = LoadStatus.NoFile;

            _loadThread = null;
        }

        void ServerRequestFunc()
        {
            _loadStatus = LoadStatus.FetchingFromServer;

            // Connect to server
            HClient client = new HClient("ServerRequestFunc");
            client.Connect(HClient.ServerAddress, HClient.ServerPort);
            client.OnGameInfoResponse += Client_OnGameInfoResponse;
            client.OnAccountResponse += Client_OnAccountResponse;

            if (!client.Connected)
                _loadStatus = LoadStatus.ServerUnreachable;
            else
            {
                // Send request
                client.SendAccountRequest(_userName, _passWord, _accountCreate ? _userName : null, _oauthMode);

                // Wait for response
                while (client.Connected && _loadStatus == LoadStatus.FetchingFromServer)
                {
                    client.Update();
                    Task.Delay(10).Wait();
                }

                client.Close();
            }
        }

        void ValidateWithServer()
        {
            _loadStatus = LoadStatus.FetchingFromServer;

            // Connect to server
            HClient client = new HClient("ValidateWithServer");
            client.Connect(HClient.ServerAddress, HClient.ServerPort);
            client.OnGameInfoResponse += Client_OnGameInfoResponse;
            client.OnAccountResponse += Client_OnAccountResponse;

            // Send request
            client.SendValidateGameInfoRequest(m_GameInfo.AuthString, Encoding.UTF8.GetString(m_GameInfo.Hash, 0, m_GameInfo.Hash.Length));

            // Wait for response
            while (client.Connected && _loadStatus == LoadStatus.FetchingFromServer)
            {
                client.Update();
                Task.Delay(10).Wait();
            }

            client.Close();
        }

        private void Client_OnAccountResponse(object sender, EventArgs e)
        {
            // Only way to get this here is to be denied access because our auth string is not legit?
            _loadStatus = LoadStatus.ServerDeniedAccess;
        }

        private void Client_OnGameInfoResponse(HClient arg1, GameInfo arg2, int accountId)
        {
            Happiness.Game.AccountId = accountId;
            if (arg2 != null)
            {   
                m_GameInfo = arg2;
                m_GameInfo.GenerateHash();
                SaveToDisk();
            }

            _loadStatus = LoadStatus.ServerFetchComplete;
        }

        public LoadStatus Status { get { return _loadStatus; } }
    }
}
