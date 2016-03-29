namespace PocketProxy.PC.Objects
{
    public class StatusResponsePlayers
    {
        public StatusResponsePlayers(int _max, int _online)
        {
            max = _max;
            online = _online;
        }

        public int max { get; set; }
        public int online { get; set; }
    }
}
