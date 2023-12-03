using _Assets.Scripts.Misc;

namespace _Assets.Scripts.Services.Lobbies
{
    public struct LobbyPlayerData
    {
        public readonly ulong ClientId;
        public readonly int SelectedSkin;
        public NetworkString Nickname;

        public LobbyPlayerData(ulong clientId, int selectedSkin, NetworkString nickname)
        {
            ClientId = clientId;
            SelectedSkin = selectedSkin;
            Nickname = nickname;
        }
    }
}