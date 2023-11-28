using _Assets.Scripts.Services.Skins;
using Unity.Netcode;
using VContainer;

namespace _Assets.Scripts.Services.Lobbies
{
    public class LobbyNetwork : NetworkBehaviour
    {
        private Lobby _lobby;
        private SkinService _skinService;

        [Inject]
        private void Inject(Lobby lobby, SkinService skinService)
        {
            _lobby = lobby;
            _skinService = skinService;
        }

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _lobby.AddPlayer(NetworkManager.Singleton.LocalClientId, _skinService.SelectedSkinIndex);
            }
        }

        private void ClientConnected(ulong clientId)
        {
            if (!IsServer)
            {
                int index = _skinService.SelectedSkinIndex;
                ClientConnectedServerRpc(clientId, index);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClientConnectedServerRpc(ulong clientId, int skinIndex) => _lobby.AddPlayer(clientId, skinIndex);

        private void ClientDisconnected(ulong clientId) => _lobby.RemovePlayer(clientId);
    }
}