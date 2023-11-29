using _Assets.Scripts.Services.Lobbies;
using TMPro;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Misc
{
    public class KillPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;


        public void Show(NetworkString text)
        {
            _text.text = text;
        }

        public void Hide()
        {
            _text.text = string.Empty;
        }
    }
}