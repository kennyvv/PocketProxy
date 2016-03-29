using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PocketProxy.PE.Query
{
    public static class Gs4
    {
        public static Gs4ServerInfo QueryServer(IPEndPoint endpoint)
        {
            IPEndPoint recpoint = new IPEndPoint(IPAddress.Any, 0);
            UdpClient client = new UdpClient();
            client.Connect(endpoint);

            byte[] sendme;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.WriteByte(0xFE); // Magic
                ms.WriteByte(0xFD); // Magic
                ms.WriteByte(0x09); // Type
                ms.WriteByte(0x01); // Session
                ms.WriteByte(0x01); // Session
                ms.WriteByte(0x01); // Session
                ms.WriteByte(0x01); // Session

                sendme = ms.ToArray();
                client.Send(sendme, sendme.Length);
            }

            byte[] rec = client.Receive(ref recpoint);
            string number = rec.Where((t, i) => i > 4 && t != 0x00).Aggregate("", (current, t) => current + (char)t);

            using (var ms = new MemoryStream())
            {
                ms.WriteByte(0xFE); // Magic
                ms.WriteByte(0xFD); // Magic
                ms.WriteByte(0x00); // Type
                ms.WriteByte(0x01); // Session
                ms.WriteByte(0x01); // Session
                ms.WriteByte(0x01); // Session
                ms.WriteByte(0x01); // Session
                byte[] numberbytes = BitConverter.GetBytes(int.Parse(number)).Reverse().ToArray();
                ms.Write(numberbytes, 0, 4); // Challenge
                ms.WriteByte(0x00); // Padding
                ms.WriteByte(0x00); // Padding
                ms.WriteByte(0x00); // Padding
                ms.WriteByte(0x00); // Padding
                sendme = ms.ToArray();
                client.Send(sendme, sendme.Length);
            }
            rec = client.Receive(ref recpoint);

            var data = new byte[rec.Length - 16];
            Array.Copy(rec, 16, data, 0, data.Length);

            StringBuilder sb = new StringBuilder();
            Dictionary<string, string> values = new Dictionary<string, string>();
            List<string> players = new List<string>();

            bool nowPlayers = false;
            string key = "";
            using (var ms = new MemoryStream(data))
            {
                while (ms.Position != ms.Length)
                {
                    var bit = (char) ms.ReadByte();

                    if (nowPlayers)
                    {
                        if (bit != '\0')
                        {
                            sb.Append(bit);
                            continue;
                        }
                        var play = sb.ToString();
                        if (string.IsNullOrEmpty(play)) continue;
                        players.Add(play);
                        sb.Clear();
                        continue;
                    }

                    if (bit != '\0')
                    {
                        sb.Append(bit);
                        continue;
                    }

                    if (key == "")
                    {
                        key = sb.ToString();
                        if (key.Equals("\u0001player_"))
                        {
                            nowPlayers = true;
                            ms.Position++;
                        }
                        sb.Clear();
                        continue;
                    }

                    var val = sb.ToString();
                    values.Add(key, val);
                    key = "";
                    sb.Clear();
                }
            }

            int online = 0;
            int max = 0;
            int.TryParse(values["numplayers"], out online);
            int.TryParse(values["maxplayers"], out max);
            string hostname = values["hostname"];

            var serverEngine = "THISDOESNOTHAVEASERVERENGINE";

            if (values.ContainsKey("server_engine"))
            {
                serverEngine = values["server_engine"] + ":";
            }

            List<string> plugins = new List<string>();

            var rawPlugins = values["plugins"].Split(';');
            if (rawPlugins.Length > 1)
            {
                foreach (var i in rawPlugins)
                {
                    var plugin = i;
                    if (plugin.StartsWith(serverEngine))
                    {
                        plugin = plugin.Replace(serverEngine, "");
                    }
                    if (plugin[0] == ' ')
                    {
                        plugin = plugin.Substring(1);
                    }
                    plugins.Add(plugin);
                }
            }

            return new Gs4ServerInfo(max, online, hostname, players.ToArray(), plugins.ToArray(), values);
        }
    }
}
