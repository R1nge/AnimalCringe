﻿using _Assets.Scripts.Services;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

namespace _Assets.Scripts.UIs
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button host, join;
        private SceneLoader _sceneLoader;

        [Inject]
        private void Inject(SceneLoader sceneLoader)
        {
            _sceneLoader = sceneLoader;
        }

        private void Awake()
        {
            host.onClick.AddListener(Host);
            join.onClick.AddListener(Join);
        }

        private void Host()
        {
            NetworkManager.Singleton.StartHost();
            _sceneLoader.LoadSceneNetwork("Lobby", LoadSceneMode.Single);
        }

        private void Join() => NetworkManager.Singleton.StartClient();

        private void OnDestroy()
        {
            host.onClick.RemoveAllListeners();
            join.onClick.RemoveAllListeners();
        }
    }
}