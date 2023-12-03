using System;
using _Assets.Scripts.Damageables;
using _Assets.Scripts.Players;
using _Assets.Scripts.Services.Skins;
using _Assets.Scripts.Weapons;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace _Assets.Scripts.Services.Lobbies
{
    public class LobbySpawner : NetworkBehaviour
    {
        private Lobby _lobby;
        private SkinService _skinService;

        [Inject]
        private void Inject(Lobby lobby, SkinService skinService)
        {
            _lobby = lobby;
            _skinService = skinService;
        }

        private void Awake() => _lobby.OnClientConnected += SpawnPlayerServerRpc;

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ulong clientId)
        {
            if (!_lobby.LobbyData.ContainsKey(clientId)) return;
            NetworkObject player = Instantiate(_skinService.GetSkinSo(_lobby.LobbyData[clientId].SelectedSkin).LobbySkin);
            player.SpawnAsPlayerObject(clientId);

            player.transform.position = RandomPosition();
        }

        [ServerRpc(RequireOwnership = false)]
        public void RespawnServerRpc(ulong clientId)
        {
            if (NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId))
            {
                NetworkObject player = NetworkManager.SpawnManager.GetPlayerNetworkObject(clientId);

                Vector3 position = RandomPosition();
                
                player.GetComponent<CharacterController>().enabled = false;
                player.transform.position = position;
                player.GetComponent<CharacterController>().enabled = true;
                RespawnClientRpc(position, player);
            }
            else
            {
                Debug.LogError("[SERVER] Player not found");
            }
        }

        [ClientRpc]
        private void RespawnClientRpc(Vector3 position, NetworkObjectReference player)
        {
            if (player.TryGet(out NetworkObject playerNetworkObject))
            {
                playerNetworkObject.GetComponent<CharacterController>().enabled = false;
                playerNetworkObject.transform.position = position;
                playerNetworkObject.GetComponent<CharacterController>().enabled = true;
            }
        }

        private Vector3 RandomPosition() => new(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));

        public override void OnDestroy()
        {
            base.OnDestroy();
            _lobby.OnClientConnected -= SpawnPlayerServerRpc;
        }
    }
}