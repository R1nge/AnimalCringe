using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Assets.Scripts.UIs
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button host, join;

        private void Awake()
        {
            host.onClick.AddListener(Host);
            join.onClick.AddListener(Join);
        }

        private void Host()
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
        }

        private void Join() => NetworkManager.Singleton.StartClient();

        private void OnDestroy()
        {
            host.onClick.RemoveAllListeners();
            join.onClick.RemoveAllListeners();
        }
    }
}