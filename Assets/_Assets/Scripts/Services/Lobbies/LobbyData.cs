namespace _Assets.Scripts.Services.Lobbies
{
    public struct LobbyData
    {
        public readonly ulong ClientId;
        public readonly int SelectedSkin;

        public LobbyData(ulong clientId, int selectedSkin)
        {
            ClientId = clientId;
            SelectedSkin = selectedSkin;
        }
    }
}