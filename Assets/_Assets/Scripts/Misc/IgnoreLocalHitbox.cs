using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Misc
{
    public class IgnoreLocalHitbox : NetworkBehaviour
    {
        // private LayerMask _localPlayerLayer;
        //
        // private void Start()
        // {
        //     if (IsLocalPlayer)
        //     {
        //         SetLayer();
        //     }
        // }
        //
        // //For the server the layer is playerIgnore, since the weapon ingnores it the hit doesn't happen
        //
        // private void SetLayer()
        // {
        //     _localPlayerLayer = LayerMask.NameToLayer("LocalPlayer");
        //
        //     Transform[] children = GetComponentsInChildren<Transform>(true);
        //
        //     foreach (var child in children)
        //     {
        //         child.gameObject.layer = _localPlayerLayer;
        //     }
        // }
    }
}