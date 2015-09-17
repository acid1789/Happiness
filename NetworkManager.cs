using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HappinessNetwork;
using System.IO;

namespace Happiness
{
    class NetworkManager
    {
        public enum SignInStatus
        {
            None,
            CredentialsSent,
            InvalidPassword,
            InvalidAccount,
            SignedIn
        }

        NetworkLog m_NetworkLog;
        HClient m_Client;
        Thread m_ClientPump;
        bool m_bDisabled;

        string m_szServerAddress;
        int m_iServerPort;
        string m_szEmail;
        string m_szPassword;

        SignInStatus m_SignInStatus;
        GameDataArgs m_GameData;
        VipDataArgs m_VipData;

        public NetworkManager()
        {
            m_NetworkLog = new NetworkLog();

            m_Client = new HClient();
            m_Client.OnAccountResponse += M_Client_OnAccountResponse;
            m_Client.OnGameDataResponse += M_Client_OnGameDataResponse;

            m_ClientPump = new Thread(new ThreadStart(ClientPumpThread));
            m_ClientPump.Name = "Client Pump Thread";
            m_ClientPump.Start();

            m_SignInStatus = SignInStatus.None;
        }

        public void Shutdown()
        {
            if (m_ClientPump != null)
            {
                m_ClientPump.Abort();
                m_ClientPump = null;
            }
        }

        void ClientPumpThread()
        {
            while (true)
            {
                if (!m_bDisabled)
                {
                    if (!m_Client.Connected && m_szServerAddress != null)
                        m_Client.Connect(m_szServerAddress, m_iServerPort);

                    if (m_Client.Connected)
                        m_Client.Update();
                }
                Thread.Sleep(100);
            }
        }

        public void Connect(string address, int port)
        {
            m_szServerAddress = address;
            m_iServerPort = port;
        }

        public bool Connect()
        {
            if (!m_Client.Connected)
            {
                m_Client.Connect(m_szServerAddress, m_iServerPort);
            }
            return m_Client.Connected;
        }

        public void Disconnect()
        {
            if (m_Client.Connected)
            {
                m_Client.Close();
            }
        }

        public void SignIn(string email, string password, string displayName)
        {
            m_szEmail = email;
            m_szPassword = password;  
            SignIn(displayName);          
        }

        public void SignIn(string displayName = null)
        {
            if (m_SignInStatus != SignInStatus.CredentialsSent)
            {
                m_Client.SendAccountRequest(m_szEmail, m_szPassword, displayName);
                m_SignInStatus = SignInStatus.CredentialsSent;
            }
        }

        public void FetchGameData()
        {
            m_Client.FetchGameData();
        }

        public void PuzzleComplete(int tower, int floor, double completionTime)
        {
            if (m_GameData.TowerFloors[tower] >= floor)
            {
                m_GameData.TowerFloors[tower] = floor + 1;

                if (m_bDisabled)
                    StoreStaticData();
                else
                    m_Client.PuzzleComplete(tower, floor, (float)completionTime);
            }
        }

        public void SpendCoins(int coinCount, int spentOn)
        {
            if (m_bDisabled)
            {
                m_Client.HardCurrency -= coinCount;
                StoreStaticData();
            }
            else
                m_Client.SpendCoins(coinCount, spentOn);
        }

        #region Response Handlers
        private void M_Client_OnAccountResponse(object sender, EventArgs e)
        {
            if( m_Client.AccountId < 0 )
                m_SignInStatus = SignInStatus.InvalidAccount;
            else if( m_Client.DisplayName == null )
                m_SignInStatus = SignInStatus.InvalidPassword;
            else
                m_SignInStatus = SignInStatus.SignedIn;
        }

        private void M_Client_OnGameDataResponse(object sender, GameDataArgs e)
        {
            m_GameData = e;
        }
        #endregion

        #region StaticData
        const string s_GameDataFile = "gamedata";
        public void LoadStaticData()
        {
            GameDataArgs gd = new GameDataArgs();
            gd.TowerFloors = new int[4];

            // Load game data if it exists
            if (File.Exists(s_GameDataFile))
            {
                FileStream fs = File.OpenRead(s_GameDataFile);
                BinaryReader br = new BinaryReader(fs);

                int version = br.ReadInt32();
                for (int i = 0; i < gd.TowerFloors.Length; i++)
                    gd.TowerFloors[i] = br.ReadInt32();
                gd.Level = br.ReadInt32();
                gd.Exp = br.ReadInt32();
                m_Client.HardCurrency = br.ReadInt32();

                br.Close();
            }
            else
            {
                gd.TowerFloors[0] = 1;
                gd.TowerFloors[1] = 0;
                gd.TowerFloors[2] = 0;
                gd.TowerFloors[3] = 0;
                gd.Level = 1;
                gd.Exp = 0;
                m_Client.HardCurrency = 1000;
            }

            m_GameData = gd;

            if (m_VipData == null)
            {
                m_VipData = new VipDataArgs();
                m_VipData.Level = 1;
                m_VipData.Progress = 0;
                m_VipData.Hints = 5;
                m_VipData.MegaHints = 2;
                m_VipData.UndoSize = 5;
            }
        }

        void StoreStaticData()
        {
            if (m_GameData != null)
            {
                FileStream fs = File.Create(s_GameDataFile);
                BinaryWriter bw = new BinaryWriter(fs);
                
                bw.Write(1);    // version
                foreach(int floor in m_GameData.TowerFloors )
                    bw.Write(floor);
                bw.Write(m_GameData.Level);
                bw.Write(m_GameData.Exp);
                bw.Write(m_Client.HardCurrency);

                bw.Close();
            }
        }
        #endregion

        #region Accessors
        public SignInStatus SignInState
        {
            get { return m_SignInStatus; }
        }

        public GameDataArgs GameData
        {
            get { return m_GameData; }
        }

        public VipDataArgs VipData
        {
            get { return m_VipData; }
        }

        public int HardCurrency
        {
            get { return m_Client.HardCurrency; }
        }

        public bool Connected
        {
            get { return m_Client.Connected; }
        }

        public bool Disabled
        {
            get { return m_bDisabled; }
            set { m_bDisabled = value; }
        }

        public string DisplayName
        {
            get { return m_bDisabled ? "Static" : m_Client.DisplayName; }
        }
        #endregion

        #region Static Access
        static NetworkManager s_NetworkManager;
        public static NetworkManager Net
        {
            get { if (s_NetworkManager == null) s_NetworkManager = new NetworkManager(); return s_NetworkManager; }
        }
        #endregion
    }
}
