using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Gameplay
{
    public class RollbackService : NetworkBehaviour
    {
        private NetworkVariable<int> _currentTick;
        public int CurrentTick => _currentTick.Value;

        private void Awake()
        {
            NetworkManager.Singleton.NetworkTickSystem.Tick += OnTick;
            _currentTick = new NetworkVariable<int>();
        }

        private void OnTick()
        {
            if (!IsServer) return;
            _currentTick.Value++;
        }
    }

    public struct PlayerRollbackData : INetworkSerializable, IEquatable<PlayerRollbackData>
    {
        public Vector3 Position;
        public int Tick;

        public PlayerRollbackData(Vector3 position, int tick)
        {
            Position = position;
            Tick = tick;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Tick);
        }

        public bool Equals(PlayerRollbackData other)
        {
            return Position.Equals(other.Position) && Tick == other.Tick;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerRollbackData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, Tick);
        }
    }
}