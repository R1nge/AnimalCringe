using System.Collections.Generic;
using _Assets.Scripts.Misc;

namespace _Assets.Scripts.Services.Lobbies
{
    public class Lobby
    {
        private readonly ILogger _logger;
        private readonly Dictionary<ulong, LobbyData> _lobbyData = new();
        
        public Dictionary<ulong, LobbyData> LobbyData => _lobbyData;

        private Lobby(ILogger logger) => _logger = logger;

        public void AddPlayer(ulong clientId, int skinIndex, NetworkString nickname)
        {
            if (_lobbyData.TryAdd(clientId, new LobbyData(clientId, skinIndex, nickname)))
            {
                _logger.Log("Successfully added player");
            }
            else
            {
                _logger.Log("Failed to add player");
            }
        }

        public void RemovePlayer(ulong clientId)
        {
            if (_lobbyData.Remove(clientId))
            {
                _logger.Log("Successfully removed player");
            }
            else
            {
                _logger.Log("Failed to remove player");
            }
        }
    }
}