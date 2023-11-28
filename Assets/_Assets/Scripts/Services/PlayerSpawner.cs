using _Assets.Scripts.Services.Lobbies;
using _Assets.Scripts.Services.Skins;
using Unity.Netcode;
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
        public void SpawnPlayerServerRpc()
        {
            foreach (var pair in _lobby.LobbyData)
            {
                NetworkObject playerInstance = Instantiate(_skinService.GetSkin(pair.Value.SelectedSkin));
                playerInstance.SpawnWithOwnership(pair.Value.ClientId);
            }
        }
    }
}