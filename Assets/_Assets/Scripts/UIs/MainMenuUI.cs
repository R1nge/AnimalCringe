using _Assets.Scripts.Services;
using _Assets.Scripts.Services.Gameplay;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace _Assets.Scripts.UIs
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField nicknameInput;
        [SerializeField] private TMP_InputField joinCodeInput;
        [SerializeField] private Button host, join, skins;
        [SerializeField] private GameObject skinsMenu;
        private SceneLoader _sceneLoader;
        private NicknameService _nicknameService;
        private JoinCodeHolder _joinCodeHolder;

        [Inject]
        private void Inject(SceneLoader sceneLoader, NicknameService nicknameService, JoinCodeHolder joinCodeHolder)
        {
            _sceneLoader = sceneLoader;
            _nicknameService = nicknameService;
            _joinCodeHolder = joinCodeHolder;
        }

        private void Awake()
        {
            SetNickname(string.Empty);
            nicknameInput.onValueChanged.AddListener(SetNickname);
            nicknameInput.onEndEdit.AddListener(SetNickname);
            joinCodeInput.onValueChanged.AddListener(SetJoinCode);
            joinCodeInput.onValueChanged.AddListener(SetJoinCode);
            host.onClick.AddListener(HostBtn);
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
        
        private void SetJoinCode(string code) => _joinCodeHolder.SetCode(code);

        private void ShowSkins() => skinsMenu.SetActive(true);

        private async void HostBtn()
        {
            await Host();
        }

        private async UniTask Host()
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(5);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            _joinCodeHolder.SetCode(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData
            (
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            NetworkManager.Singleton.StartHost();
            _sceneLoader.LoadSceneNetwork("Lobby", LoadSceneMode.Single);
        }

        private async void Join()
        {
            string joinCode = _joinCodeHolder.JoinCode;
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData
            (
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );
            
            NetworkManager.Singleton.StartClient();
        }

        private void OnDestroy()
        {
            host.onClick.RemoveAllListeners();
            join.onClick.RemoveAllListeners();
        }
    }
}