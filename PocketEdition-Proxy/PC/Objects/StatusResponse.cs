namespace PocketProxy.PC.Objects
{
    public class StatusResponse
    {
        public StatusResponse(string versionName, int protocolVersion, int onlinePlayers, int maxPlayers,
            ChatObject motd)
        {
            version = new StatusResponseVersion(versionName, protocolVersion);
            players = new StatusResponsePlayers(maxPlayers, onlinePlayers);
            description = motd;
        }

        public StatusResponseVersion version { get; set; }
        public StatusResponsePlayers players { get; set; }
        public ChatObject description { get; set; }
    }
}
