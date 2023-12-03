using System;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Gameplay
{
    public class JumpPad : NetworkBehaviour
    {
        [SerializeField] private float force;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.TryGetComponent(out CPMPlayer player))
            {
                player.AddForce(Vector3.up * force);
                Debug.LogError("TRIGGER");
            }
        }
    }
}