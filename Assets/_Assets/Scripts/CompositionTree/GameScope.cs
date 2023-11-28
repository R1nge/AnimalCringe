using _Assets.Scripts.Services;
using _Assets.Scripts.Services.StateMachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.CompositionTree
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private PlayerSpawner playerSpawner;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerSpawner);
            builder.Register<GameStatesFactory>(Lifetime.Singleton);
            builder.Register<GameStateMachine>(Lifetime.Singleton);
        }
    }
}