using _Assets.Scripts.Misc;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services
{
    public class DamagePopupService : NetworkBehaviour
    {
        [SerializeField] private DamagePopup damagePopupPrefab;

        [ClientRpc]
        public void ShowPopupClientRpc(Vector3 position, Vector3 direction, float duration, int damage, ClientRpcParams clientRpcParams)
        {
            Debug.LogError("Showed a popup");
            DamagePopup damagePopup = Instantiate(damagePopupPrefab);
            damagePopup.ShowPopup(position, direction, duration, damage);
        }
    }
}