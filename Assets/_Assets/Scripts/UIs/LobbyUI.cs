using System.Collections.Generic;
using _Assets.Scripts.Services;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.UIs
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI joinCodeText;
        [SerializeField] private Button start;
        private LifetimeScope _parent;
        private SceneLoader _sceneLoader;
        private JoinCodeHolder _joinCodeHolder;

        [Inject]
        private void Inject(LifetimeScope parent, SceneLoader sceneLoader, JoinCodeHolder joinCodeHolder)
        {
            _parent = parent;
            _sceneLoader = sceneLoader;
            _joinCodeHolder = joinCodeHolder;
        }

        private void Start()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;

            start.gameObject.SetActive(NetworkManager.Singleton.IsHost);

            if (NetworkManager.Singleton.IsHost)
            {
                joinCodeText.text = _joinCodeHolder.JoinCode;
                start.onClick.AddListener(StartGame);
            }
        }

        private void StartGame() => LoadScene();

        //TODO: make a method 'LoadSceneWithInjectedParent();
        private void LoadScene()
        {
            using (LifetimeScope.EnqueueParent(_parent))
            {
                _sceneLoader.LoadSceneNetwork("Test", LoadSceneMode.Additive);
            }
        }

        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode mode, List<ulong> completed, List<ulong> timedout)
        {
            if (sceneName == "Test")
            {
                _sceneLoader.UnloadScene("Lobby");
            }
        }

        private void OnDestroy() => start.onClick.RemoveAllListeners();
    }
}