using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Windows.IoT.ServerService
{
    public static class ServerSocketConnection
    {
        //exposed to hook into server status changes
        public static event EventHandler<ConnectionStatusChangedEventArgs> ConnectStatusChanged;

        //exposed to hook into new messages
        public static event EventHandler<MessageSentEventArgs> NewMessageReady;

        private const string Port = "8027";

        private static StreamSocketListener _socketListener;
        private static StreamSocket _streamSocket;
        private static ConnectionStatus _connectionStatus;
        private static string _host;
        private static DataWriter _writer;

        public static ConnectionStatus ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                if (value != _connectionStatus)
                {
                    _connectionStatus = value;
                    var args = new ConnectionStatusChangedEventArgs { Status = _connectionStatus, Host = _host };
                    var handler = ConnectStatusChanged;
                    handler?.Invoke(ConnectionStatus, args);
                }
            }
        }

        private static string _message;

        public static string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    //System.Diagnostics.Debug.WriteLine("Robot Received: " + _message);
                    _message = value;
                    var args = new MessageSentEventArgs { Message = _message };
                    var handler = NewMessageReady;
                    handler?.Invoke(Message, args);
                }
            }
        }

        public static async void StartListener()
        {
            try
            {
                ConnectionStatus = ConnectionStatus.Connecting;
                _socketListener = new StreamSocketListener();
                _socketListener.ConnectionReceived += OnConnection;
                await _socketListener.BindServiceNameAsync(Port);
                ConnectionStatus = ConnectionStatus.Listening;
            }
            catch (Exception e)
            {
                ConnectionStatus = ConnectionStatus.Failed;
            }
        }

        private static async void OnConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            _streamSocket = args.Socket;
            _host = _streamSocket.Information.RemoteAddress.ToString();
            ConnectionStatus = ConnectionStatus.Connected;
            DataReader reader = new DataReader(_streamSocket.InputStream);
            _writer = new DataWriter(_streamSocket.OutputStream);

            try
            {
                while (true)
                {
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        return;
                    }

                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        return;
                    }

                    Message = reader.ReadString(actualStringLength);

                }
            }
            catch (Exception e)
            {
                ConnectionStatus = ConnectionStatus.Failed;
                //TODO:send a connection status message with error, then try to reconnect
            }
        }

        public static async void SendData(string message)
        {
            _writer.WriteUInt32(_writer.MeasureString(message));
            _writer.WriteString(message);
            await _writer.StoreAsync();
            //await _writer.FlushAsync();
        }
    }

    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatus Status;
        public string Host;
    }

    public class MessageSentEventArgs : EventArgs
    {
        public string Message;
    }

    public class SendDataEventArgs : EventArgs
    {
        public string Message;
    }

    public enum ConnectionStatus
    {
        Idle = 0,
        Connecting = 1,
        Listening = 2,
        Connected = 3,
        Failed = 99
    }
}
