using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace _Assets.Scripts.Services
{
    public class SceneLoader
    {
        public void UnloadScene(string name) => SceneManager.UnloadSceneAsync(name);

        public async UniTask LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            await SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        }

        public void LoadSceneNetwork(string sceneName, LoadSceneMode loadSceneMode)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, loadSceneMode);
        }
    }
}