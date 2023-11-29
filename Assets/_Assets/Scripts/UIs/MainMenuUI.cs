using _Assets.Scripts.Services;
using _Assets.Scripts.Services.Gameplay;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace _Assets.Scripts.UIs
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nicknameInput;
        [SerializeField] private Button host, join, skins;
        [SerializeField] private GameObject skinsMenu;
        private SceneLoader _sceneLoader;
        private NicknameService _nicknameService;

        [Inject]
        private void Inject(SceneLoader sceneLoader, NicknameService nicknameService)
        {
            _sceneLoader = sceneLoader;
            _nicknameService = nicknameService;
        }

        private void Awake()
        {
            SetNickname(string.Empty);
            nicknameInput.onValueChanged.AddListener(SetNickname);
            nicknameInput.onEndEdit.AddListener(SetNickname);
            host.onClick.AddListener(Host);
            join.onClick.AddListener(Join);
            skins.onClick.AddListener(ShowSkins);
        }

        private void SetNickname(string nickname)
        {
            if (nickname == string.Empty)
            {
                host.interactable = false;
                join.interactable = false;
            }
            else
            {
                _nicknameService.SetNickname(nickname);
                host.interactable = true;
                join.interactable = true;
            }
        }

        private void ShowSkins() => skinsMenu.SetActive(true);

        private void Host()
        {
            NetworkManager.Singleton.StartHost();
            _sceneLoader.LoadSceneNetwork("Lobby", LoadSceneMode.Single);
        }

        private void Join() => NetworkManager.Singleton.StartClient();

        private void OnDestroy()
        {
            host.onClick.RemoveAllListeners();
            join.onClick.RemoveAllListeners();
        }
    }
}