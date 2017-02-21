using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            ServerDeniedAccess,
            ServerFetchComplete,
            LoadedNoValidation,
            NoFile,
        }

        const string LocalFileName = "hgi.dat";
        
        Thread _loadThread;
        LoadStatus _loadStatus;
        GameInfo _gi;

        public GameInfoValidator()
        {
            _loadThread = null;
            _loadStatus = LoadStatus.Idle;
        }

        public void Reset()
        {
            if (_loadThread != null)
            {
                _loadThread.Abort();
                _loadThread = null;
            }
            _gi = null;
            _loadStatus = LoadStatus.Idle;
        }

        public void BeginLoadFromDisk()
        {
            if (_loadStatus != LoadStatus.Loading &&
                _loadStatus != LoadStatus.FetchingFromServer )
            {
                _loadThread = new Thread(new ThreadStart(LoadThreadFunc));
                _loadThread.Name = "GameInfo Loading Thread";
                _loadThread.Start();
                _loadStatus = LoadStatus.Loading;
            }
        }

        void SaveToDisk()
        {
            FileStream fs = File.Open(LocalFileName, FileMode.OpenOrCreate);
            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(GameInfo.GameInfoVersion);
            bw.Write(_gi.AuthString);
            _gi.Save(bw);
            bw.Close();
        }

        void LoadThreadFunc()
        {
            // Check for the local file
            if (File.Exists(LocalFileName))
            {
                // Read the file
                FileStream fs = File.OpenRead(LocalFileName);
                BinaryReader br = new BinaryReader(fs);

                int version = br.ReadInt32();
                _gi.AuthString = br.ReadString();
                _gi.Load(br, version);
                br.Close();

                // Hash the data
                _gi.GenerateHash();

                // Validate with the server
                if (_gi.AuthString != null)
                    ValidateWithServer();
                else
                    _loadStatus = LoadStatus.LoadedNoValidation;
            }
            else
                _loadStatus = LoadStatus.NoFile;

            _loadThread = null;
        }

        void ValidateWithServer()
        {
            _loadStatus = LoadStatus.FetchingFromServer;

            // Connect to server
            HClient client = new HClient();
            client.Connect(HClient.ServerAddress, HClient.ServerPort);
            client.OnGameInfoResponse += Client_OnGameInfoResponse;
            client.OnAccountResponse += Client_OnAccountResponse;

            // Send request
            client.SendValidateGameInfoRequest(_gi.AuthString, Encoding.UTF8.GetString(_gi.Hash));

            // Wait for response
            while (client.Connected && _loadStatus == LoadStatus.FetchingFromServer)
            {
                client.Update();
                Thread.Sleep(10);
            }

            client.Close();
        }

        private void Client_OnAccountResponse(object sender, EventArgs e)
        {
            // Only way to get this here is to be denied access because our auth string is not legit?
            _loadStatus = LoadStatus.ServerDeniedAccess;
        }

        private void Client_OnGameInfoResponse(HClient arg1, GameInfo arg2)
        {
            if (arg2 != null)
            {
                arg2.AuthString = _gi.AuthString;
                _gi = arg2;
                _gi.GenerateHash();
                SaveToDisk();
            }

            _loadStatus = LoadStatus.ServerFetchComplete;
        }

        public LoadStatus Status { get { return _loadStatus; } }
    }
}
