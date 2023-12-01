using System;
using System.Collections;
using System.Collections.Generic;
using _Assets.Scripts.Services.Lobbies;
using _Assets.Scripts.Services.StateMachine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using VContainer;

namespace _Assets.Scripts.Services
{
    public class EntryPoint : NetworkBehaviour
    {
        private GameStateMachine _gameStateMachine;
        private Lobby _lobby;

        [Inject]
        private void Inject(GameStateMachine gameStateMachine, Lobby lobby)
        {
            _gameStateMachine = gameStateMachine;
            _lobby = lobby;
        }

        private void Awake()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnLoadEventCompleted;
        }

        private void OnLoadEventCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
        {
            if (!IsServer) return;
            if (clientscompleted.Count + clientstimedout.Count == _lobby.LobbyData.Count)
            {
                _gameStateMachine.SwitchState(GameStateType.Game);
            }
        }
    }
}