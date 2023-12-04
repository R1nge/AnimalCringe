using Unity.Netcode;

namespace _Assets.Scripts.Players.Movement
{
    public struct PlayerInputCommand : INetworkSerializable
    {
        public float ForwardMove;
        public float RightMove;
        public bool WishJump;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref ForwardMove);
            serializer.SerializeValue(ref RightMove);
            serializer.SerializeValue(ref WishJump);
        }
    }
}