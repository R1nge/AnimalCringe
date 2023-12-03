using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Misc
{
    public class HideFromLocalPlayer : NetworkBehaviour
    {
        [SerializeField] private MeshRenderer[] meshRenderers;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                for (int i = 0; i < meshRenderers.Length; i++)
                {
                    meshRenderers[i].enabled = false;
                }
            }
        }
    }
}