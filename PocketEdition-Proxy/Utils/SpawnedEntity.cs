namespace PocketProxy.Utils
{
    public struct SpawnedEntity
    {
        public long EntityId { get; set; }
        public int EntityTypeId { get; set; }
    }

    public struct SpawnedPlayer
    {
        public string Username { get; set; }
        public string UUID { get; set; }
        public int EntityId { get; set; }
    }
}
