using _Assets.Scripts.Services.Factories;
using _Assets.Scripts.Services.Gameplay;
using _Assets.Scripts.Services.StateMachine;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.CompositionTree
{
    public class GameScope : LifetimeScope
    {
        [SerializeField] private PlayerSpawner playerSpawner;
        [SerializeField] private KillService killService;
        [SerializeField] private RollbackService rollbackService;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(rollbackService);
            builder.Register<PlayerFactory>(Lifetime.Singleton);
            builder.RegisterComponent(playerSpawner);
            builder.RegisterComponent(killService);
            builder.Register<GameStatesFactory>(Lifetime.Singleton);
            builder.Register<GameStateMachine>(Lifetime.Singleton);
        }
    }
}