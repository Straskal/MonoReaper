namespace Adventure
{
    public enum MessageType : byte
    {
        WelcomeMessage,
        ProfileMessage,
        PlayerDisconnectedMessage,
        StateChangedMessage,
        StateMessage,
        SnapshotMessage,
        EntityMessage
    }

    public enum EntityMessageType : byte
    {
        InputMessage
    }

    public enum EntityType : byte
    {
        Player,
        Tile,
        Tilemap,
        SpawnPosition
    }
}
