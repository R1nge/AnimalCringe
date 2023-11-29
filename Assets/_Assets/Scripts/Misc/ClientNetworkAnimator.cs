using Unity.Netcode.Components;

namespace _Assets.Scripts.Misc
{
    public class ClientNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative() => false;
    }
}