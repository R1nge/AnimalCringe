using _Assets.Scripts.Services.Factories;
using _Assets.Scripts.Services.Lobbies;
using Unity.Netcode;
using VContainer;

namespace _Assets.Scripts.Services
{
    public class PlayerSpawner : NetworkBehaviour
    {
        private Lobby _lobby;
        private PlayerFactory _playerFactory;

        [Inject]
        private void Inject(Lobby lobby, PlayerFactory playerFactory)
        {
            _lobby = lobby;
            _playerFactory = playerFactory;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayersServerRpc()
        {
            foreach (var pair in _lobby.LobbyData)
            {
                _playerFactory.CreatePlayer(pair.Key);
            }
        }
    }
}