using System;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace PocketProxy.PE.Net
{
    public class McpeClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(McpeClient));

        private IPEndPoint LocalEndpoint { get; set; }
        private string Username { get; set; }
        private string Hostname { get; set; }
        private int Port { get; set; }
        private UdpClient UdpClient { get; set; }
        private int Mtu { get; set; }
        private long DatagramSequenceNumber { get; set; }

        public long ClientId { get; private set; }
        public McpeClient(string hostname, int port, string username)
        {
            Hostname = hostname;
            Port = port;
            Username = username;
            ClientId = new Random().NextLong();
            LocalEndpoint = new IPEndPoint(IPAddress.Any, 0);
        }

        public void Connect()
        {
            if (UdpClient != null) return;
            try
            {
                Log.InfoFormat("Trying to connect to {0}:{1}", Hostname, Port);
                UdpClient = new UdpClient(LocalEndpoint)
                {
                    Client =
                    {
                        ReceiveBufferSize = int.MaxValue,
                        SendBufferSize = int.MaxValue
                    }
                };
                UdpClient.Connect(Hostname, Port);

                var connectStart = DateTime.UtcNow;
                var connectEnd = DateTime.MinValue;
                var gotResponse = false;

                
            }
            catch
            {
                UdpClient = null;
            }
        }

        private void SendData(byte[] data)
        {
            if (UdpClient == null) return;

            try
            {
                UdpClient.SendAsync(data, data.Length);
            }
            catch (Exception e)
            {
                Log.Debug("Send exception", e);
            }
        }
    }
}
