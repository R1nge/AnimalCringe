using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.Services.Lobbies.Interactions
{
    public class InteractStartGameUI : NetworkBehaviour, IInteractable
    {
        [SerializeField] private string mapToLoad;
        [Inject] private SceneLoader _sceneLoader;
        [Inject] private LifetimeScope _parent;

        private void Start() => NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;

        public void Interact()
        {
            if (IsServer)
            {
                LoadScene();
            }
        }

        private void LoadScene()
        {
            using (LifetimeScope.EnqueueParent(_parent))
            {
                _sceneLoader.LoadSceneNetwork(mapToLoad, LoadSceneMode.Additive);
            }
        }

        private void SceneManagerOnOnLoadEventCompleted(string sceneName, LoadSceneMode mode, List<ulong> completed, List<ulong> timedout)
        {
            if (sceneName == mapToLoad)
            {
                _sceneLoader.UnloadScene("Lobby");
            }
        }
    }
}