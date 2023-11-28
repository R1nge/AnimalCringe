namespace _Assets.Scripts.Services.Lobbies
{
    public struct LobbyData
    {
        public ulong ClientId;

        public LobbyData(ulong clientId)
        {
            ClientId = clientId;
        }
    }
}