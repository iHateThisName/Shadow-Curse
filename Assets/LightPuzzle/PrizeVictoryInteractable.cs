using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowCurse.LightPuzzle {
    /// <summary>Loads the victory scene when the player interacts with the prize.</summary>
    public sealed class PrizeVictoryInteractable : MonoBehaviour, IInteractable {
        [SerializeField] private string victorySceneName = "VICTORY";

        public bool IsOn => true;

        private void Awake() {
            Collider2D interactionCollider = GetComponent<Collider2D>();
            if (interactionCollider == null) {
                CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
                circle.radius = 1.25f;
                interactionCollider = circle;
            }

            interactionCollider.isTrigger = true;
        }

        public void Interact() {
            SceneManager.LoadScene(victorySceneName);
        }
    }
}
