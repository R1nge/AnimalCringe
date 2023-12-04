using System;
using _Assets.Scripts.Players.Movement;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Gameplay
{
    public struct PlayerRollbackData : INetworkSerializable, IEquatable<PlayerRollbackData>
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;

        public PlayerRollbackData(int tick, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            Tick = tick;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref Velocity);
        }

        public bool Equals(PlayerRollbackData other)
        {
            return Tick == other.Tick && Position.Equals(other.Position) && Rotation.Equals(other.Rotation) && Velocity.Equals(other.Velocity);
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerRollbackData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Tick, Position, Rotation, Velocity);
        }
    }
}