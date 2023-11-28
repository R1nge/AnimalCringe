using System;
using _Assets.Scripts.Services.Lobbies;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts
{
    public class LobbyTest : MonoBehaviour
    {
        private Lobby _lobby;
        
        [Inject]
        private void Inject(Lobby lobby)
        {
            _lobby = lobby;
        }

        private void Start()
        {
            print(_lobby == null);
        }
    }
}