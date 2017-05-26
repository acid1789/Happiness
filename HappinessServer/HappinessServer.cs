using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkCore;
using ServerCore;
using HappinessNetwork;

namespace HappinessServer
{
    class HappinessServer : GameServer
    {
        public HappinessServer(int listenPort, string dbConnectionString, string globalAddress, int globalPort) : base(listenPort, dbConnectionString, globalAddress, globalPort)
        {
#if DEBUG
            LogThread.AlwaysPrintToConsole = true;
#endif
            TaskProcessor = new HTaskProcessor(this);
        }

        public override GameClient CreateClient(System.Net.Sockets.Socket s)
        {
            HClient client = new HClient(s);

            // Register Happiness specific handlers
            client.OnGameDataRequest += Client_OnGameDataRequest;
            client.OnPuzzleComplete += Client_OnPuzzleComplete;
            client.OnSpendCoins += Client_OnSpendCoins;
            client.OnTowerDataRequest += Client_OnTowerDataRequest;
            client.OnTutorialData += Client_OnTutorialData;
            client.OnValidateGameInfo += Client_OnValidateGameInfo;
            client.OnCoinBalanceRequest += Client_OnCoinBalanceRequest;
            client.OnProductsRequest += Client_OnProductsRequest;

            return client;
        }

        public override void NewAuthorizedClient(GameClient client)
        {
            base.NewAuthorizedClient(client);
            

            TaskProcessor.AddTask(new HTask(HTask.HTaskType.ValidateGameInfo, (HClient)client, client.AuthString, null));
        }

#region Client Event Handlers
        private void Client_OnGameDataRequest(object sender, EventArgs e)
        {
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.GameData_Fetch, (HClient)sender, null));
        }

        private void Client_OnPuzzleComplete(object sender, PuzzleCompleteArgs e)
        {
            string str = string.Format("OnPuzzleComplete: Tower({0}), Floor({1}), Time({2}), AuthToken({3})", e.TowerIndex, e.FloorNumber, e.CompletionTime, AuthStringManager.AuthStringToAscii(e.AuthToken));
            LogThread.Log(str, LogInterface.LogMessageType.Normal, true);
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.PuzzleComplete_FetchData, (HClient)sender, e));
        }

        private void Client_OnSpendCoins(object sender, SpendCoinsArgs e)
        {
            string str = string.Format("OnSpendCoins: SpendOn({0}), Coins({1}), AuthToken({2})", e.SpendOn, e.Coins, AuthStringManager.AuthStringToAscii(e.AuthToken));
            LogThread.Log(str, LogInterface.LogMessageType.Normal, true);
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.SpendCoins, (HClient)sender, e));
        }

        private void Client_OnTowerDataRequest(object sender, TowerDataRequstArgs e)
        {
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.TowerData_Fetch, (HClient)sender, e));
        }

        private void Client_OnTutorialData(HClient arg1, ulong arg2, string authString)
        {
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.TutorialData_Store, arg1, arg2, authString));
        }

        private void Client_OnValidateGameInfo(HClient arg1, string authString, string hash)
        {
            string str = string.Format("OnValidateGameInfo: AuthToken({0}), Hash({1})", AuthStringManager.AuthStringToAscii(authString), AuthStringManager.AuthStringToAscii(hash));
            LogThread.Log(str, LogInterface.LogMessageType.Normal, true);
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.ValidateGameInfo, arg1, authString, hash));
        }

        private void Client_OnCoinBalanceRequest(HClient arg1, string obj)
        {
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.CoinBalance_Process, arg1, obj));
        }

        private void Client_OnProductsRequest(HClient obj)
        {
            obj.SendProducts(Products);
        }
        #endregion

        #region Accessors
        #endregion
    }
}
