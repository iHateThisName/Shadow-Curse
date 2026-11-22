using UnityEngine;
[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    private readonly int MaxLights = 3;
    [field: SerializeField] public int CurrentLights { get; private set; } = 0;

    public static readonly string PlayerTag = "Player";

    public bool IsPlayerDead { get; private set; } = false;

    [SerializeField] private Canvas gameOverCanvas;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }
    }

    public bool SpendLight() {
        if (CurrentLights > 0) {
            CurrentLights--;
            PlayerUI.Instance.UpdateMirrorUI(this.CurrentLights);
            return true;
        }
        return false;
    }

    public bool AddLight() {
        if (CurrentLights < MaxLights) {
            CurrentLights++;
            PlayerUI.Instance.UpdateMirrorUI(this.CurrentLights);
            return true;
        }
        return false;
    }

    public void OnPlayerKilled() {
        Debug.Log("Player has been killed.");
        this.IsPlayerDead = true;
        this.gameOverCanvas.gameObject.SetActive(true);
    }

    public void loadSceneInIndex(int index) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }


}
