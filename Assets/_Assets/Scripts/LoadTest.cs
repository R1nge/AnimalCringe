using System.Collections;
using System.Collections.Generic;
using _Assets.Scripts.Services;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts
{
    public class LoadTest : MonoBehaviour
    {
        private LifetimeScope _parent;
        private SceneLoader _sceneLoader;
        
        [Inject]
        private void Inject(LifetimeScope parent, SceneLoader sceneLoader)
        {
            _parent = parent;
            _sceneLoader = sceneLoader;
        }

        private IEnumerator Start()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;

            yield return new WaitForSeconds(10f);
            
            LoadScene();
        }

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
    }
}