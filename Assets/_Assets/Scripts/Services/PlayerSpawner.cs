using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject player;

        public override void OnNetworkSpawn() => SpawnPlayerServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
        {
            NetworkObject playerInstance = Instantiate(player);
            playerInstance.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }
    }
}