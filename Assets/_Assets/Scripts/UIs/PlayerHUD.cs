using _Assets.Scripts.Damageables;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.UIs
{
    public class PlayerHUD : NetworkBehaviour
    {
        [SerializeField] private GameObject hud;
        [SerializeField] private TextMeshProUGUI healthText;
        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _health.OnHealthChanged += UpdateHealthUI;
        }

        private void Start()
        {
            hud.SetActive(IsOwner);
        }

        private void UpdateHealthUI(float health) => healthText.text = $"Health: {health}";

        public override void OnDestroy()
        {
            base.OnDestroy();
            _health.OnHealthChanged += UpdateHealthUI;
        }
    }
}