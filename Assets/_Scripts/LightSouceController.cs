using System.Collections;
using UnityEngine;

public class LightSouceController : MonoBehaviour {

    [SerializeField] private SpriteRenderer lightSourceSpriteRenderer;
    [field: SerializeField] public bool IsLightOn { get; private set; } = false;
    [SerializeField] private Sprite[] lightOnSprites;
    [SerializeField] private Sprite[] lightOffSprites;
    private Coroutine animationCoroutine;

    public void TurnOnLight() {
        this.IsLightOn = true;
        if (this.animationCoroutine != null) StopCoroutine(this.animationCoroutine);
        this.animationCoroutine = StartCoroutine(AnimationPlayer(this.lightSourceSpriteRenderer, this.lightOnSprites, 0.1f, 0));
    }

    public void TurnOffLight() {
        this.IsLightOn = false;
        if (this.animationCoroutine != null) StopCoroutine(this.animationCoroutine);
        this.animationCoroutine = StartCoroutine(AnimationPlayer(this.lightSourceSpriteRenderer, this.lightOffSprites, 0.1f, 0));
    }

    public static IEnumerator AnimationPlayer(SpriteRenderer renderer, Sprite[] sprites, float delay, int startIndex) {
        for (int i = startIndex; i < sprites.Length; i++) {
            if (i >= sprites.Length) i = 0; // Loop back to the start if we reach the end of the array
            renderer.sprite = sprites[i]; // Set the sprite to the current frame
            yield return new WaitForSeconds(delay); // Wait for the specified delay before moving to the next frame
        }
    }
}
