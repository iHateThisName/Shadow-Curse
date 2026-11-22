using System.Collections;
using UnityEngine;

namespace ShadowCurse.LightPuzzle {
    /// <summary>
    /// A two-state mirror that plugs into the project's existing Space interaction system.
    /// </summary>
    public sealed class PuzzleMirror : MonoBehaviour, IInteractable {
        [SerializeField] private bool slashOrientation = true;
        [SerializeField, Min(0.01f)] private float rotationDuration = 0.16f;

        private LightPuzzleController controller;
        private Coroutine rotationRoutine;

        // IInteractable requires this property. For a mirror it represents the slash state.
        public bool IsOn => slashOrientation;
        public bool IsSlashOrientation => slashOrientation;

        private void Awake() {
            controller = GetComponentInParent<LightPuzzleController>();
            EnsureInteractionCollider();
            SnapToOrientation();
        }

        public void Interact() {
            if (rotationRoutine != null) return;

            slashOrientation = !slashOrientation;
            controller ??= GetComponentInParent<LightPuzzleController>();
            controller?.RefreshBeam();
            rotationRoutine = StartCoroutine(RotateToCurrentOrientation());
        }

        public Vector2Int Reflect(Vector2Int incomingDirection) {
            if (slashOrientation) {
                if (incomingDirection == Vector2Int.up) return Vector2Int.right;
                if (incomingDirection == Vector2Int.right) return Vector2Int.up;
                if (incomingDirection == Vector2Int.down) return Vector2Int.left;
                return Vector2Int.down;
            }

            if (incomingDirection == Vector2Int.up) return Vector2Int.left;
            if (incomingDirection == Vector2Int.left) return Vector2Int.up;
            if (incomingDirection == Vector2Int.down) return Vector2Int.right;
            return Vector2Int.down;
        }

        public void ConfigureInitialOrientation(bool useSlashOrientation) {
            slashOrientation = useSlashOrientation;
            SnapToOrientation();
        }

        private IEnumerator RotateToCurrentOrientation() {
            Quaternion start = transform.localRotation;
            Quaternion target = TargetRotation();
            float elapsed = 0f;

            while (elapsed < rotationDuration) {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(elapsed / rotationDuration));
                transform.localRotation = Quaternion.Slerp(start, target, t);
                yield return null;
            }

            transform.localRotation = target;
            rotationRoutine = null;
        }

        private void SnapToOrientation() {
            transform.localRotation = TargetRotation();
        }

        private Quaternion TargetRotation() {
            return Quaternion.Euler(0f, 0f, slashOrientation ? 45f : -45f);
        }

        private void EnsureInteractionCollider() {
            Collider2D mirrorCollider = GetComponent<Collider2D>();
            if (mirrorCollider == null) {
                CircleCollider2D circle = gameObject.AddComponent<CircleCollider2D>();
                circle.radius = 1.25f;
                mirrorCollider = circle;
            }

            mirrorCollider.isTrigger = true;
        }
    }
}
