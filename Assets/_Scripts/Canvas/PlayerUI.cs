using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
    public static PlayerUI Instance { get; private set; }

    [SerializeField] private Sprite mirrorOn;
    [SerializeField] private Sprite mirrorOff;

    [SerializeField] private Image oneMirror;
    [SerializeField] private Image twoMirror;
    [SerializeField] private Image threeMirror;

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this.gameObject);
        } else {
            Instance = this;
        }

        UpdateMirrorUI(GameManager.Instance.CurrentLights);
    }
    public void UpdateMirrorUI(int mirrorCount) {
        this.oneMirror.sprite = mirrorCount >= 1 ? this.mirrorOn : this.mirrorOff;
        this.twoMirror.sprite = mirrorCount >= 2 ? this.mirrorOn : this.mirrorOff;
        this.threeMirror.sprite = mirrorCount >= 3 ? this.mirrorOn : this.mirrorOff;
    }
}
