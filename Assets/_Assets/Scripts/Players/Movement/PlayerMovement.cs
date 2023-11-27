using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Players.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        private readonly Queue<PlayerMovementInput> _movementInputQueue = new();
        private PlayerMovementInput _playerMovementInput;
        private CharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
        }

        private void Update() => Move();

        private void Tick()
        {
            SendInputServerRpc(_playerMovementInput);
            MoveServerRpc();
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void SendInputServerRpc(PlayerMovementInput playerMovementInput)
        {
            _movementInputQueue.Enqueue(playerMovementInput);
        }

        [ServerRpc(Delivery = RpcDelivery.Unreliable)]
        private void MoveServerRpc()
        {
            if (_movementInputQueue.Count <= 0) return;
            PlayerMovementInput inputData = _movementInputQueue.Dequeue();
            _characterController.Move(inputData.MovementDirection);
        }

        private void Move()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float speedX = Input.GetAxis("Vertical");
            float speedZ = Input.GetAxis("Horizontal");
            Vector3 direction = forward * speedX + right * speedZ;
            _playerMovementInput = new PlayerMovementInput(direction);
            _characterController.Move(direction * Time.deltaTime);
        }
    }
}