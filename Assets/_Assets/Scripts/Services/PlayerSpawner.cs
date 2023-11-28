using _Assets.Scripts.Services.Lobbies;
using _Assets.Scripts.Services.Skins;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services
{
    public class PlayerSpawner : NetworkBehaviour
    {
        private Lobby _lobby;
        private SkinService _skinService;

        [Inject]
        private void Inject(Lobby lobby, SkinService skinService)
        {
            _lobby = lobby;
            _skinService = skinService;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayersServerRpc()
        {
            foreach (var pair in _lobby.LobbyData)
            {
                Debug.LogError($"Client id {pair.Value.ClientId} skin {_skinService.GetSkinSo(pair.Value.SelectedSkin).Skin.name}");
                NetworkObject playerInstance = Instantiate(_skinService.GetSkinSo(pair.Value.SelectedSkin).Skin);
                playerInstance.SpawnWithOwnership(pair.Value.ClientId);
            }
        }
    }
}