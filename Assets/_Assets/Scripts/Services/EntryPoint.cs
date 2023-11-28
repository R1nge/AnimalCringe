using System.Collections;
using _Assets.Scripts.Services.StateMachine;
using Unity.Netcode;
using VContainer;

namespace _Assets.Scripts.Services
{
    public class EntryPoint : NetworkBehaviour
    {
        private GameStateMachine _gameStateMachine;
        
        [Inject]
        private void Inject(GameStateMachine gameStateMachine)
        {
            _gameStateMachine = gameStateMachine;
        }

        private IEnumerator Start()
        {
            yield return null;
            _gameStateMachine.SwitchState(GameStateType.Game);
        }
    }
}