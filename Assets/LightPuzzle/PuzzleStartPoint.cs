using UnityEngine;
using UnityEngine.Events;

namespace ShadowCurse.LightPuzzle {
    /// <summary>
    /// Consumes one of the player's light charges and starts the mirror beam.
    /// Uses the same Space interaction path as the mirrors.
    /// </summary>
    public sealed class PuzzleStartPoint : MonoBehaviour, IInteractable {
        [SerializeField] private LightPuzzleController controller;
        [SerializeField] private SpriteRenderer indicator;
        [SerializeField] private Color inactiveColor = Color.white;
        [SerializeField] private Color activeColor = new Color(1f, 0.78f, 0.2f, 1f);
        [SerializeField] private bool isActivated;
        [SerializeField] private UnityEvent onActivated = new UnityEvent();

        public bool IsOn => isActivated;

        private void Awake() {
            controller ??= GetComponentInParent<LightPuzzleController>();
            indicator ??= GetComponent<SpriteRenderer>();
            EnsureInteractionCollider();
            UpdateVisual();
        }

        private void Start() {
            controller?.SetBeamActive(isActivated);
        }

        public void Interact() {
            if (isActivated) return;
            if (GameManager.Instance == null) {
                Debug.LogWarning("StartPoint could not find the GameManager, so it cannot spend a light charge.", this);
                return;
            }

            if (!GameManager.Instance.SpendLight()) {
                Debug.Log("A light charge is required to activate the mirror puzzle.", this);
                return;
            }

            Activate();
        }

        public void Configure(LightPuzzleController puzzleController) {
            controller = puzzleController;
            indicator ??= GetComponent<SpriteRenderer>();
            isActivated = false;
            UpdateVisual();
        }

        private void Activate() {
            isActivated = true;
            UpdateVisual();
            controller?.SetBeamActive(true);
            onActivated?.Invoke();
        }

        private void UpdateVisual() {
            if (indicator != null) indicator.color = isActivated ? activeColor : inactiveColor;
        }

        private void EnsureInteractionCollider() {
            Collider2D startCollider = GetComponent<Collider2D>();
            if (startCollider == null) {
                CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
                circle.radius = 1.25f;
                startCollider = circle;
            }
            startCollider.isTrigger = true;
        }
    }
}
