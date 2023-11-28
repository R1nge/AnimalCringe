using _Assets.Scripts.Services.Skins;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.UIs
{
    public class SkinMenuUI : MonoBehaviour
    {
        [SerializeField] private Transform parent;
        [SerializeField] private SkinSlot slot;
        [SerializeField] private GameObject skinsMenu;
        [SerializeField] private Button close;
        private IObjectResolver _objectResolver;
        private SkinService _skinService;

        [Inject]
        private void Inject(IObjectResolver objectResolver, SkinService skinService)
        {
            _objectResolver = objectResolver;
            _skinService = skinService;
        }

        private void Awake()
        {
            close.onClick.AddListener(() => skinsMenu.SetActive(false));
            SpawnSlots();
        }

        private void SpawnSlots()
        {
            for (int i = 0; i < _skinService.SkinsCount; i++)
            {
                SkinSlot slotInstance = _objectResolver.Instantiate(slot, parent);
                slotInstance.Initialize(i);
            }
        }
    }
}