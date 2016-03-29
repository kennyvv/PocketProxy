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
}
