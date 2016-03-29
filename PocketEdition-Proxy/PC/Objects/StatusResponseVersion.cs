namespace PocketProxy.PC.Objects
{
    public class StatusResponseVersion
    {
        public StatusResponseVersion(string _name, int _protocol)
        {
            name = _name;
            protocol = _protocol;
        }

        public string name { get; set; }
        public int protocol { get; set; }
    }
}
