using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Sockets.Plugin;
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
            TowerData_Request,
            TowerData_Response,
            TutorialData,
            CoinBalance_Request,
            Close,
            Products_Request,
            Products_Response,
            Account_Id,

            ValidateGameInfo_Request,
            ValidateGameInfo_Response,

            //DebugInfo,
        }

        public event EventHandler OnGameDataRequest;
        public event EventHandler<GameDataArgs> OnGameDataResponse;
        public event EventHandler<PuzzleCompleteArgs> OnPuzzleComplete;
        public event EventHandler<SpendCoinsArgs> OnSpendCoins;
        public event EventHandler<TowerDataRequstArgs> OnTowerDataRequest;
        public event EventHandler<TowerData> OnTowerDataResponse;
        public event Action<HClient, ulong, string> OnTutorialData;
        public event Action<HClient, string, string> OnValidateGameInfo;
        public event Action<HClient, GameInfo, int> OnGameInfoResponse;        
        public event Action<HClient, string> OnCoinBalanceRequest;
        public event Action<HClient> OnProductsRequest;
        public event Action<GlobalProduct[]> OnProductsResponse;

        public delegate VipDataArgs BuildVipDataArgsFunc(int vipPoints);
        public BuildVipDataArgsFunc OnBuildVipDataArgs;

        public static string ServerAddress = "www.ronzgames.com";
        public static int ServerPort = 1255;

        public VipDataArgs VipInfo;

        //public string RemoteDebugInfo;

        public HClient(string name)
            : base(null)
        {
            //RemoteDebugInfo = name;
        }

        public HClient(Socket s)
            : base(s)
        {
        }

        public override void Close()
        {
            BeginPacket(HPacketType.Close);
            SendPacket();

            base.Close();
        }

        public override void Connect(string address, int port)
        {
            base.Connect("192.168.0.15", port);
            if ( _socket != null && _socket.Connected )
                return;

            base.Connect(address, port);

            //BeginPacket(HPacketType.DebugInfo);
            //_outgoingBW.Write(RemoteDebugInfo);
            //SendPacket();
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();
            _packetHandlers[(ushort)HPacketType.GameData_Request] = GameData_Request_Handler;
            _packetHandlers[(ushort)HPacketType.GameData_Response] = GameData_Response_Handler;
            _packetHandlers[(ushort)HPacketType.Puzzle_Complete] = Puzzle_Complete_Handler;
            _packetHandlers[(ushort)HPacketType.SpendCoins] = SpendCoins_Handler;
            _packetHandlers[(ushort)HPacketType.TowerData_Request] = TowerData_Request_Handler;
            _packetHandlers[(ushort)HPacketType.TowerData_Response] = TowerData_Response_Handler;
            _packetHandlers[(ushort)HPacketType.TutorialData] = TutorialData_Handler;
            _packetHandlers[(ushort)HPacketType.CoinBalance_Request] = CoinBalance_Request_Handler;
            _packetHandlers[(ushort)HPacketType.Close] = Close_Handler;
            _packetHandlers[(ushort)HPacketType.Products_Request] = Products_Request_Handler;
            _packetHandlers[(ushort)HPacketType.Products_Response] = Products_Response_Handler;
            _packetHandlers[(ushort)HPacketType.Account_Id] = Account_Id_Handler;

            _packetHandlers[(ushort)HPacketType.ValidateGameInfo_Request] = ValidateGameInfo_Request_Handler;
            _packetHandlers[(ushort)HPacketType.ValidateGameInfo_Response] = ValidateGameInfo_Response_Handler;
            
            //_packetHandlers[(ushort)HPacketType.DebugInfo] = DebugInfo_Handler;
        }

        void BeginPacket(HPacketType type)
        {
            LogInterface.Log(string.Format("BeginPacket({0})", type), LogInterface.LogMessageType.Debug);
            BeginPacket((ushort)type);
        }

        protected override void WriteVipData(int vipPoints)
        {
            VipDataArgs vip = OnBuildVipDataArgs.Invoke(vipPoints);
            vip.Write(_outgoingBW);
        }

        protected override void ReadVipData(BinaryReader br)
        {
            base.ReadVipData(br);
            VipInfo = new VipDataArgs();
            VipInfo.Read(br);
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
            for (int i = 0; i < gameData.TowerFloors.Length; i++)
                _outgoingBW.Write(gameData.TowerFloors[i]);

            _outgoingBW.Write(gameData.Level);
            _outgoingBW.Write(gameData.Exp);
            _outgoingBW.Write(gameData.Tutorial);

            SendPacket();
        }

        public void PuzzleComplete(string authToken, int tower, int floor, float completionTime, bool noExpBonus)
        {
            BeginPacket(HPacketType.Puzzle_Complete);

            _outgoingBW.Write(authToken);
            _outgoingBW.Write(tower);
            _outgoingBW.Write(floor);
            _outgoingBW.Write(completionTime);
            _outgoingBW.Write(noExpBonus);

            SendPacket();
        }

        public void SpendCoins(string authToken, int coins, int spendOn)
        {
            HardCurrency -= coins;

            BeginPacket(HPacketType.SpendCoins);

            _outgoingBW.Write(authToken);
            _outgoingBW.Write(coins);
            _outgoingBW.Write(spendOn);

            SendPacket();
        }

        public void RequestCoinBalance(string authToken)
        {
            BeginPacket(HPacketType.CoinBalance_Request);

            _outgoingBW.Write(authToken);
            SendPacket();
        }

        public void RequestTowerData(int tower, bool shortList)
        {
            BeginPacket(HPacketType.TowerData_Request);

            _outgoingBW.Write(tower);
            _outgoingBW.Write(shortList);

            SendPacket();
        }

        public void RequestProducts()
        {
            BeginPacket(HPacketType.Products_Request);

            SendPacket();
        }

        public void SendTowerData(int tower, TowerFloorRecord[] floors)
        {
            BeginPacket(HPacketType.TowerData_Response);

            _outgoingBW.Write(tower);
            _outgoingBW.Write(floors.Length);
            foreach (TowerFloorRecord floor in floors)
            {
                _outgoingBW.Write(floor.Floor);
                _outgoingBW.Write(floor.BestTime);
                _outgoingBW.Write(floor.RankFriends);
                _outgoingBW.Write(floor.RankGlobal);
            }

            SendPacket();
        }

        public void SendTutorialData(ulong tutorialData, string authToken)
        {
            BeginPacket(HPacketType.TutorialData);

            _outgoingBW.Write(tutorialData);
            _outgoingBW.Write(authToken);
            SendPacket();
        }

        public void SendValidateGameInfoRequest(string authString, string hash)
        {
            BeginPacket(HPacketType.ValidateGameInfo_Request);

            _outgoingBW.Write(authString);
            _outgoingBW.Write(hash);
            SendPacket();
        }

        public void SendValidateGameInfoResponse(int accountId, int hardCurrency, int vip, bool hashMatches, GameInfo serverData)
        {
            BeginPacket(HPacketType.ValidateGameInfo_Response);

            _outgoingBW.Write(accountId);
            _outgoingBW.Write(hardCurrency);
            _outgoingBW.Write(vip);
            _outgoingBW.Write(hashMatches);
            if( !hashMatches )
                serverData.Save(_outgoingBW);

            SendPacket();
        }

        public void SendProducts(GlobalProduct[] products)
        {
            BeginPacket(HPacketType.Products_Response);

            _outgoingBW.Write(products.Length);
            foreach (GlobalProduct p in products)
            {
                _outgoingBW.Write(p.ProductId);
                _outgoingBW.Write(p.Coins);
                _outgoingBW.Write(p.USD);
                _outgoingBW.Write(p.VIP);
            }

            SendPacket();
        }

        public void SetAccountId(int accountId)
        {
            BeginPacket(HPacketType.Account_Id);

            _outgoingBW.Write(accountId);
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
            for (int i = 0; i < args.TowerFloors.Length; i++)
                args.TowerFloors[i] = br.ReadInt32();

            args.Level = br.ReadInt32();
            args.Exp = br.ReadInt32();
            args.Tutorial = br.ReadUInt64();

            OnGameDataResponse(this, args);
        }

        void Puzzle_Complete_Handler(BinaryReader br)
        {
            PuzzleCompleteArgs args = new PuzzleCompleteArgs();
            args.AuthToken = br.ReadString();
            args.TowerIndex = br.ReadInt32();
            args.FloorNumber = br.ReadInt32();
            args.CompletionTime = br.ReadSingle();
            args.NoExpBonus = br.ReadBoolean();
            OnPuzzleComplete(this, args);
        }

        void SpendCoins_Handler(BinaryReader br)
        {
            SpendCoinsArgs args = new SpendCoinsArgs();
            args.AuthToken = br.ReadString();
            args.Coins = br.ReadInt32();
            args.SpendOn = br.ReadInt32();

            HardCurrency -= args.Coins;

            OnSpendCoins(this, args);
        }

        void TowerData_Request_Handler(BinaryReader br)
        {
            TowerDataRequstArgs args = new TowerDataRequstArgs();
            args.Tower = br.ReadInt32();
            args.ShortList = br.ReadBoolean();
            OnTowerDataRequest(this, args);
        }

        void TowerData_Response_Handler(BinaryReader br)
        {
            TowerData td = new TowerData();
            td.Tower = br.ReadInt32();
            int floors = br.ReadInt32();
            td.Floors = new TowerFloorRecord[floors];
            for (int i = 0; i < floors; i++)
            {
                TowerFloorRecord record = new TowerFloorRecord();
                record.Floor = br.ReadInt32();
                record.BestTime = br.ReadInt32();
                record.RankFriends = br.ReadInt32();
                record.RankGlobal = br.ReadInt32();
                td.Floors[i] = record;
            }

            OnTowerDataResponse(this, td);
        }

        void TutorialData_Handler(BinaryReader br)
        {
            ulong tutorialData = br.ReadUInt64();
            string authToken = br.ReadString();
            OnTutorialData(this, tutorialData, authToken);
        }

        void CoinBalance_Request_Handler(BinaryReader br)
        {
            string authToken = br.ReadString();
            OnCoinBalanceRequest(this, authToken);
        }

        void Close_Handler(BinaryReader br)
        {
            base.Close();
        }

        void Products_Request_Handler(BinaryReader br)
        {
            OnProductsRequest(this);
        }

        void Products_Response_Handler(BinaryReader br)
        {
            List<GlobalProduct> products = new List<GlobalProduct>();
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                products.Add(new GlobalProduct { ProductId = br.ReadInt32(), Coins = br.ReadInt32(), USD = br.ReadSingle(), VIP = br.ReadInt32() });
            }
            OnProductsResponse(products.ToArray());
        }

        void Account_Id_Handler(BinaryReader br)
        {
            AccountId = br.ReadInt32();
        }

        void DebugInfo_Handler(BinaryReader br)
        {
            //RemoteDebugInfo = br.ReadString();
        }

        void ValidateGameInfo_Request_Handler(BinaryReader br)
        {
            string authString = br.ReadString();
            string hash = br.ReadString();

            OnValidateGameInfo(this, authString, hash);
        }

        void ValidateGameInfo_Response_Handler(BinaryReader br)
        {
            AccountId = br.ReadInt32();
            HardCurrency = br.ReadInt32();
            Vip = br.ReadInt32();
            bool hashMatches = br.ReadBoolean();
            GameInfo serverData = null;
            if (!hashMatches)
            {
                serverData = new GameInfo();
                serverData.Load(br);
            }

            OnGameInfoResponse(this, serverData, AccountId);
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
        public ulong Tutorial;
    }

    public class PuzzleCompleteArgs : EventArgs
    {
        public string AuthToken;
        public int TowerIndex;
        public int FloorNumber;
        public float CompletionTime;
        public bool NoExpBonus;
    }

    public class VipDataArgs : EventArgs
    {
        public int Level;
        public int Points;
        public int Hints;
        public int MegaHints;
        public int UndoSize;
        public float ExpBonus;

        public void Write(BinaryWriter bw)
        {
            bw.Write(Level);
            bw.Write(Points);
            bw.Write(Hints);
            bw.Write(MegaHints);
            bw.Write(UndoSize);
            bw.Write(ExpBonus);
        }

        public void Read(BinaryReader br, int version = 2)
        {
            Level = br.ReadInt32();
            Points = br.ReadInt32();
            Hints = br.ReadInt32();
            MegaHints = br.ReadInt32();
            UndoSize = br.ReadInt32();

            if (version >= 2)
                ExpBonus = br.ReadSingle();
            else
                ExpBonus = 1.0f;
        }
    }

    public class SpendCoinsArgs : EventArgs
    {
        public string AuthToken;
        public int Coins;
        public int SpendOn;
    }

    public class TowerDataRequstArgs : EventArgs
    {
        public int Tower;
        public bool ShortList;
    }

    public class TowerFloorRecord
    {
        public int Floor;
        public int BestTime;
        public int RankFriends;
        public int RankGlobal;
    }

    public class TowerData : EventArgs
    {
        public int Tower;
        public TowerFloorRecord[] Floors;
    }
}
