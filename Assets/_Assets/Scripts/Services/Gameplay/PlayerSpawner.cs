using _Assets.Scripts.Damageables;
using _Assets.Scripts.Players;
using _Assets.Scripts.Services.Factories;
using _Assets.Scripts.Services.Lobbies;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services.Gameplay
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

        [ServerRpc(RequireOwnership = false)]
        public void RespawnServerRpc(ulong clientId)
        {
            if (NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId))
            {
                NetworkObject player = NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId);
                player.GetComponent<CharacterController>().enabled = false;

                int index = Random.Range(0, spawnPositions.Length);
                RespawnClientRpc(index, player);   
                
                player.GetComponent<CharacterController>().enabled = true;
                player.GetComponent<PlayerInput>().EnableServerRpc(true);
                player.GetComponent<Health>().Respawn();
            }
        }

        [ClientRpc]
        private void RespawnClientRpc(int index, NetworkObjectReference player)
        {
            if (player.TryGet(out NetworkObject playerNetworkObject))
            {
                playerNetworkObject.GetComponent<CharacterController>().enabled = false;
                playerNetworkObject.transform.position = spawnPositions[index].position;
                playerNetworkObject.GetComponent<CharacterController>().enabled = true;
            }
        }
    }
}