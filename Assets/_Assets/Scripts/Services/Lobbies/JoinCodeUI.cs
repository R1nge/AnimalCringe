using TMPro;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Services.Lobbies
{
    public class JoinCodeUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI joinCodeText;
        [Inject] private JoinCodeHolder _joinCodeHolder;

        private void Start()
        {
            if (IsServer)
            {
                joinCodeText.text = _joinCodeHolder.JoinCode;
            }
        }
    }
}