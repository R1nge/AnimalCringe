using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services
{
    public class PlayerSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkObject player;

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayerServerRpc(ServerRpcParams rpcParams = default)
        {
            NetworkObject playerInstance = Instantiate(player);
            playerInstance.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        }
    }
}