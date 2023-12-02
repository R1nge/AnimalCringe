namespace _Assets.Scripts.Services
{
    public class JoinCodeHolder
    {
        private string _joinCode;
        public string JoinCode => _joinCode;
        public void SetCode(string code) => _joinCode = code;
    }
}