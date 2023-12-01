using System.Collections.Generic;
using _Assets.Scripts.Services;
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
        [SerializeField] private Button start;
        private LifetimeScope _parent;
        private SceneLoader _sceneLoader;

        [Inject]
        private void Inject(LifetimeScope parent, SceneLoader sceneLoader)
        {
            _parent = parent;
            _sceneLoader = sceneLoader;
        }

        private void Start()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            start.gameObject.SetActive(NetworkManager.Singleton.IsServer);
            start.onClick.AddListener(StartGame);
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