﻿using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services.Lobbies
{
    public class LobbyKillZone : NetworkBehaviour
    {
        [Inject] private LobbySpawner _lobbySpawner;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.TryGetComponent(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent(out CPMPlayer player))
                {
                    _lobbySpawner.RespawnServerRpc(networkObject.OwnerClientId);
                }
            }
        }
    }
}