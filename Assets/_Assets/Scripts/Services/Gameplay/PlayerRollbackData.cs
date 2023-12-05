using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Gameplay
{
    public struct PlayerRollbackData : INetworkSerializable, IEquatable<PlayerRollbackData>
    {
        public double Time;
        public Vector3[] ColliderPositions;


        public PlayerRollbackData(double time, Vector3[] colliderPositions)
        {
            Time = time;
            ColliderPositions = colliderPositions;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Time);
            serializer.SerializeValue(ref ColliderPositions);
        }

        public bool Equals(PlayerRollbackData other)
        {
            return Time.Equals(other.Time) && Equals(ColliderPositions, other.ColliderPositions);
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerRollbackData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Time, ColliderPositions);
        }
    }
}