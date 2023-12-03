using _Assets.Scripts.Services;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Services.Authentication;
using Unity.Services.Core;
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

        private async void Start()
        {
            await SingIn();
            await _sceneLoader.LoadSceneAsync("Main", LoadSceneMode.Single);
        }

        private async UniTask SingIn()
        {
            DOTween.Init();
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("The player has singed in anonymously");
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
}