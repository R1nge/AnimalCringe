using _Assets.Scripts.Misc;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace _Assets.Scripts.UIs
{
    public class NicknameUI : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI nicknameText;

        [ServerRpc(RequireOwnership = false)]
        public void SetNicknameServerRpc(NetworkString nickname) => SetNicknameClientRpc(nickname);

        [ClientRpc]
        private void SetNicknameClientRpc(NetworkString nickname) => nicknameText.text = nickname;
    }
}