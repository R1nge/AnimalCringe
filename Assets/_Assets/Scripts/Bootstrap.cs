using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Assets.Scripts
{
    public class Bootstrap : MonoBehaviour
    {
        private void Start() => SceneManager.LoadSceneAsync("Test");
    }
}