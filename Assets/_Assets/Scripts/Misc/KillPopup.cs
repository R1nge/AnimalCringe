using _Assets.Scripts.Services.Lobbies;
using TMPro;
using UnityEngine;
using VContainer;

namespace _Assets.Scripts.Misc
{
    public class KillPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        private Lobby _lobby;
        
        [Inject]
        private void Inject(Lobby lobby)
        {
            _lobby = lobby;
        }
        
        public void Show(ulong killedId)
        {
            NetworkString killedName = _lobby.LobbyData[killedId].Nickname;
            _text.text = killedName;
        }

        public void Hide()
        {
            _text.text = string.Empty;
        }
    }
}