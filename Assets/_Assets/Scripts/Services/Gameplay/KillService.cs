using System;
using System.Collections;
using System.Collections.Generic;
using _Assets.Scripts.Misc;
using _Assets.Scripts.Services.Lobbies;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services.Gameplay
{
    public class KillService : NetworkBehaviour
    {
        private Lobby _lobby;
        private PlayerSpawner _playerSpawner;
        private readonly List<ulong> _respawnList = new();

        [Inject]
        private void Inject(Lobby lobby, PlayerSpawner playerSpawner)
        {
            _lobby = lobby;
            _playerSpawner = playerSpawner;
        }

        private void Start()
        {
            if (IsServer)
            {
                StartCoroutine(Respawn_C());
                Debug.LogError("Started a coroutine");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void KillServerRpc(ulong killedId, ulong killerId)
        {
            if (NetworkManager.SpawnManager.GetPlayerNetworkObject(killerId))
            {
                if (NetworkManager.SpawnManager.GetPlayerNetworkObject(killerId))
                {
                    var clientRpc = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams
                        {
                            TargetClientIds = new[] { killerId }
                        }
                    };

                    NetworkString killedNickname = _lobby.LobbyData[killedId].Nickname;

                    _respawnList.Add(killedId);

                    ShowPopupClientRpc(NetworkManager.SpawnManager.GetPlayerNetworkObject(killerId), killedNickname, clientRpc);
                }
            }

            //(Show kill feed)
        }

        private IEnumerator Respawn_C()
        {
            while (enabled)
            {
                if (_respawnList.Count == 0)
                {
                    yield return null;
                }

                for (int i = 0; i < _respawnList.Count; i++)
                {
                    _playerSpawner.RespawnServerRpc(_respawnList[i]);
                }

                _respawnList.Clear();

                yield return new WaitForSeconds(5f);
            }
        }

        [ClientRpc]
        private void ShowPopupClientRpc(NetworkObjectReference player, NetworkString killedNickname, ClientRpcParams rpcParams)
        {
            if (player.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent(out KillPopup killPopup))
                {
                    //TODO: move it a bit lower the crosshair
                    killPopup.Show($"{killedNickname}");
                }
            }
        }
    }
}