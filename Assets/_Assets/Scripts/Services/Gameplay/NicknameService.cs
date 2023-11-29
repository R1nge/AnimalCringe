namespace _Assets.Scripts.Services.Gameplay
{
    public class NicknameService
    {
        private string _nickname;
        public string Nickname => _nickname;
        public void SetNickname(string nickname) => _nickname = nickname;
    }
}