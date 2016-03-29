using System.Collections.Generic;

namespace PocketProxy.PE
{
    public class ServerInfo
    {
        public int MaxPlayers { get; }
        public int OnlinePlayers { get; }
        public string MOTD { get; }

        internal ServerInfo(int max, int now, string motd)
        {
            MaxPlayers = max;
            OnlinePlayers = now;
            MOTD = motd;
        }
    }

    public class Gs4ServerInfo
    {
        public int MaxPlayers { get; }
        public int OnlinePlayers { get; }
        public string MOTD { get; }
        public string[] Players { get; } 
        public string[] Plugins { get; }
        public IReadOnlyDictionary<string, string> Values { get; }  

        internal Gs4ServerInfo(int max, int now, string motd, string[] players, string[] plugins, IReadOnlyDictionary<string,string> values)
        {
            MaxPlayers = max;
            OnlinePlayers = now;
            MOTD = motd;
            Players = players;
            Plugins = plugins;
            Values = values;
        }
    }
}
