using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Skins
{
    public class SkinService : MonoBehaviour
    {
        [SerializeField] private List<NetworkObject> skins;
        private int _selectedSkinIndex;
        
        public int SelectedSkinIndex => _selectedSkinIndex;
        public NetworkObject GetSkin(int index) => skins[index];

        public void SelectSkin(int skinIndex) => _selectedSkinIndex = skinIndex;
    }
}