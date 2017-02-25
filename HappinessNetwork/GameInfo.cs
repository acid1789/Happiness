using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Cryptography;

namespace HappinessNetwork
{
    public class GameInfo
    {
        public const int GameInfoVersion = 1;

        byte[] _hash;
        string _authString;
        GameDataArgs _gameData;
        TowerData[] _towerData;


        public GameInfo()
        {
        }        

        public void Load(BinaryReader br, int version = GameInfoVersion)
        {
            int length = br.ReadInt32();
            byte[] ascii = br.ReadBytes(length);
            _authString = Encoding.ASCII.GetString(ascii);
            LoadGameData(br, version);
            LoadTowerData(br, version);
        }

        void LoadGameData(BinaryReader br, int version)
        {
            _gameData = new GameDataArgs();
            
            int floorCount = br.ReadInt32();
            if (floorCount > 0)
            {
                _gameData.TowerFloors = new int[floorCount];
                for( int i = 0; i < floorCount; i++ )
                    _gameData.TowerFloors[i] = br.ReadInt32();
            }
            _gameData.Level = br.ReadInt32();
            _gameData.Exp = br.ReadInt32();
            _gameData.Tutorial = br.ReadUInt64();
        }

        public void Save(BinaryWriter bw)
        {
            byte[] authString = Encoding.ASCII.GetBytes(_authString);
            bw.Write(authString.Length);
            bw.Write(authString);
            SaveGameData(bw);
            SaveTowerData(bw);
        }

        void SaveGameData(BinaryWriter bw)
        {
            bw.Write(_gameData.TowerFloors.Length);
            foreach( int towerFloor in _gameData.TowerFloors )
                bw.Write(towerFloor);
            bw.Write(_gameData.Level);
            bw.Write(_gameData.Exp);
            bw.Write(_gameData.Tutorial);
        }

        void LoadTowerData(BinaryReader br, int version)
        {
            int towerCount = br.ReadInt32();
            _towerData = new TowerData[towerCount];
            for (int i = 0; i < towerCount; i++)
            {
                _towerData[i] = new TowerData();
                _towerData[i].Tower = br.ReadInt32();

                int floorCount = br.ReadInt32();
                _towerData[i].Floors = new TowerFloorRecord[floorCount];
                for (int j = 0; j < floorCount; j++)
                {
                    TowerFloorRecord tfr = new TowerFloorRecord();
                    tfr.Floor = br.ReadInt32();
                    tfr.BestTime = br.ReadInt32();
                    _towerData[i].Floors[j] = tfr;
                }
            }
        }

        void SaveTowerData(BinaryWriter bw)
        {
            if (_towerData != null)
            {
                bw.Write(_towerData.Length);
                foreach (TowerData td in _towerData)
                {
                    bw.Write(td.Tower);
                    bw.Write(td.Floors == null ? 0 : td.Floors.Length);
                    if (td.Floors != null)
                    {
                        foreach (TowerFloorRecord tfr in td.Floors)
                        {
                            bw.Write(tfr.Floor);
                            bw.Write(tfr.BestTime);
                        }
                    }
                }
            }
        }

        public byte[] GenerateHash()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            SaveGameData(bw);
            SaveTowerData(bw);

            MD5 md5 = MD5.Create();
            _hash = md5.ComputeHash(ms.GetBuffer());

            bw.Close();
            return _hash;
        }

        

        public GameDataArgs GameData
        {
            get { return _gameData; }
            set { _gameData = value; }
        }

        public TowerData[] TowerData
        {
            get { return _towerData; }
            set { _towerData = value; }
        }

        public string AuthString { get { return _authString; } set { _authString = value; } }
        public byte[] Hash { get { return _hash; } }
    }
}
