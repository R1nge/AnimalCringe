using System.Collections;
using TMPro;
using UnityEngine;

namespace _Assets.Scripts.Misc
{
    public class DamagePopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;

        public void ShowPopup(Vector3 position, Vector3 direction, float duration, int damage)
        {
            //TODO: look at player
            //TODO: spawn a bit in front of the hit position
            Vector3 lookDirection = (position - direction).normalized;
            lookDirection.x = 0;
            lookDirection.z = 0;
            transform.LookAt(lookDirection);
            transform.position = (position + transform.forward).normalized;
            text.text = $"{damage}";
            gameObject.SetActive(true);
            StartCoroutine(Hide(duration));
        }

        private IEnumerator Hide(float duration)
        {
            yield return new WaitForSeconds(duration);
            gameObject.SetActive(false);
        }
    }
}