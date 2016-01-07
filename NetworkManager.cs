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
        TowerData m_TowerData;

        ulong m_pendingTutorialData;
        DateTime m_tutorialDataTime;

        public NetworkManager()
        {
            m_NetworkLog = new NetworkLog();

            m_Client = new HClient();
            m_Client.OnAccountResponse += M_Client_OnAccountResponse;
            m_Client.OnGameDataResponse += M_Client_OnGameDataResponse;
            m_Client.OnTowerDataResponse += M_Client_OnTowerDataResponse;

            m_ClientPump = new Thread(new ThreadStart(ClientPumpThread));
            m_ClientPump.Name = "Client Pump Thread";
            m_ClientPump.Start();

            m_SignInStatus = SignInStatus.None;

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


                    if (m_Client.Connected && m_pendingTutorialData != 0 && (DateTime.Now - m_tutorialDataTime).TotalSeconds >= 30)
                    {
                        m_Client.SendTutorialData(m_pendingTutorialData);
                        m_pendingTutorialData = 0;
                    }
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
            if (m_bDisabled)
            {
                StoreStaticData();
                int floorIndex = m_TowerData.Floors.Length - floor;
                if (floorIndex < m_TowerData.Floors.Length)
                {
                    if (m_TowerData.Floors[floorIndex].BestTime == 0 || m_TowerData.Floors[floorIndex].BestTime > completionTime)
                        m_TowerData.Floors[floorIndex].BestTime = (int)completionTime;
                }
                SaveStaticTowerData(tower, m_TowerData);
            }
            else
            {
                // Tell the server
                m_Client.PuzzleComplete(tower, floor, (float)completionTime);
            }

            int maxFloor = m_GameData.TowerFloors[tower];
            if (maxFloor == floor)
            {
                // Unlock the next floor
                m_GameData.TowerFloors[tower]++;

                if (m_TowerData.Floors.Length == floor)
                {
                    List<TowerFloorRecord> records = new List<TowerFloorRecord>(m_TowerData.Floors);
                    TowerFloorRecord nf = new TowerFloorRecord();
                    nf.Floor = floor + 1;
                    records.Insert(0, nf);
                    m_TowerData.Floors = records.ToArray();
                }
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

        public void RequestTowerData(int tower, bool shortList)
        {
            // Clear existing tower data
            m_TowerData = null;

            if (m_bDisabled)
            {
                // load data from disk
                LoadStaticTowerData(tower);
            }
            else
            {
                // Request data from server
                m_Client.RequestTowerData(tower, shortList);
            }
        }

        public void SaveTutorialData(ulong tutorialData)
        {
            m_pendingTutorialData = tutorialData;
            m_tutorialDataTime = DateTime.Now;
            m_GameData.Tutorial = tutorialData;

            if( m_bDisabled )
                StoreStaticData();
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


        private void M_Client_OnTowerDataResponse(object sender, TowerData e)
        {
            // tower data from server is not in order, re-order it now

            // find the highest floor and move into a dictionary
            int highestFloor = 0;
            Dictionary<int, TowerFloorRecord> map = new Dictionary<int, TowerFloorRecord>();
            foreach (TowerFloorRecord floor in e.Floors)
            {
                if( floor.Floor > highestFloor )
                    highestFloor = floor.Floor;
                map[floor.Floor] = floor;
            }

            e.Floors = new TowerFloorRecord[highestFloor + 1];
            e.Floors[0] = new TowerFloorRecord();
            e.Floors[0].Floor = highestFloor + 1;            
            foreach (KeyValuePair<int, TowerFloorRecord> kvp in map)
            {
                int index = (highestFloor - kvp.Key) + 1;
                e.Floors[index] = kvp.Value;
            }
            
            // Fix any nulls that might exist
            for (int i = 1; i < e.Floors.Length; i++)
            {
                TowerFloorRecord r = e.Floors[i];
                if (r == null)
                {
                    r = new TowerFloorRecord();
                    r.Floor = e.Floors[i - 1].Floor - 1;
                    e.Floors[i] = r;
                }
            }
                                    
            // store the data
            m_TowerData = e;
        }
        #endregion

        #region StaticData
        const string s_GameDataFile = "gamedata";
        public void LoadStaticData()
        {
            GameDataArgs gd = new GameDataArgs();
            gd.TowerFloors = new int[6];

            // Load game data if it exists
            if (File.Exists(s_GameDataFile))
            {
                FileStream fs = File.OpenRead(s_GameDataFile);
                BinaryReader br = new BinaryReader(fs);

                int version = br.ReadInt32();
                int towers = version == 1 ? 4 : gd.TowerFloors.Length;
                for (int i = 0; i < towers; i++)
                    gd.TowerFloors[i] = br.ReadInt32();
                gd.Level = br.ReadInt32();
                gd.Exp = br.ReadInt32();
                gd.Tutorial = br.ReadUInt32();
                m_Client.HardCurrency = br.ReadInt32();

                br.Close();
            }
            else
            {
                gd.TowerFloors[0] = 1;
                gd.TowerFloors[1] = 0;
                gd.TowerFloors[2] = 0;
                gd.TowerFloors[3] = 0;
                gd.TowerFloors[4] = 0;
                gd.TowerFloors[5] = 0;
                gd.Level = 1;
                gd.Exp = 0;
                gd.Tutorial = 0;
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

        public void StoreStaticData()
        {
            if (m_GameData != null)
            {
                FileStream fs = File.Create(s_GameDataFile);
                BinaryWriter bw = new BinaryWriter(fs);
                
                bw.Write(2);    // version
                foreach(int floor in m_GameData.TowerFloors )
                    bw.Write(floor);
                bw.Write(m_GameData.Level);
                bw.Write(m_GameData.Exp);
                bw.Write(m_GameData.Tutorial);
                bw.Write(m_Client.HardCurrency);

                bw.Close();
            }
        }

        const string s_TowerDataFile = "towerdata";
        void LoadStaticTowerData(int tower)
        {
            string file = s_TowerDataFile + tower;
            TowerData td = new TowerData();
            td.Tower = tower;
            List<TowerFloorRecord> floorRecords = new List<TowerFloorRecord>();
            if (File.Exists(file))
            {
                BinaryReader br = new BinaryReader(File.OpenRead(file));
                int floorCount = br.ReadInt32();
                if (floorCount > 0)
                {
                    for (int i = 0; i < floorCount; i++)
                    {
                        TowerFloorRecord tfr = new TowerFloorRecord();
                        tfr.Floor = i + 1;
                        tfr.BestTime = br.ReadInt32();
                        tfr.RankFriends = 0;
                        tfr.RankGlobal = 0;
                        floorRecords.Add(tfr);
                    }
                }
                br.Close();
            }

            // Add 'next' floor
            TowerFloorRecord nextFloor = new TowerFloorRecord();
            nextFloor.Floor = floorRecords.Count + 1;
            floorRecords.Add(nextFloor);
            floorRecords.Reverse();
            td.Floors = floorRecords.ToArray();
            
            m_TowerData = td;
        }

        void SaveStaticTowerData(int tower, TowerData td)
        {
            string file = s_TowerDataFile + tower;

            FileStream fs = File.Open(file, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);            
            bw.Write(td.Floors.Length);
            if (td.Floors.Length > 0)
            {
                List<TowerFloorRecord> floors = new List<TowerFloorRecord>(td.Floors);
                floors.Reverse();
                foreach (TowerFloorRecord tfr in floors)
                {
                    bw.Write(tfr.BestTime);
                }
            }
            bw.Close();
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

        public TowerData TowerData
        {
            get { return m_TowerData; }
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
