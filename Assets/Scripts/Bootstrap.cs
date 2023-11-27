using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private void Start() => SceneManager.LoadSceneAsync("Main");
}