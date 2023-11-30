using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Gameplay
{
    public struct PlayerRollbackData : INetworkSerializable, IEquatable<PlayerRollbackData>
    {
        public Vector3[] Positions;
        public int Tick;

        public PlayerRollbackData(Vector3[] positions, int tick)
        {
            Positions = positions;
            Tick = tick;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref Positions);
        }

        public bool Equals(PlayerRollbackData other)
        {
            return Equals(Positions, other.Positions) && Tick == other.Tick;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerRollbackData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Positions, Tick);
        }
    }
}