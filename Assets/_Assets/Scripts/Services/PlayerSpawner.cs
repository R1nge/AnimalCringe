using _Assets.Scripts.Services.Factories;
using _Assets.Scripts.Services.Lobbies;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private Transform[] spawnPositions;
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
            var i = 0;
            foreach (var pair in _lobby.LobbyData)
            {
                
                NetworkObject player = _playerFactory.CreatePlayer(pair.Key);
                player.transform.position = spawnPositions[i].position;
                i++;
            }
        }
    }
}