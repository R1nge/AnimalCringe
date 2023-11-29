using _Assets.Scripts.Misc;

namespace _Assets.Scripts.Services.Lobbies
{
    public struct LobbyData
    {
        public readonly ulong ClientId;
        public readonly int SelectedSkin;
        public NetworkString Nickname;

        public LobbyData(ulong clientId, int selectedSkin, NetworkString nickname)
        {
            ClientId = clientId;
            SelectedSkin = selectedSkin;
            Nickname = nickname;
        }
    }
}