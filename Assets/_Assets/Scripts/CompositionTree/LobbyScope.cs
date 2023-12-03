using _Assets.Scripts.Services.Lobbies;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.CompositionTree
{
    public class LobbyScope : LifetimeScope
    {
        [SerializeField] private LobbySpawner lobbySpawner;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(lobbySpawner);
            builder.Register<Lobby>(Lifetime.Singleton);
        }
    }
}