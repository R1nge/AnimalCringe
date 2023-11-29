using _Assets.Scripts.Services.Lobbies;
using _Assets.Scripts.Services.Skins;
using _Assets.Scripts.UIs;
using Unity.Netcode;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.Services.Factories
{
    public class PlayerFactory
    {
        private readonly IObjectResolver _objectResolver;
        private readonly Lobby _lobby;
        private readonly SkinService _skinService;

        private PlayerFactory(IObjectResolver objectResolver, Lobby lobby, SkinService skinService)
        {
            _objectResolver = objectResolver;
            _lobby = lobby;
            _skinService = skinService;
        }

        public NetworkObject CreatePlayer(ulong clientId)
        {
            NetworkObject playerInstance = _objectResolver.Instantiate(_skinService.GetSkinSo(_lobby.LobbyData[clientId].SelectedSkin).Skin);
            playerInstance.SpawnAsPlayerObject(clientId);
            
            if (playerInstance.TryGetComponent(out NicknameUI nicknameUI))
            {
                nicknameUI.SetNicknameServerRpc(_lobby.LobbyData[clientId].Nickname);
            }

            return playerInstance;
        }
    }
}