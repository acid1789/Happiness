using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HappinessNetwork;
using ServerCore;

namespace HappinessServer
{
    public class HTask : GSTask
    {
        public enum HTaskType
        {
            GameData_Fetch = GSTask.GSTType.Last,
            GameData_Fetched,
            PuzzleComplete_FetchData,
            PuzzleComplete_Validate,
            SpendCoins,
            TowerData_Fetch,
            TowerData_Process,
            FloorRecord_Process,
            TutorialData_Store,
            CoinBalance_Process,

            ValidateGameInfo,
            ValidateGameInfo_GameDataProcess,
            ValidateGameInfo_FloorInfoProcess,
        }

        public HTask(HTaskType type, HClient client = null, params object[] args)
        {
            Type = (int)type;
            _client = client;
            _args = args;
        }

        public new HClient Client
        {
            get { return (HClient)_client; }
            set { _client = value; }
        }
    }

    public class HTaskProcessor : GSTaskProcessor
    {

        public HTaskProcessor(GameServer server) : base(server)
        {
            // Add task handlers here
            _taskHandlers[(int)HTask.HTaskType.GameData_Fetch] = GameData_Fetch_Handler;
            _taskHandlers[(int)HTask.HTaskType.GameData_Fetched] = GameData_Fetched_Handler;
            _taskHandlers[(int)HTask.HTaskType.PuzzleComplete_FetchData] = PuzzleComplete_FetchData_Handler;
            _taskHandlers[(int)HTask.HTaskType.PuzzleComplete_Validate] = PuzzleComplete_Validate_Handler;
            _taskHandlers[(int)HTask.HTaskType.SpendCoins] = SpendCoins_Handler;
            _taskHandlers[(int)HTask.HTaskType.TowerData_Fetch] = TowerData_Fetch_Handler;
            _taskHandlers[(int)HTask.HTaskType.TowerData_Process] = TowerData_Process_Handler;
            _taskHandlers[(int)HTask.HTaskType.FloorRecord_Process] = FloorRecord_Process_Handler;
            _taskHandlers[(int)HTask.HTaskType.TutorialData_Store] = TutorialData_Store_Handler;
            _taskHandlers[(int)HTask.HTaskType.CoinBalance_Process] = CoinBalance_Process_Handler;

            _taskHandlers[(int)HTask.HTaskType.ValidateGameInfo] = ValidateGameInfo_Handler;
            _taskHandlers[(int)HTask.HTaskType.ValidateGameInfo_GameDataProcess] = ValidateGameInfo_GameDataProcess_Handler;
            _taskHandlers[(int)HTask.HTaskType.ValidateGameInfo_FloorInfoProcess] = ValidateGameInfo_FloorInfoProcess_Handler;
        }

        #region Task Handlers
        void GameData_Fetch_Handler(Task t)
        {
            HTask task = (HTask)t;
            string sql = string.Format("SELECT * FROM game_data WHERE account_id={0};", task.Client.AccountId);
            t.Type = (int)HTask.HTaskType.GameData_Fetched;
            AddDBQuery(sql, t);
        }

        void GameData_Fetched_Handler(Task t)
        {
            HTask task = (HTask)t;
            if (task.Query.Rows.Count <= 0)
            {
                // game data doesnt exist for this user yet
                string sql = string.Format("INSERT INTO game_data SET account_id={0},tower_floor_0=1;", task.Client.AccountId);
                t.Type = (int)HTask.HTaskType.GameData_Fetch;
                AddDBQuery(sql, t);

                // Also give this user 100 coins
                SpendCoinsArgs args = new SpendCoinsArgs();
                args.Coins = -100;
                args.SpendOn = 0;
                AddTask(new HTask(HTask.HTaskType.SpendCoins, task.Client, args));
            }
            else
            {
                GameDataArgs gameData = ReadGameData(task.Query);
                task.Client.SendGameData(gameData);
            }
        }

        void PuzzleComplete_FetchData_Handler(Task t)
        {
            HTask task = (HTask)t;
            object[] args = (object[])t.Args;
            PuzzleCompleteArgs pca = (PuzzleCompleteArgs)args[0];

            // Validate auth string
            AuthStringManager.AuthAccountInfo aai = _server.AuthManager.FindAccount(pca.AuthToken);
            if (aai == null)
            {
                // This account isnt in the cache, need to go fetch from global server   
                task.Client.PendingAuthTask = task;
                _server.GlobalServer.FetchAuthString(pca.AuthToken, task.Client.SessionKey);
                return;
            }

            string sql = string.Format("SELECT * FROM game_data WHERE account_id={0};", aai.AccountID);
            task.Client.AccountId = aai.AccountID;
            task.Type = (int)HTask.HTaskType.PuzzleComplete_Validate;
            AddDBQuery(sql, t);
        }

        void PuzzleComplete_Validate_Handler(Task t)
        {
            HTask task = (HTask)t;
            GameDataArgs gameData = ReadGameData(t.Query);

            object[] args = (object[])t.Args;
            PuzzleCompleteArgs pca = (PuzzleCompleteArgs)args[0];
            if (pca.TowerIndex < 0 || pca.TowerIndex >= gameData.TowerFloors.Length)
            {
                // Invalid tower number
                RecordError(task.Client.AccountId, "Invalid floor number submitted: " + pca.TowerIndex);
            }
            else
            {
                if (pca.FloorNumber > gameData.TowerFloors[pca.TowerIndex])
                {
                    // Trying to complete a puzzle further ahead than the current progress?!
                    RecordError(task.Client.AccountId, string.Format("Out of order completion - got:{0}, expected{1}", pca.FloorNumber, gameData.TowerFloors[pca.TowerIndex]));
                }
                else
                {
                    // This is the expected puzzle or a repeat of a previous puzzle

                    // Move to the next puzzle on this floor
                    if (pca.FloorNumber == gameData.TowerFloors[pca.TowerIndex])
                        gameData.TowerFloors[pca.TowerIndex]++;
                    
                    // Level up?
                    double baseExp = Balance.BaseExp(pca.TowerIndex);
                    double bonusExp = Balance.BonusExp(pca.TowerIndex, pca.CompletionTime);
                    double total = baseExp + bonusExp;
                    int exp = (int)total;
                    gameData.Exp += exp;
                    int expForNextLevel = Balance.ExpForNextLevel(gameData.Level);
                    while (gameData.Exp >= expForNextLevel)
                    {
                        gameData.Level++;
                        gameData.Exp -= expForNextLevel;
                        expForNextLevel = Balance.ExpForNextLevel(gameData.Level);
                    }

                    // Unlock next tower?
                    if (pca.TowerIndex < (gameData.TowerFloors.Length - 1) &&
                        gameData.TowerFloors[pca.TowerIndex + 1] == 0 &&
                        gameData.Level >= Balance.UnlockThreshold(pca.TowerIndex) )
                    {
                        gameData.TowerFloors[pca.TowerIndex + 1] = 1;
                    }

                    // Save changes
                    string sql = string.Format("UPDATE game_data SET tower0={0}, tower1={1}, tower2={2}, tower3={3}, tower4={4}, tower5={5}, level={6}, exp={7} WHERE account_id={8};",
                                                                     gameData.TowerFloors[0], gameData.TowerFloors[1], gameData.TowerFloors[2], gameData.TowerFloors[3], gameData.TowerFloors[4], gameData.TowerFloors[5], gameData.Level, gameData.Exp, task.Client.AccountId);
                    AddDBQuery(sql, null, false);
                    task.Client.SendGameData(gameData);

                    // Store this completion record
                    sql = string.Format("SELECT * FROM floor_records WHERE account_id={0} AND tower={1} AND floor={2};", task.Client.AccountId, pca.TowerIndex, pca.FloorNumber);
                    task.Type = (int)HTask.HTaskType.FloorRecord_Process;
                    AddDBQuery(sql, task);
                }
            }
        }

        void SpendCoins_Handler(Task t)
        {
            HTask task = (HTask)t;
            object[] argsA = (object[])t.Args;
            SpendCoinsArgs args = (SpendCoinsArgs)argsA[0];

            // Validate auth string
            AuthStringManager.AuthAccountInfo aai = _server.AuthManager.FindAccount(args.AuthToken);
            if (aai == null)
            {
                // This account isnt in the cache, need to go fetch from global server   
                task.Client.PendingAuthTask = task;
                _server.GlobalServer.FetchAuthString(args.AuthToken, task.Client.SessionKey);
                return;
            }

            // Record this spend in the database
            string sql = string.Format("INSERT INTO spends SET account_id={0}, amount={1}, reason={2}, timestamp={3}; SELECT LAST_INSERT_ID();", aai.AccountID, args.Coins, args.SpendOn, DateTime.Now.Ticks);
            task.Client.AccountId = aai.AccountID;
            t.Type = (int)GSTask.GSTType.SpendCoins_Global;
            t.Args = args.Coins;
            AddDBQuery(sql, t);
        }

        void TowerData_Fetch_Handler(Task t)
        {
            HTask task = (HTask)t;
            TowerDataRequstArgs args = (TowerDataRequstArgs)t.Args;
            string sql = string.Format("SELECT * FROM floor_records WHERE account_id={0} AND tower={1};", task.Client.AccountId, args.Tower);
            t.Type = (int)HTask.HTaskType.TowerData_Process;
            AddDBQuery(sql, t);
        }

        void TowerData_Process_Handler(Task t)
        {
            HTask task = (HTask)t;
            TowerDataRequstArgs args = (TowerDataRequstArgs)t.Args;

            // get the records            
            task.Client.SendTowerData(args.Tower, ReadTowerFloors(t));
        }

        void FloorRecord_Process_Handler(Task t)
        {
            HTask task = (HTask)t;
            object[] args = (object[])t.Args;
            PuzzleCompleteArgs pca = (PuzzleCompleteArgs)args[0];
            if (task.Query.Rows.Count > 0)
            {
                // record exists, see if this time is better
                object[] row = task.Query.Rows[0];
                int oldTime = (int)row[3];
                int newTime = (int)pca.CompletionTime;
                if (newTime < oldTime)
                {
                    // Better time, update the record
                    string sql = string.Format("UPDATE floor_records SET best_time={0} WHERE account_id={1} AND tower={2} AND floor={3};", newTime, task.Client.AccountId, pca.TowerIndex, pca.FloorNumber);
                    AddDBQuery(sql, null, false);
                }
            }
            else
            {
                // No record for this floor yet, just submit what we have
                string sql = string.Format("INSERT INTO floor_records SET account_id={0}, tower={1}, floor={2}, best_time={3}, friend_rank={4}, global_rank={5};", task.Client.AccountId, pca.TowerIndex, pca.FloorNumber, (int)pca.CompletionTime, 0, 0);
                AddDBQuery(sql, null, false);
            }
        }

        void TutorialData_Store_Handler(Task t)
        {
            HTask task = (HTask)t;
            object[] args = (object[])t.Args;
            ulong tutorialData = (ulong)args[0];
            string authToken = (string)args[1];

            AuthStringManager.AuthAccountInfo aai = _server.AuthManager.FindAccount(authToken);
            if (aai == null)
            {
                // This account isnt in the cache, need to go fetch from global server   
                task.Client.PendingAuthTask = task;
                _server.GlobalServer.FetchAuthString(authToken, task.Client.SessionKey);
                return;
            }

            string sql = string.Format("UPDATE game_data SET tutorial={0} WHERE account_id={1};", tutorialData, aai.AccountID);
            AddDBQuery(sql, null, false);
        }

        void CoinBalance_Process_Handler(Task t)
        {
            HTask task = (HTask)t;

            object[] args = (object[])t.Args;
            string authToken = (string)args[0];

            AuthStringManager.AuthAccountInfo aai = _server.AuthManager.FindAccount(authToken);
            if (aai == null)
            {
                // This account isnt in the cache, need to go fetch from global server   
                task.Client.PendingAuthTask = task;
                _server.GlobalServer.FetchAuthString(authToken, task.Client.SessionKey);
                return;
            }

            task.Client.CurrencyUpdate(aai.HardCurrency);
        }

        void ValidateGameInfo_Handler(Task t)
        {
            HTask task = (HTask)t;

            object[] args = (object[])t.Args;
            string authString = (string)args[0];
            string hash = (string)args[1];


            // Validate auth string            
            AuthStringManager.AuthAccountInfo aai = _server.AuthManager.FindAccount(authString);
            if (aai == null)
            {
                // This account isnt in the cache, need to go fetch from global server                   
                task.Client.PendingAuthTask = task;
                _server.GlobalServer.FetchAuthString(authString, task.Client.SessionKey);
                return;
            }

            ValidateGameInfoArgs gia = new ValidateGameInfoArgs(authString, hash);

            task.Client.AccountId = aai.AccountID;
            task.Client.HardCurrency = aai.HardCurrency;

            // Fetch game data
            string sql = string.Format("SELECT * FROM game_data WHERE account_id={0};", task.Client.AccountId);
            AddDBQuery(sql, new HTask(HTask.HTaskType.ValidateGameInfo_GameDataProcess, task.Client, gia));

            // Fetch floor data
            sql = string.Format("SELECT * FROM floor_records WHERE account_id={0};", task.Client.AccountId);
            AddDBQuery(sql, new HTask(HTask.HTaskType.ValidateGameInfo_FloorInfoProcess, task.Client, gia));
        }

        void ValidateGameInfo_GameDataProcess_Handler(Task t)
        {
            HTask task = (HTask)t;
            object[] args = (object[])t.Args;
            ValidateGameInfoArgs gia = (ValidateGameInfoArgs)args[0];
            
            if (t.Query.Rows.Count > 0)
            {
                gia.GameInfo.GameData = ReadGameData(t.Query);
            }
            else
            {
                // Put this record in the database
                string sql = string.Format("INSERT INTO game_data SET account_id={0},tower0=1;", task.Client.AccountId);                
                t.Type = -1;
                AddDBQuery(sql, t);

                // Also give this user 100 coins
                SpendCoinsArgs scargs = new SpendCoinsArgs();
                scargs.AuthToken = gia.AuthString;
                scargs.Coins = -100;
                scargs.SpendOn = 0;
                AddTask(new HTask(HTask.HTaskType.SpendCoins, task.Client, scargs));
                AuthStringManager.AuthAccountInfo aai = _server.AuthManager.FindAccount(gia.AuthString);
                aai.HardCurrency += 100;

                // Setup the default info
                gia.GameInfo.GameData = new GameDataArgs();
                gia.GameInfo.GameData.TowerFloors = new int[6];
                gia.GameInfo.GameData.TowerFloors[0] = 1;
            }

            if (gia.RequestFinished)
                ProcessValidateGameInfo(gia, task.Client);
        }

        void ValidateGameInfo_FloorInfoProcess_Handler(Task t)
        {
            Dictionary<int, List<TowerFloorRecord> > towerData = new Dictionary<int, List<TowerFloorRecord>>();
            foreach (object[] row in t.Query.Rows)
            {
                // 0: record_id
                // 1: account_id
                // 2: tower
                // 3: floor
                // 4: best_time
                // 5: friend_rank
                // 6: global_rank

                TowerFloorRecord record = new TowerFloorRecord();
                int tower = (int)row[2];

                if (!towerData.ContainsKey(tower))
                    towerData[tower] = new List<TowerFloorRecord>();

                record.Floor = (int)row[3];
                record.BestTime = (int)row[4];
                record.RankFriends = (int)row[5];
                record.RankGlobal = (int)row[6];
                towerData[tower].Add(record);
            }

            TowerData[] td = new TowerData[6];
            for (int i = 0; i < td.Length; i++)
            {
                td[i] = new TowerData();
                td[i].Tower = i;
                td[i].Floors = towerData.ContainsKey(i) ? towerData[i].ToArray() : null;
            }

            HTask task = (HTask)t;
            object[] args = (object[])t.Args;
            ValidateGameInfoArgs gia = (ValidateGameInfoArgs)args[0];
            gia.GameInfo.TowerData = td;

            if (gia.RequestFinished)
               ProcessValidateGameInfo(gia, task.Client);
        }
        #endregion

        void ProcessValidateGameInfo(ValidateGameInfoArgs gia, HClient client)
        {
            byte[] hash = gia.GameInfo.GenerateHash();
            string serverHash = Encoding.UTF8.GetString(hash);
            string clientHash = gia.Hash;

            AuthStringManager.AuthAccountInfo aai = _server.AuthManager.FindAccount(gia.AuthString);
            gia.GameInfo.AuthString = gia.AuthString;
            gia.GameInfo.DisplayName = aai.DisplayName;
            gia.GameInfo.HardCurrency = aai.HardCurrency;
            gia.GameInfo.VipData = VipData.Create(aai.Vip);            
            client.SendValidateGameInfoResponse(aai.AccountID, aai.HardCurrency, aai.Vip, serverHash == clientHash, gia.GameInfo);
        }

        #region Data Functions
        GameDataArgs ReadGameData(DBQuery query)
        {
            GameDataArgs gameData = new GameDataArgs();

            object[] row = query.Rows[0];
            gameData.TowerFloors = new int[6];            
            for (int i = 0; i < gameData.TowerFloors.Length; i++)
                gameData.TowerFloors[i] = (int)row[i + 1];

            gameData.Level = (int)row[7];
            gameData.Exp = (int)row[8];
            gameData.Tutorial = (ulong)row[9];

            return gameData;
        }

        TowerFloorRecord[] ReadTowerFloors(Task t)
        {
            List<TowerFloorRecord> floors = new List<TowerFloorRecord>();
            foreach (object[] row in t.Query.Rows)
            {
                // 0: account_id
                // 1: tower
                // 2: floor
                // 3: best_time
                // 4: friend_rank
                // 5: global_rank

                TowerFloorRecord record = new TowerFloorRecord();
                record.Floor = (int)row[2];
                record.BestTime = (int)row[3];
                record.RankFriends = (int)row[4];
                record.RankGlobal = (int)row[5];
                floors.Add(record);
            }
            return floors.ToArray();
        }
        #endregion

        

        void RecordError(int accountId, string error)
        {
            string sql = string.Format("INSERT INTO errors SET account_id={0}, error_str=\"{1}\", timestamp={2};", accountId, error, DateTime.Now.Ticks);
            AddDBQuery(sql, null, false);
        }

        class ValidateGameInfoArgs
        {
            public string AuthString;
            public string Hash;
            public GameInfo GameInfo;

            public ValidateGameInfoArgs(string authString, string hash)
            {
                AuthString = authString;
                Hash = hash;
                GameInfo = new GameInfo();
            }

            public bool RequestFinished
            {
                get { return (GameInfo.GameData != null && GameInfo.TowerData != null); }
            }
        }
    }
}
