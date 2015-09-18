using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Sockets;
using NetworkCore;

namespace HappinessNetwork
{
    public class HClient : GameClient
    {
        public enum HPacketType
        {
            GameData_Request = 5000,
            GameData_Response,
            Puzzle_Complete,
            SpendCoins,
        }

        public event EventHandler OnGameDataRequest;
        public event EventHandler<GameDataArgs> OnGameDataResponse;   
        public event EventHandler<PuzzleCompleteArgs> OnPuzzleComplete;
        public event EventHandler<SpendCoinsArgs> OnSpendCoins;

        public HClient()
            : base(null)
        {
        }

        public HClient(Socket s)
            : base(s)
        {
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();
            _packetHandlers[(ushort)HPacketType.GameData_Request] = GameData_Request_Handler;
            _packetHandlers[(ushort)HPacketType.GameData_Response] = GameData_Response_Handler;
            _packetHandlers[(ushort)HPacketType.Puzzle_Complete] = Puzzle_Complete_Handler;
            _packetHandlers[(ushort)HPacketType.SpendCoins] = SpendCoins_Handler;
        }

        void BeginPacket(HPacketType type)
        {
            LogInterface.Log(string.Format("BeginPacket({0})", type), LogInterface.LogMessageType.Debug);
            BeginPacket((ushort)type);
        }

        #region Packet Construction
        public void FetchGameData()
        {
            BeginPacket(HPacketType.GameData_Request);
            SendPacket();
        }

        public void SendGameData(GameDataArgs gameData)
        {
            BeginPacket(HPacketType.GameData_Response);
            for( int i = 0; i < gameData.TowerFloors.Length; i++ )
                _outgoingBW.Write(gameData.TowerFloors[i]);

            _outgoingBW.Write(gameData.Level);
            _outgoingBW.Write(gameData.Exp);

            SendPacket();
        }

        public void PuzzleComplete(int tower, int floor, float completionTime)
        {
            BeginPacket(HPacketType.Puzzle_Complete);

            _outgoingBW.Write(tower);
            _outgoingBW.Write(floor);
            _outgoingBW.Write(completionTime);

            SendPacket();
        }

        public void SpendCoins(int coins, int spendOn)
        {
            HardCurrency -= coins;

            BeginPacket(HPacketType.SpendCoins);

            _outgoingBW.Write(coins);
            _outgoingBW.Write(spendOn);

            SendPacket();
        }
        #endregion

        #region Packet Handlers
        void GameData_Request_Handler(BinaryReader br)
        {
            OnGameDataRequest(this, null);
        }

        void GameData_Response_Handler(BinaryReader br)
        {
            GameDataArgs args = new GameDataArgs();
            args.TowerFloors = new int[6];
            for( int i = 0; i < args.TowerFloors.Length; i++ )
                args.TowerFloors[i] = br.ReadInt32();

            args.Level = br.ReadInt32();
            args.Exp = br.ReadInt32();

            OnGameDataResponse(this, args);
        }

        void Puzzle_Complete_Handler(BinaryReader br)
        {
            PuzzleCompleteArgs args = new PuzzleCompleteArgs();
            args.TowerIndex = br.ReadInt32();
            args.FloorNumber = br.ReadInt32();
            args.CompletionTime = br.ReadSingle();
            OnPuzzleComplete(this, args);
        }

        void SpendCoins_Handler(BinaryReader br)
        {
            SpendCoinsArgs args = new SpendCoinsArgs();
            args.Coins = br.ReadInt32();
            args.SpendOn = br.ReadInt32();

            HardCurrency -= args.Coins;

            OnSpendCoins(this, args);
        }
        #endregion

        #region Accessors
        #endregion
    }

    public class GameDataArgs : EventArgs
    {
        public int[] TowerFloors;
        public int Level;
        public int Exp;
    }

    public class PuzzleCompleteArgs : EventArgs
    {
        public int TowerIndex;
        public int FloorNumber;
        public float CompletionTime;
    }

    public class VipDataArgs : EventArgs
    {
        public int Level;
        public int Progress;
        public int Hints;
        public int MegaHints;
        public int UndoSize;
    }

    public class SpendCoinsArgs : EventArgs
    {
        public int Coins;
        public int SpendOn;
    }
}
