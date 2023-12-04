using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Gameplay
{
    public class JumpPad : NetworkBehaviour
    {
        [SerializeField] private float force;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out CPMPlayer player))
            {
                player.AddForce(Vector3.up * force);
                return;
            }

            if (other.transform.root.TryGetComponent(out CPMPlayer playerRoot))
            {
                playerRoot.AddForce(Vector3.up * force);
            }
        }
    }
}