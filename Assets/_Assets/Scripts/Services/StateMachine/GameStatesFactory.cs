using _Assets.Scripts.Services.Gameplay;
using _Assets.Scripts.Services.StateMachine.States;

namespace _Assets.Scripts.Services.StateMachine
{
    public class GameStatesFactory
    {
        private readonly PlayerSpawner _playerSpawner;

        public GameStatesFactory(PlayerSpawner playerSpawner)
        {
            _playerSpawner = playerSpawner;
        }
        
        public IGameState CreateGameState(GameStateMachine stateMachine)
        {
            return new GameState(stateMachine, _playerSpawner);
        }
    }
}