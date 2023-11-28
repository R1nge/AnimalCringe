using _Assets.Scripts.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        private SceneLoader _sceneLoader;

        [Inject]
        private void Inject(SceneLoader sceneLoader) => _sceneLoader = sceneLoader;

        private async void Start() => await _sceneLoader.LoadSceneAsync("Main", LoadSceneMode.Single);
    }
}