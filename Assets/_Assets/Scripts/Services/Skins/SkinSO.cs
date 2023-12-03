using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Skins
{
    [CreateAssetMenu(fileName = "SkinSO", menuName = "Configs/Skin")]
    public class SkinSO : ScriptableObject
    {
        [SerializeField] private Sprite icon;
        [SerializeField] private NetworkObject gameSkin;
        [SerializeField] private NetworkObject lobbySkin;
        
        public Sprite Icon => icon;
        public NetworkObject GameSkin => gameSkin;
        public NetworkObject LobbySkin => lobbySkin;
    }
}