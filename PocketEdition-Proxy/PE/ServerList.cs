using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PocketProxy.PE
{
    public static class ServerList
    {
        private static readonly byte[] OfflineMessageDataId =
        {
            0x00, 0xff, 0xff, 0x00, 0xfe, 0xfe, 0xfe, 0xfe, 0xfd, 0xfd, 0xfd, 0xfd, 0x12, 0x34, 0x56, 0x78
        };

        /// <summary>
        /// Query the server for the server's MOTD, Online players & Maximum players.
        /// </summary>
        /// <param name="serverEndPoint">The server to query</param>
        /// <returns></returns>
        public static ServerInfo QueryServer(IPEndPoint serverEndPoint)
        {
            IPEndPoint recpoint = new IPEndPoint(IPAddress.Any, 0);
            UdpClient client = new UdpClient
            {
                Client =
                {
                    ReceiveTimeout = 1000
                }
            };

            try
            {
                int maxPlayers = 0;
                int onlinePlayers = 0;
                string motd = "";

                client.Connect(serverEndPoint);

                using (var ms = new MemoryStream())
                {
                    ms.WriteByte(0x01);
                    byte[] pingId = BitConverter.GetBytes((long) 12).Reverse().ToArray();
                    ms.Write(pingId, 0, 8);
                    ms.Write(OfflineMessageDataId, 0, OfflineMessageDataId.Length);
                    var data = ms.ToArray();
                    client.Send(data, data.Length);
                }

                byte[] receivedData = client.Receive(ref recpoint);
                if (receivedData[0] == 0x1c)
                {
                    string raw;
                    using (var ms = new MemoryStream(receivedData))
                    {
                        ms.Position = 33;

                        byte[] shortsBytes = new byte[2];
                        ms.Read(shortsBytes, 0, shortsBytes.Length);
                        short stringLength = BitConverter.ToInt16(shortsBytes, 0);

                        byte[] stringBuffer = new byte[stringLength];
                        int length = ms.Read(stringBuffer, 0, stringBuffer.Length);
                        raw = Encoding.UTF8.GetString(stringBuffer, 0, length);
                    }
                    var splitData = raw.Split(';');
                    motd = splitData[1];
                    onlinePlayers = int.Parse(splitData[4]);
                    maxPlayers = int.Parse(splitData[5]);
                }
                client.Close();

                return new ServerInfo(maxPlayers, onlinePlayers, motd);
            }
            catch
            {
                client?.Close();
                return null;
            }
        }
    }
}
