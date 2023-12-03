using System;
using System.Collections.Generic;
using _Assets.Scripts.Misc;

namespace _Assets.Scripts.Services.Lobbies
{
    public class Lobby
    {
        public event Action<ulong> OnClientConnected; 
        private readonly ILogger _logger;
        private readonly Dictionary<ulong, LobbyPlayerData> _lobbyData = new();

        public Dictionary<ulong, LobbyPlayerData> LobbyData => _lobbyData;

        private Lobby(ILogger logger) => _logger = logger;

        public void AddPlayer(ulong clientId, int skinIndex, NetworkString nickname)
        {
            if (_lobbyData.TryAdd(clientId, new LobbyPlayerData(clientId, skinIndex, nickname)))
            {
                OnClientConnected?.Invoke(clientId);
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