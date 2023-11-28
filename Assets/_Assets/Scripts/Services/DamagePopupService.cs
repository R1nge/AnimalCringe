using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services
{
    public class DamagePopupService : NetworkBehaviour
    {
        [ClientRpc]
        public void ShowPopupClientRpc(Vector3 position, float duration, ClientRpcParams clientRpcParams)
        {
            Debug.LogError("Showed a popup");
        }
    }
}