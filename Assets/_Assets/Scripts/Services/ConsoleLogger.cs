using UnityEngine;

namespace _Assets.Scripts.Services
{
    public class ConsoleLogger : ILogger
    {
        public void Log(string message) => Debug.Log(message);
    }
}