using _Assets.Scripts.Services.Lobbies;
using VContainer;
using VContainer.Unity;

namespace _Assets.Scripts.CompositionTree
{
    public class LobbyScope : LifetimeScope
    {
        //TODO: pre load previous scope
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Lobby>(Lifetime.Singleton);
        }
    }
}