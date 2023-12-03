using UnityEngine;

namespace _Assets.Scripts.Players
{
    public class PlayerDeathController : MonoBehaviour
    {
        private PlayerInput _playerInput;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        public void Die()
        {
            _playerInput.EnableServerRpc(false);
            //TODO: Enable ragdoll
        }
    }
}