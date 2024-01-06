namespace Messages
{
    public enum MessageType : ushort
    {
        Test = 0,
        
        DirectionInput = 1,
        Position = 2,
        ReconciliationSync = 3,
        CharactersSync = 4,
        RemoveClient = 5,
    }
}