namespace Adventure.Networking
{
    public enum MessageType : byte
    {
        ProfileMessage,
        WelcomeMessage,
        JoinAnotherWorldRequest,
        PlayerDisconnected,
        ClientStateReadyRequest,
        StateRequest,
        EntityRequest,
        JoinedWorldEvent,
        StateChangedMessage,
        StateMessage,
        EntityMessage,
        SnapshotMessage,
    }

    public enum EntityMessageType 
    {
        InputRequest
    }

    public enum EntityType 
    {
        Player,
        Tile,
        Tilemap
    }
}
