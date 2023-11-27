using Unity.Netcode.Components;

namespace _Assets.Scripts
{
    public class ClientNetworkTransform : NetworkTransform
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}