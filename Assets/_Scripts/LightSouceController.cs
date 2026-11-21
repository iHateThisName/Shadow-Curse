using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class LightSouceController : MonoBehaviour, IInteractable {

    [SerializeField] private SpriteRenderer lightSourceSpriteRenderer;
    [SerializeField] private Light2D lightSourceLight2D;
    [field: SerializeField] public bool IsLightOn { get; private set; } = false;

    public bool IsOn => IsLightOn;

    [SerializeField] private Sprite[] lightOnSprites;
    [SerializeField] private Sprite[] lightOffSprites;
    private Coroutine animationCoroutine;
    public UnityEvent<bool> OnLightStateChanged;
    private void Awake() {
        if (this.IsLightOn) {
            TurnOnLight();
        } else {
            TurnOffLight();
            
        }
    }

    [ContextMenu("Turn On Light")]
    public void TurnOnLight() {
        this.IsLightOn = true;
        this.lightSourceLight2D.enabled = true;
        if (this.animationCoroutine != null) StopCoroutine(this.animationCoroutine);
        this.animationCoroutine = StartCoroutine(AnimationPlayer(this.lightSourceSpriteRenderer, this.lightOnSprites, 0.1f, 0));
        OnLightStateChanged?.Invoke(true);
    }

    [ContextMenu("Turn Off Light")]
    public void TurnOffLight() {
        this.IsLightOn = false;
        this.lightSourceLight2D.enabled = false;
        if (this.animationCoroutine != null) StopCoroutine(this.animationCoroutine);
        this.animationCoroutine = StartCoroutine(AnimationPlayer(this.lightSourceSpriteRenderer, this.lightOffSprites, 0.1f, 0));
        OnLightStateChanged?.Invoke(false);
    }

    public static IEnumerator AnimationPlayer(SpriteRenderer renderer, Sprite[] sprites, float delay, int startIndex) {
        for (int i = startIndex; i < sprites.Length; i++) {
            if (i >= sprites.Length) i = 0; // Loop back to the start if we reach the end of the array
            renderer.sprite = sprites[i]; // Set the sprite to the current frame
            yield return new WaitForSeconds(delay); // Wait for the specified delay before moving to the next frame
        }
    }

    public void Interact() {
        if (this.IsLightOn) {
            GameManager.Instance.AddLight();
            TurnOffLight();
        } else {
            if (GameManager.Instance.SpendLight()) {
                TurnOnLight();
            }
        }
    }
}
