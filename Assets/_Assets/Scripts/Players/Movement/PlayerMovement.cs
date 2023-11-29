using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Players.Movement
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<float> speed;
        [SerializeField] private float gravity;
        [SerializeField] private float jumpForce;
        private readonly Queue<PlayerMovementInput> _movementInputQueue = new();
        private PlayerMovementInput _playerMovementInput;
        private CharacterController _characterController;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            NetworkManager.Singleton.NetworkTickSystem.Tick += Tick;
        }

        private void Update()
        {
            if (!IsOwner) return;
            Move();
        }

        private void Tick()
        {
            if (!IsOwner) return;
            if (IsServer) return;
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
            _characterController.Move(inputData.MovementDirection * speed.Value);
        }

        private void Move()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            Vector3 right = transform.TransformDirection(Vector3.right);
            float speedX = Input.GetAxis("Vertical");
            float speedZ = Input.GetAxis("Horizontal");
            bool jump = Input.GetButtonDown("Jump");
            Vector3 direction = forward * speedX + right * speedZ;
            var movementDirectionY = direction.y;

            if (jump && _characterController.isGrounded)
            {
                Debug.LogError("Jumped");
                direction.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            }
            else
            {
                direction.y = movementDirectionY;
            }
            
            if (!_characterController.isGrounded)
            {
                direction.y -= gravity * Time.deltaTime;
            }
            
            _playerMovementInput = new PlayerMovementInput(direction);

            _characterController.Move(direction * (speed.Value * Time.deltaTime));
        }
    }
}