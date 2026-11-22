using UnityEngine;
using UnityEngine.UI;

public class GameOverUIController : MonoBehaviour {
    [SerializeField] private Button restartButton;

    private void OnEnable() {
        this.restartButton.onClick.AddListener(RestartScene);
    }

    private void OnDisable() {
        this.restartButton.onClick.RemoveListener(RestartScene);
    }

    private void RestartScene() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
