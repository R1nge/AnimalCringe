using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Skins
{
    [CreateAssetMenu(fileName = "SkinSO", menuName = "Configs/Skin")]
    public class SkinSO : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private NetworkObject skin;
        
        public Sprite Icon => icon;
        public NetworkObject Skin => skin;
    }
}