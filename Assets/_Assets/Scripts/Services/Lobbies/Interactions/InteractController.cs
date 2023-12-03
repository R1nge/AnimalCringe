using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Lobbies.Interactions
{
    public class InteractController : NetworkBehaviour
    {
        [SerializeField] private Transform playerCamera;

        private void Update()
        {
            if (!IsOwner) return;
            if (Input.GetKeyDown(KeyCode.E))
            {
                Vector3 origin = playerCamera.position;
                Vector3 direction = playerCamera.forward;

                if (IsServer)
                {
                    Interact(origin, direction);
                }
                else
                {
                    InteractServerRpc(origin, direction);
                }
            }
        }

        private void Interact(Vector3 origin, Vector3 direction)
        {
            if (Physics.Raycast(origin, playerCamera.forward, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent(out IInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }

        [ServerRpc]
        private void InteractServerRpc(Vector3 origin, Vector3 direction) => Interact(origin, direction);
    }
}