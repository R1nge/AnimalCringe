namespace _Assets.Scripts.Services.StateMachine.States
{
    public class GameState : IGameState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly PlayerSpawner _playerSpawner;

        public GameState(GameStateMachine stateMachine, PlayerSpawner playerSpawner)
        {
            _stateMachine = stateMachine;
            _playerSpawner = playerSpawner;
        }

        public void Enter()
        {
            _playerSpawner.SpawnPlayerServerRpc();
        }

        public void Exit() { }
    }
}