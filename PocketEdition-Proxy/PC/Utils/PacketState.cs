namespace PocketProxy.PC.Utils
{
    public enum PacketState
    {
        Handshake = 0,
        Status = 1,
        Login = 2,
        Play = 3,
        Disconnected = -1
    }
}
