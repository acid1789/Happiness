using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;

namespace NetworkCore
{
    public class GameClient : Connection
    {
        public enum GCPacketType
        {
            AccountRequest = 1000,
            AccountResponse,
            ChatChannels,
            ChatMessage,
            CurrencyUpdate,
        }

        public event EventHandler<AccountRequestArgs> OnAccountRequest;
        public event EventHandler OnAccountResponse;
        public event EventHandler OnChatChannels;
        public event EventHandler<ChatMessageArgs> OnChatMessage;
        public event EventHandler OnHardCurrencyUpdate;

        #region Account Info
        int _accountId;
        string _displayName;
        string _authString;
        int _hardCurrency;
        int _vip;
        #endregion

        #region Chat Info
        List<uint> _blockList;
        uint _chatChannels;
        #endregion

        object _pendingAuthTask;


        public GameClient()
            : base(null)
        {
        }

        public GameClient(Socket s)
            : base(s)
        {
            _sessionKey = (uint)DateTime.Now.Ticks;
            _blockList = new List<uint>();
        }

        protected override void RegisterPacketHandlers()
        {
            base.RegisterPacketHandlers();

            _packetHandlers[(ushort)GCPacketType.AccountRequest] = AccountRequestHandler;
            _packetHandlers[(ushort)GCPacketType.AccountResponse] = AccountResponseHandler;
            _packetHandlers[(ushort)GCPacketType.ChatChannels] = ChatChannelsHandler;
            _packetHandlers[(ushort)GCPacketType.ChatMessage] = ChatMessageHandler;
            _packetHandlers[(ushort)GCPacketType.CurrencyUpdate] = CurrencyUpdateHandler;
        }

        void BeginPacket(GCPacketType type)
        {
            LogInterface.Log(string.Format("BeginPacket({0})", type), LogInterface.LogMessageType.Debug);
            BeginPacket((ushort)type);
        }

        #region Misc Functions
        public bool IsInChannel(int channel)
        {
            uint mask = (uint)(1 << channel);
            return ((mask & _chatChannels) == mask);
        }

        public bool IsChatBlocked(int sender)
        {
            int index = _blockList.IndexOf((uint)sender);
            return (index >= 0 );
        }
        #endregion

        #region Packet Construction        
        public void SendAccountRequest(string email, string pass, string displayName = null)
        {
            BeginPacket(GCPacketType.AccountRequest);

            WriteUTF8String(email);
            WriteUTF8String(pass);
            WriteUTF8String(displayName);

            SendPacket();
        }

        public void SendAccountResponse(int accountId, string displayName, string authString = null)
        {
            BeginPacket(GCPacketType.AccountResponse);

            _outgoingBW.Write(_sessionKey);
            _outgoingBW.Write(accountId);
            WriteUTF8String(displayName);
            WriteUTF8String(authString);

            SendPacket();
        }

        public void SendChatChannels()
        {
            BeginPacket(GCPacketType.ChatChannels);

            _outgoingBW.Write(_chatChannels);
            SendPacket();
        }

        public void SendChat(int channel, string message, string sender)
        {
            BeginPacket(GCPacketType.ChatMessage);

            _outgoingBW.Write(channel);
            WriteUTF8String(message);
            WriteUTF8String(sender);
            
            SendPacket();
        }

        public void CurrencyUpdate(int newCurrency)
        {
            _hardCurrency = newCurrency;

            BeginPacket(GCPacketType.CurrencyUpdate);

            _outgoingBW.Write(newCurrency);

            SendPacket();
        }
        #endregion

        #region Packet Handlers
        void AccountRequestHandler(BinaryReader br)
        {
            string email = ReadUTF8String(br);
            string pass = ReadUTF8String(br);
            string displayName = ReadUTF8String(br);

            AccountRequestArgs args = new AccountRequestArgs();
            args.Email = email;
            args.Password = pass;
            args.DisplayName = displayName;
            OnAccountRequest(this, args);
        }

        void AccountResponseHandler(BinaryReader br)
        {
            _sessionKey = br.ReadUInt32();
            _accountId = br.ReadInt32();
            _displayName = ReadUTF8String(br);

            OnAccountResponse(this, null);
        }

        void ChatChannelsHandler(BinaryReader br)
        {
            _chatChannels = br.ReadUInt32();

            OnChatChannels(this, null);
        }

        void ChatMessageHandler(BinaryReader br)
        {
            int channel = br.ReadInt32();
            string message = ReadUTF8String(br);
            string sender = ReadUTF8String(br);

            OnChatMessage(this, new ChatMessageArgs(channel, message, sender));
        }

        void CurrencyUpdateHandler(BinaryReader br)
        {
            _hardCurrency = br.ReadInt32();
            if( OnHardCurrencyUpdate != null )
                OnHardCurrencyUpdate(this, null);
        }
        #endregion

        #region Accessors
        public int AccountId
        {
            get { return _accountId; }
            set { _accountId = value; }
        }

        public int HardCurrency
        {
            get { return _hardCurrency; }
            set { _hardCurrency = value; }
        }

        public int Vip
        {
            get { return _vip; }
            set { _vip = value; }
        }

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string AuthString
        {
            get { return _authString; }
            set { _authString = value; }
        }

        public List<uint> BlockList
        {
            get { return _blockList; }            
        }

        public uint ChatChannels
        {
            get { return _chatChannels; }
            set { _chatChannels = value; }
        }

        public object PendingAuthTask
        {
            get { return _pendingAuthTask; }
            set { _pendingAuthTask = value; }
        }
        #endregion
    }

    #region Args Classes
    public class AccountRequestArgs : EventArgs
    {
        public string Email;
        public string Password;
        public string DisplayName;
    }

    public class ChatMessageArgs : EventArgs
    {
        public int Channel;
        public string Message;
        public string Sender;

        public ChatMessageArgs(int channel, string message, string sender)
        {
            Channel = channel;
            Message = message;
            Sender = sender;
        }
    }
    #endregion
}
