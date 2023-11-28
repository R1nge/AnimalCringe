using _Assets.Scripts.Services.Lobbies;
using _Assets.Scripts.Services.Skins;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.Services
{
    //TODO: create a factory
    public class PlayerSpawner : NetworkBehaviour
    {
        private IObjectResolver _objectResolver;
        private Lobby _lobby;
        private SkinService _skinService;

        [Inject]
        private void Inject(IObjectResolver objectResolver, Lobby lobby, SkinService skinService)
        {
            _objectResolver = objectResolver;
            _lobby = lobby;
            _skinService = skinService;
        }

        [ServerRpc(RequireOwnership = false)]
        public void SpawnPlayersServerRpc()
        {
            foreach (var pair in _lobby.LobbyData)
            {
                Debug.LogError($"Client id {pair.Value.ClientId} skin {_skinService.GetSkinSo(pair.Value.SelectedSkin).Skin.name}");
                NetworkObject playerInstance = _objectResolver.Instantiate(_skinService.GetSkinSo(pair.Value.SelectedSkin).Skin);
                playerInstance.SpawnWithOwnership(pair.Value.ClientId);
            }
        }
    }
}