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
        }

        public HTask(HTaskType type, HClient client = null, object args = null)
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
            string sql = string.Format("SELECT * FROM game_data WHERE account_id={0};", task.Client.AccountId);
            task.Type = (int)HTask.HTaskType.PuzzleComplete_Validate;
            AddDBQuery(sql, t);
        }

        void PuzzleComplete_Validate_Handler(Task t)
        {
            HTask task = (HTask)t;
            GameDataArgs gameData = ReadGameData(t.Query);

            PuzzleCompleteArgs pca = (PuzzleCompleteArgs)t.Args;
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
                else if (pca.FloorNumber == gameData.TowerFloors[pca.TowerIndex])
                {
                    // This is the expected puzzle

                    // Move to the next puzzle on this floor
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
                    string sql = string.Format("UPDATE game_data SET tower_floor_0={0}, tower_floor_1={1}, tower_floor_2={2}, tower_floor_3={3}, level={4}, exp={5} WHERE account_id={6};",
                                                                     gameData.TowerFloors[0], gameData.TowerFloors[1], gameData.TowerFloors[2], gameData.TowerFloors[3], gameData.Level, gameData.Exp, task.Client.AccountId);
                    AddDBQuery(sql, null, false);
                    task.Client.SendGameData(gameData);
                }
            }
        }

        void SpendCoins_Handler(Task t)
        {
            HTask task = (HTask)t;
            SpendCoinsArgs args = (SpendCoinsArgs)t.Args;

            // Record this spend in the database
            string sql = string.Format("INSERT INTO spends SET account_id={0}, amount={1}, reason={2}, timestamp={3}; SELECT LAST_INSERT_ID();", task.Client.AccountId, args.Coins, args.SpendOn, DateTime.Now.Ticks);
            t.Type = (int)GSTask.GSTType.SpendCoins_Global;
            t.Args = args.Coins;
            AddDBQuery(sql, t);
        }
        #endregion

        #region Data Functions
        GameDataArgs ReadGameData(DBQuery query)
        {
            GameDataArgs gameData = new GameDataArgs();

            object[] row = query.Rows[0];
            gameData.TowerFloors = new int[6];
            for (int i = 0; i < gameData.TowerFloors.Length; i++)
                gameData.TowerFloors[i] = (int)row[i + 1];

            gameData.Level = (int)row[5];
            gameData.Exp = (int)row[6];

            return gameData;
        }
        #endregion

        

        void RecordError(int accountId, string error)
        {
            string sql = string.Format("INSERT INTO errors SET account_id={0}, error_str=\"{1}\", timestamp={2};", accountId, error, DateTime.Now.Ticks);
            AddDBQuery(sql, null, false);
        }
    }
}
