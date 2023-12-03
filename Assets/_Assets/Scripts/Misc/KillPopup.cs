using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Assets.Scripts.Misc
{
    public class KillPopup : MonoBehaviour
    {
        [SerializeField] private float duration = .5f;
        [SerializeField] private TextMeshProUGUI popupText;

        public void Show(NetworkString text)
        {
            popupText.text = text;
            popupText.transform.DOScale(Vector3.one, duration).OnComplete(() => { popupText.transform.DOScale(Vector3.zero, duration); });
        }
    }
}