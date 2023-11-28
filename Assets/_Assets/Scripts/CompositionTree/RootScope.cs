using _Assets.Scripts.Services;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.CompositionTree
{
    public class RootScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ILogger, ConsoleLogger>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
        }
    }
}