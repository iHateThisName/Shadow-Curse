using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button startButton;

    private void OnEnable() {
        this.startButton.onClick.AddListener(StartGame);
    }

    private void OnDisable() {
        this.startButton.onClick.RemoveListener(StartGame);
    }

    private void StartGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
