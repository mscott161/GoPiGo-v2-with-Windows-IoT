using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Windows.IoT.ClientService
{
    public static class ClientSocketConnection
    {
        //exposed to hook into server status changes
        public static event EventHandler<ConnectionStatusChangedEventArgs> ConnectStatusChanged;
        private static ConnectionStatus _status = ConnectionStatus.Idle;

        public static ConnectionStatus Status
        {
            get { return _status; }
            set
            {
                if (value != _status)
                {
                    _status = value;
                    var args = new ConnectionStatusChangedEventArgs { Status = _status };
                    var handler = ConnectStatusChanged;
                    handler?.Invoke(Status, args);
                }
            }
        }

        private static HostName _hostName;
        private static StreamSocket _streamSocket2;
        private static DataReader _reader2;
        private static DataWriter _writer2;

        public static async void Connect(string serverIP, string serverPort)
        {
            try
            {
                Status = ConnectionStatus.Connecting;
                _hostName = new HostName(serverIP);
                _streamSocket2 = new StreamSocket();
                await _streamSocket2.ConnectAsync(_hostName, serverPort);
                Status = ConnectionStatus.Connected;
                _writer2 = new DataWriter(_streamSocket2.OutputStream);
                _reader2 = new DataReader(_streamSocket2.InputStream);

                GetData();
            }
            catch (Exception e)
            {
                Status = ConnectionStatus.Failed;
                //todo:report errors via event to be consumed by UI thread
            }
        }

        public static async void SendMessage(string message)
        {
            try
            {
                _writer2.WriteUInt32(_writer2.MeasureString(message));
                _writer2.WriteString(message);
                await _writer2.StoreAsync();
                //await _writer2.FlushAsync();
            }
            catch (Exception exc)
            {
                Status = ConnectionStatus.Failed;
            }
        }

        public static async void GetData()
        {
            try
            {
                while (true)
                {
                    uint sizeFieldCount = await _reader2.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        return;
                    }

                    uint stringLength = _reader2.ReadUInt32();
                    uint actualStringLength = await _reader2.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        return;
                    }

                    Message = _reader2.ReadString(actualStringLength);

                }
            }
            catch (Exception e)
            {
                Status = ConnectionStatus.Failed;
                //TODO:send a connection status message with error, then try to reconnect
            }
        }

        public static event EventHandler<MessageSentEventArgs> NewMessageReady;
        private static string _message;

        public static string Message
        {
            get { return _message; }
            set
            {
                    //System.Diagnostics.Debug.WriteLine("Robot Received: " + _message);
                    _message = value;
                    var args = new MessageSentEventArgs { Message = _message };
                    var handler = NewMessageReady;
                    handler?.Invoke(Message, args);
            }
        }
    }

    public class ConnectionStatusChangedEventArgs : EventArgs
    {
        public ConnectionStatus Status;
    }

    public class MessageSentEventArgs : EventArgs
    {
        public string Message;
    }

    public enum ConnectionStatus
    {
        Idle = 0,
        Connecting,
        Listening,
        Connected,
        Failed = 99
    }
}
