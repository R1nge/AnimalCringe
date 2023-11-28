using Unity.Netcode;
using VContainer;

namespace _Assets.Scripts.Services.Lobbies
{
    public class LobbyNetwork : NetworkBehaviour
    {
        private Lobby _lobby;

        [Inject]
        private void Inject(Lobby lobby) => _lobby = lobby;

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _lobby.AddPlayer(NetworkManager.Singleton.LocalClientId);
            }
        }

        private void ClientConnected(ulong clientId) => _lobby.AddPlayer(clientId);

        private void ClientDisconnected(ulong clientId) => _lobby.RemovePlayer(clientId);
    }
}