using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Mcpc.Lib;
using PocketProxy.Network;
using PocketProxy.PE;
using PocketProxy.Utils;
using Timer = System.Timers.Timer;

namespace PocketProxy
{
    public class PocketProxy
    {
        public const string ProtocolName = "PC 1.9.1-Pre3 | PE 0.14";

        private IPEndPoint ServerEndPoint { get; }
        private string IpAddress { get; }
        private int Port { get; }

        private TcpListener Listener { get; set; }
        private ConcurrentDictionary<IPEndPoint, PocketClient> Clients { get; }
        private Timer TickTimer { get; }

        public PocketProxy(string ip, int listeningPort, IPEndPoint serverEndPoint)
        {
            if (ip == "") ip = "0.0.0.0";
            IpAddress = ip;
            Port = listeningPort;

            ServerEndPoint = serverEndPoint;
            Clients = new ConcurrentDictionary<IPEndPoint, PocketClient>();

            Tick = 0;
            TickTimer = new Timer(50); //1000 / 20 = 50
            TickTimer.Elapsed += TickTimer_Elapsed;

            UpdateTitle();
        }

        private long Tick { get; set; }
        private readonly object _tickSync = new object();

        private void TickTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!Monitor.TryEnter(_tickSync)) return;

            Tick++;
            var checkDisconnect = (Tick%50 == 0);

            try
            {
                foreach (var client in Clients.CloneDictionairy())
                {
                    client.Value.OnTick(Tick);

                    if (!checkDisconnect || client.Value.IsConnected()) continue;

                    PocketClient c;
                    Clients.TryRemove(client.Key, out c);
                    client.Value.KillProxy();
                }
            }
            finally
            {
                Monitor.Exit(_tickSync);
            }

            if (Tick % 200 == 0) //Update every 10 seconds (10 * 20 = 200)
            {
                UpdateTitle();
            }
        }

        private void UpdateTitle()
        {
            int online = 0;
            int max = 0;
            string status = "Offline";

            var serverInfo = ServerList.QueryServer(ServerEndPoint);
            if (serverInfo != null)
            {
                online = serverInfo.OnlinePlayers;
                max = serverInfo.MaxPlayers;
                status = "Online";
            }
            Console.Title = string.Format("PocketProxy (#{0}) | Connections: {1} | Target: {4} |  Players: {2}/{3}", Info.ProtocolVersion, Clients.Count, online, max, status);
        }

        public void Start()
        {
            try
            {
                Listener?.Stop();
                Listener = new TcpListener(IPAddress.Parse(IpAddress), Port);
                Listener.Start();
                TickTimer.Start();

                Listener.BeginAcceptTcpClient(Callback, null);
            }
            catch (Exception)
            {
                //Could not start
            }
        }

        public void Stop()
        {
            TickTimer.Stop();
            foreach (var i in Clients.ToArray())
            {
                i.Value.KillProxy();
            }

            Thread.Sleep(500); //Wait...
            Listener.Stop();
        }

        private void Callback(IAsyncResult asyncResult)
        {
            try
            {
                var tcpClient = Listener.EndAcceptTcpClient(asyncResult);
                Clients.TryAdd((IPEndPoint) tcpClient.Client.RemoteEndPoint, new PocketClient(tcpClient, ServerEndPoint));
                Listener.BeginAcceptTcpClient(Callback, null);
            }
            catch (Exception)
            {
                //Could not do callback
            }
        }
    }
}