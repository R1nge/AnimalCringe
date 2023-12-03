using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Gameplay
{
    public struct PlayerRollbackData : INetworkSerializable, IEquatable<PlayerRollbackData>
    {
        public int Tick;
        public Vector3[] ColliderPositions;


        public PlayerRollbackData(int tick, Vector3[] colliderPositions)
        {
            Tick = tick;
            ColliderPositions = colliderPositions;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref ColliderPositions);
        }

        public bool Equals(PlayerRollbackData other)
        {
            return Equals(ColliderPositions, other.ColliderPositions) && Tick == other.Tick;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerRollbackData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ColliderPositions, Tick);
        }
    }
}