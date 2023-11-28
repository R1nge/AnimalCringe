using System;
using _Assets.Scripts.Services.Skins;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace _Assets.Scripts.UIs
{
    public class SkinSlot : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Button select;
        private SkinService _skinService;
        private int _index;

        [Inject]
        private void Inject(SkinService skinService) => _skinService = skinService;

        private void Awake() => select.onClick.AddListener(SelectSkin);

        public void Initialize(int index)
        {
            _index = index;
            icon.sprite = _skinService.GetSkinSo(index).Icon;
        }

        private void SelectSkin() => _skinService.SelectSkin(_index);
    }
}