using _Assets.Scripts.Misc;
using _Assets.Scripts.Services.Gameplay;
using _Assets.Scripts.Services.Skins;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services.Lobbies
{
    public class LobbyNetwork : NetworkBehaviour
    {
        private Lobby _lobby;
        private SkinService _skinService;
        private NicknameService _nicknameService;

        [Inject]
        private void Inject(Lobby lobby, SkinService skinService, NicknameService nicknameService)
        {
            _lobby = lobby;
            _skinService = skinService;
            _nicknameService = nicknameService;
        }

        private void Awake()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnected;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _lobby.AddPlayer(NetworkManager.Singleton.LocalClientId, _skinService.SelectedSkinIndex, _nicknameService.Nickname);
                Debug.LogError($"{_nicknameService.Nickname}");
            }
        }

        private void ClientConnected(ulong clientId)
        {
            if (!IsServer)
            {
                int index = _skinService.SelectedSkinIndex;
                string nickname = _nicknameService.Nickname;
                ClientConnectedServerRpc(clientId, index, nickname);
                Debug.LogError($"{_nicknameService.Nickname}");
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void ClientConnectedServerRpc(ulong clientId, int skinIndex, NetworkString nickname)
        {
            _lobby.AddPlayer(clientId, skinIndex, nickname);
        }

        private void ClientDisconnected(ulong clientId) => _lobby.RemovePlayer(clientId);
    }
}