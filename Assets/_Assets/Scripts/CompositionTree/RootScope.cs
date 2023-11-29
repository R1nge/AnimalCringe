using _Assets.Scripts.Services;
using _Assets.Scripts.Services.Skins;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ILogger = _Assets.Scripts.Services.ILogger;

namespace _Assets.Scripts.CompositionTree
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private SkinService skinService;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(skinService);

            builder.Register<NicknameService>(Lifetime.Singleton);
            builder.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
        }
    }
}