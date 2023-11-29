using _Assets.Scripts.Misc;
using _Assets.Scripts.Services.Lobbies;
using Unity.Netcode;
using VContainer;

namespace _Assets.Scripts.Services
{
    public class KillService : NetworkBehaviour
    {
        private Lobby _lobby;

        [Inject]
        private void Inject(Lobby lobby) => _lobby = lobby;

        [ServerRpc(RequireOwnership = false)]
        public void KillServerRpc(ulong killedId, ulong killerId)
        {
            if (NetworkManager.SpawnManager.GetPlayerNetworkObject(killerId))
            {
                if (NetworkManager.SpawnManager.GetPlayerNetworkObject(killerId))
                {
                    var clientRpc = new ClientRpcParams
                    {
                        Send = new ClientRpcSendParams()
                        {
                            TargetClientIds = new[] { killerId }
                        }
                    };
                    
                    NetworkString killedNickname = _lobby.LobbyData[killedId].Nickname;

                    ShowPopupClientRpc(NetworkManager.SpawnManager.GetPlayerNetworkObject(killerId),killedNickname,clientRpc);
                }
            }
            
            //(Show kill feed)
        }

        [ClientRpc]
        private void ShowPopupClientRpc(NetworkObjectReference player, NetworkString killedNickname, ClientRpcParams rpcParams)
        {
            if (player.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent(out KillPopup killPopup))
                {
                    killPopup.Show($"Killed {killedNickname}");
                }
            }
        }
    }
}