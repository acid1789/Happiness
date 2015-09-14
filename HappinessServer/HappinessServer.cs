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
            LogThread.AlwaysPrintToConsole = true;
            TaskProcessor = new HTaskProcessor(this);
        }

        public override GameClient CreateClient(System.Net.Sockets.Socket s)
        {
            HClient client = new HClient(s);

            // Register Happiness specific handlers
            client.OnGameDataRequest += Client_OnGameDataRequest;
            client.OnPuzzleComplete += Client_OnPuzzleComplete;
            client.OnSpendCoins += Client_OnSpendCoins;

            return client;
        }

        public override void NewAuthorizedClient(GameClient client)
        {
            base.NewAuthorizedClient(client);            
        }

        #region Client Event Handlers
        private void Client_OnGameDataRequest(object sender, EventArgs e)
        {
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.GameData_Fetch, (HClient)sender, null));
        }

        private void Client_OnPuzzleComplete(object sender, PuzzleCompleteArgs e)
        {
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.PuzzleComplete_FetchData, (HClient)sender, e));
        }

        private void Client_OnSpendCoins(object sender, SpendCoinsArgs e)
        {
            TaskProcessor.AddTask(new HTask(HTask.HTaskType.SpendCoins, (HClient)sender, e));
        }
        #endregion

        #region Accessors
        #endregion
    }
}
