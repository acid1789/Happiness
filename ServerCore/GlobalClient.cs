﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using NetworkCore;

namespace ServerCore
{
    public class GlobalClient : Connection
    {
        public enum GPacketType
        {
            AccountInfoRequest = 50000,
            AccountInfoResponse,
            SpendCoins,
            CurrencyUpdate,
            AuthStringRequest,
            GlobalProductInfo,
        }

        public event EventHandler<AccountInfoRequestArgs> OnAccountInfoRequest;
        public event Action<AccountInfoResponseArgs> OnAccountInfoResponse;
        public event EventHandler<GlobalSpendCoinArgs> OnSpendCoins;
        public event EventHandler<CurrencyUpdateArgs> OnCurrencyUpdate;
        public event Action<GlobalClient, string, uint> OnAuthStringRequest;
        public event Action<GlobalProduct[]> OnGlobalProductInfo;

        public GlobalClient() : base (null)
        {
        }

        public GlobalClient(Socket s)
            : base(s)
        {
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();

            _packetHandlers[(ushort)GPacketType.AccountInfoRequest] = AccountInfoRequestHandler;
            _packetHandlers[(ushort)GPacketType.AccountInfoResponse] = AccountInfoResponseHandler;
            _packetHandlers[(ushort)GPacketType.SpendCoins] = SpendCoinsHandler;
            _packetHandlers[(ushort)GPacketType.CurrencyUpdate] = CurrencyUpdteHandler;
            _packetHandlers[(ushort)GPacketType.AuthStringRequest] = AuthStringRequestHandler;
            _packetHandlers[(ushort)GPacketType.GlobalProductInfo] = GlobalProductInfoHandler;
        }

        void BeginPacket(GPacketType gt)
        {
            LogThread.Log(string.Format("BeginPacket({0})", gt), LogThread.LogMessageType.Debug);
            BeginPacket((ushort)gt);
        }

        #region Packet Construction
        public void RequestAccountInfo(uint clientKey, string email, string password, string displayName, int oauthMode)
        {
            BeginPacket(GPacketType.AccountInfoRequest);

            _outgoingBW.Write(clientKey);
            WriteUTF8String(email);
            WriteUTF8String(password);
            WriteUTF8String(displayName);
            _outgoingBW.Write(oauthMode);

            SendPacket();
        }

        public void SendAccountInfo(uint clientKey, int accountId, string displayName, int hardCurrency, int vip, string authString)
        {
            BeginPacket(GPacketType.AccountInfoResponse);

            _outgoingBW.Write(clientKey);
            _outgoingBW.Write(accountId);
            _outgoingBW.Write(hardCurrency);
            _outgoingBW.Write(vip);
            WriteUTF8String(displayName);
            WriteUTF8String(authString);

            SendPacket();
        }

        public void SpendCoins(int accountId, int amount, ulong serverRecord)
        {
            BeginPacket(GPacketType.SpendCoins);

            _outgoingBW.Write(accountId);
            _outgoingBW.Write(amount);
            _outgoingBW.Write(serverRecord);

            SendPacket();
        }

        public void HardCurrencyUpdate(int accountId, int currency, int vip)
        {
            BeginPacket(GPacketType.CurrencyUpdate);

            _outgoingBW.Write(accountId);
            _outgoingBW.Write(currency);
            _outgoingBW.Write(vip);

            SendPacket();
        }

        public void RequestAccountInfoFromAuthString(string authString, uint clientKey)
        {
            BeginPacket(GPacketType.AuthStringRequest);

            _outgoingBW.Write(authString);
            _outgoingBW.Write(clientKey);

            SendPacket();
        }

        public void SendProducts(GlobalProduct[] products)
        {
            BeginPacket(GPacketType.GlobalProductInfo);

            _outgoingBW.Write(products.Length);
            foreach (GlobalProduct prod in products)
            {
                _outgoingBW.Write(prod.ProductId);
                _outgoingBW.Write(prod.Coins);
                _outgoingBW.Write(prod.USD);
                _outgoingBW.Write(prod.VIP);
            }
            SendPacket();
        }
        #endregion

        #region Packet Handlers
        void AccountInfoRequestHandler(BinaryReader br)
        {
            AccountInfoRequestArgs args = new AccountInfoRequestArgs();
            args.ClientKey = br.ReadUInt32();
            args.Email = ReadUTF8String(br);
            args.Password = ReadUTF8String(br);
            args.DisplayName = ReadUTF8String(br);
            args.OAuthMode = br.ReadInt32();
            OnAccountInfoRequest(this, args);
        }

        void AccountInfoResponseHandler(BinaryReader br)
        {
            AccountInfoResponseArgs args = new AccountInfoResponseArgs();
            args.ClientKey = br.ReadUInt32();
            args.AccountId = br.ReadInt32();
            args.HardCurrency = br.ReadInt32();
            args.Vip = br.ReadInt32();
            args.DisplayName = ReadUTF8String(br);
            args.AuthString = ReadUTF8String(br);
            OnAccountInfoResponse(args);
        }

        void SpendCoinsHandler(BinaryReader br)
        {
            GlobalSpendCoinArgs args = new GlobalSpendCoinArgs();
            args.AccountId = br.ReadInt32();
            args.Amount = br.ReadInt32();
            args.ServerRecord = br.ReadUInt64();
            args.VIP = 0;
            OnSpendCoins(this, args);
        }

        void CurrencyUpdteHandler(BinaryReader br)
        {
            CurrencyUpdateArgs args = new CurrencyUpdateArgs();
            args.AccountId = br.ReadInt32();
            args.NewCurrency = br.ReadInt32();
            args.NewVIP = br.ReadInt32();

            OnCurrencyUpdate(this, args);
        }

        void AuthStringRequestHandler(BinaryReader br)
        {
            string authString = br.ReadString();
            uint clientKey = br.ReadUInt32();
            OnAuthStringRequest(this, authString, clientKey);
        }

        void GlobalProductInfoHandler(BinaryReader br)
        {
            int count = br.ReadInt32();
            if (count > 0)
            {
                List<GlobalProduct> products = new List<GlobalProduct>();
                for (int i = 0; i < count; i++)
                {
                    products.Add(new GlobalProduct { ProductId = br.ReadInt32(), Coins = br.ReadInt32(), USD = br.ReadSingle(), VIP = br.ReadInt32() });
                }
                OnGlobalProductInfo(products.ToArray());
            }
        }
        #endregion
    }

    #region Args Classes
    public class AccountInfoRequestArgs : EventArgs
    {
        public uint ClientKey;
        public string Email;
        public string Password;
        public string DisplayName;
        public int OAuthMode;
    }

    public class AccountInfoResponseArgs : EventArgs
    {
        public uint ClientKey;
        public int AccountId;
        public int HardCurrency;
        public int Vip;
        public string DisplayName;
        public string AuthString;
    }

    public class GlobalSpendCoinArgs : EventArgs
    {
        public int AccountId;
        public int Amount;
        public ulong ServerRecord;
        public int VIP;
    }

    public class CurrencyUpdateArgs : EventArgs
    {
        public int AccountId;
        public int NewCurrency;
        public int NewVIP;
    }
    #endregion
}
