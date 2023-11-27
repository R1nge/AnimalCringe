using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Players
{
    public struct PlayerMovementInput : INetworkSerializable
    {
        private Vector3 _movementDirection;
        public Vector3 MovementDirection => _movementDirection;
        public PlayerMovementInput(Vector3 movementDirection) => _movementDirection = movementDirection;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _movementDirection);
        }
    }
}