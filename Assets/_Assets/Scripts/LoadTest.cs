using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts
{
    public class LoadTest : MonoBehaviour
    {
        //TODO: pre load previous scope

        private LifetimeScope _parent;
        
        [Inject]
        private void Inject(LifetimeScope parent) => _parent = parent;

        private void Start()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManagerOnOnLoadEventCompleted;
            
            //Lobby
            print(_parent.name);
            LoadSceneAsync();
        }

        private void SceneManagerOnOnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (scenename == "Test")
            {
                SceneManager.UnloadSceneAsync("Lobby");
            }
        }

        private void LoadSceneAsync()
        {
            using (LifetimeScope.EnqueueParent(_parent))
            {
                NetworkManager.Singleton.SceneManager.LoadScene("Test", LoadSceneMode.Additive);
                
                //await SceneManager.LoadSceneAsync("Test", LoadSceneMode.Additive);
            }
        }
    }
}