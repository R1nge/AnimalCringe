using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.Services.Skins
{
    public class SkinService : MonoBehaviour
    {
        [SerializeField] private List<SkinSO> skins;
        private int _selectedSkinIndex;
        
        public int SelectedSkinIndex => _selectedSkinIndex;
        public int SkinsCount => skins.Count;
        public SkinSO GetSkinSo(int index) => skins[index];

        public void SelectSkin(int skinIndex) => _selectedSkinIndex = skinIndex;
    }
}