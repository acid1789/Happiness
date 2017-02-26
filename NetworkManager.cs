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
        bool m_bDisabled;

        string m_szServerAddress;
        int m_iServerPort;
        string m_szEmail;
        string m_szPassword;

        SignInStatus m_SignInStatus;
        GameDataArgs m_GameData;
        VipDataArgs m_VipData;
        TowerData m_TowerData;
        

        public NetworkManager()
        {
            m_NetworkLog = new NetworkLog();
        }

        public void Shutdown()
        {
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
            /*
            // Tell the server
            m_Client.PuzzleComplete(tower, floor, (float)completionTime);

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
            */
        }

        /*
        public void SpendCoins(int coinCount, int spentOn)
        {
            if (m_bDisabled)
            {
                m_Client.HardCurrency -= coinCount;
            }
            else
                m_Client.SpendCoins(coinCount, spentOn);
        }
        */

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
        
        #region Static Access
        static NetworkManager s_NetworkManager;
        public static NetworkManager Net
        {
            get { if (s_NetworkManager == null) s_NetworkManager = new NetworkManager(); return s_NetworkManager; }
        }
        #endregion        
    }
}
