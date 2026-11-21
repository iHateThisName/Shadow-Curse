using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance { get; private set; }

    private readonly int MaxLights = 3;
    [field: SerializeField] public int CurrentLights { get; private set; } = 0;

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
            return true;
        }
        return false;
    }

    public bool AddLight() {
        if (CurrentLights < MaxLights) {
            CurrentLights++;
            return true;
        }
        return false;
    }


}
