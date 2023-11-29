using System.Collections;
using TMPro;
using UnityEngine;

namespace _Assets.Scripts.Misc
{
    public class KillPopup : MonoBehaviour
    {
        [SerializeField] private float hideDelay = 1f;
        [SerializeField] private TextMeshProUGUI popupText;
        private Coroutine _hideCoroutine;
        private YieldInstruction _hide;

        private void Awake() => _hide = new WaitForSeconds(hideDelay);


        public void Show(NetworkString text)
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
            }

            popupText.text = text;
            _hideCoroutine = StartCoroutine(Hide_C());
        }

        private IEnumerator Hide_C()
        {
            yield return _hide;
            Hide();
        }

        private void Hide() => popupText.text = string.Empty;
    }
}