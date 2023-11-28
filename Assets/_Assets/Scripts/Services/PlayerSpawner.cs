using _Assets.Scripts.Services.Lobbies;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject player;
        private Lobby _lobby;

        [Inject]
        private void Inject(Lobby lobby) => _lobby = lobby;

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayerServerRpc()
        {
            foreach (var pair in _lobby.LobbyData)
            {
                NetworkObject playerInstance = Instantiate(player);
                playerInstance.SpawnWithOwnership(pair.Value.ClientId);
            }
        }
    }
}