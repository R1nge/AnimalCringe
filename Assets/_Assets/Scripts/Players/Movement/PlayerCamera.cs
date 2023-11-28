using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Players.Movement
{
    public class PlayerCamera : NetworkBehaviour
    {
        [SerializeField] private Camera playerCamera;
        [SerializeField] private float lookSpeed = 2f;
        [SerializeField] private float lookXLimit = 90f;
        private float _rotationX;

        private void Start()
        {
            playerCamera.enabled = IsOwner;
            playerCamera.GetComponent<AudioListener>().enabled = IsOwner;
        }

        private void Update()
        {
            if (!IsOwner) return;
            Rotate();
        }

        private void Rotate()
        {
            _rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            _rotationX = Mathf.Clamp(_rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}