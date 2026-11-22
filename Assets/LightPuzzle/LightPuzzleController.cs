using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShadowCurse.LightPuzzle {
    /// <summary>
    /// Traces the orthogonal beam across the existing seven-by-seven Level3 layout.
    /// Mirrors redirect it, obstacles stop it, and the existing Light prefab draws each segment.
    /// </summary>
    public sealed class LightPuzzleController : MonoBehaviour {
        [Header("Existing Level3 layout")]
        [SerializeField, Min(2)] private int gridSize = 7;
        [SerializeField, Min(0.01f)] private float cellSpacing = 5f;
        [SerializeField] private Vector2 bottomLeftNode = new Vector2(-15f, 6f);
        [SerializeField] private int startColumn = 0;
        [SerializeField] private int goalColumn = 6;

        [Header("Beam")]
        [SerializeField] private GameObject beamSegmentPrefab;
        [SerializeField, Min(0.01f)] private float beamThickness = 0.16f;
        [SerializeField] private Color beamColor = new Color(1f, 0.78f, 0.2f, 1f);
        [SerializeField] private int beamSortingOrder = -1;
        [SerializeField] private bool beamStartsActive = false;

        [Header("Completion")]
        [SerializeField] private GameObject armObstacle;
        [SerializeField, Min(0f)] private float armObstacleRemovalDelay = 4f;
        [SerializeField] private UnityEvent onPuzzleSolved = new UnityEvent();

        private readonly Dictionary<Vector2Int, PuzzleMirror> mirrors = new Dictionary<Vector2Int, PuzzleMirror>();
        private readonly HashSet<Vector2Int> obstacles = new HashSet<Vector2Int>();
        private Transform beamContainer;
        private bool wasSolved;
        private bool obstacleRemovalScheduled;

        public bool IsSolved { get; private set; }
        public bool IsBeamActive { get; private set; }

        private void Start() {
            IsBeamActive = beamStartsActive;
            RebuildNodeLookup();
            if (IsBeamActive) RefreshBeam();
            else ClearBeam();
        }

        [ContextMenu("Refresh Beam")]
        public void RefreshBeam() {
            if (!IsBeamActive) {
                IsSolved = false;
                ClearBeam();
                return;
            }

            RebuildNodeLookup();
            ClearBeam();

            List<Vector3> points = TraceBeam(out bool solved);
            DrawBeam(points);

            IsSolved = solved;
            if (IsSolved && !wasSolved) {
                Debug.Log("Light puzzle solved: the beam reached the receiver.", this);
                onPuzzleSolved?.Invoke();
                ScheduleArmObstacleRemoval();
            }
            wasSolved = IsSolved;
        }

        public void SetBeamActive(bool active) {
            IsBeamActive = active;
            beamStartsActive = active;
            wasSolved = false;

            if (active) RefreshBeam();
            else {
                IsSolved = false;
                ClearBeam();
            }
        }

        public void Configure(GameObject lightPrefab, GameObject armObstacleObject) {
            beamSegmentPrefab = lightPrefab;
            armObstacle = armObstacleObject;
        }

        private void ScheduleArmObstacleRemoval() {
            if (obstacleRemovalScheduled || armObstacle == null) return;
            obstacleRemovalScheduled = true;
            Debug.Log("Light puzzle complete. ArmObstacle will be removed in " + armObstacleRemovalDelay + " seconds.", armObstacle);
            Destroy(armObstacle, armObstacleRemovalDelay);
        }

        private void RebuildNodeLookup() {
            mirrors.Clear();
            obstacles.Clear();

            foreach (PuzzleMirror mirror in GetComponentsInChildren<PuzzleMirror>(true)) {
                Vector2Int cell = LocalPositionToCell(mirror.transform.localPosition);
                mirrors[cell] = mirror;
            }

            foreach (Transform child in GetComponentsInChildren<Transform>(true)) {
                if (child == transform) continue;
                if (!child.name.StartsWith("Obstacle")) continue;
                obstacles.Add(LocalPositionToCell(child.localPosition));
            }
        }

        private List<Vector3> TraceBeam(out bool solved) {
            solved = false;
            List<Vector3> points = new List<Vector3>();
            HashSet<string> visitedStates = new HashSet<string>();

            Vector2Int cell = new Vector2Int(startColumn, -1);
            Vector2Int direction = Vector2Int.up;
            points.Add(CellToWorld(cell));

            int safetyLimit = gridSize * gridSize * 4;
            for (int step = 0; step < safetyLimit; step++) {
                cell += direction;
                points.Add(CellToWorld(cell));

                if (!IsInsideGrid(cell)) {
                    solved = cell == new Vector2Int(goalColumn, gridSize) && direction == Vector2Int.up;
                    break;
                }

                if (obstacles.Contains(cell)) break;

                string state = cell.x + ":" + cell.y + ":" + direction.x + ":" + direction.y;
                if (!visitedStates.Add(state)) break;

                if (mirrors.TryGetValue(cell, out PuzzleMirror mirror)) {
                    direction = mirror.Reflect(direction);
                }
            }

            return points;
        }

        private void DrawBeam(List<Vector3> points) {
            if (beamSegmentPrefab == null) {
                Debug.LogWarning("LightPuzzleController needs the existing Assets/LightPuzzle/Light.prefab assigned.", this);
                return;
            }

            GameObject containerObject = new GameObject("Generated Beam");
            beamContainer = containerObject.transform;
            beamContainer.SetParent(transform, true);

            for (int i = 1; i < points.Count; i++) {
                CreateBeamSegment(points[i - 1], points[i], i);
            }
        }

        private void CreateBeamSegment(Vector3 start, Vector3 end, int index) {
            GameObject segment = Instantiate(beamSegmentPrefab, beamContainer);
            segment.name = "Beam Segment " + index;

            Vector3 difference = end - start;
            float length = difference.magnitude;
            segment.transform.position = (start + end) * 0.5f;
            segment.transform.rotation = Quaternion.identity;
            segment.transform.localScale = Mathf.Abs(difference.x) > Mathf.Abs(difference.y)
                ? new Vector3(length, beamThickness, 1f)
                : new Vector3(beamThickness, length, 1f);

            SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
            if (renderer != null) {
                renderer.color = beamColor;
                renderer.sortingOrder = beamSortingOrder;
            }
        }

        private void ClearBeam() {
            if (beamContainer == null) {
                Transform existing = transform.Find("Generated Beam");
                if (existing != null) beamContainer = existing;
            }

            if (beamContainer != null) {
                Destroy(beamContainer.gameObject);
                beamContainer = null;
            }
        }

        private Vector2Int LocalPositionToCell(Vector3 localPosition) {
            int column = Mathf.RoundToInt((localPosition.x - bottomLeftNode.x) / cellSpacing);
            int row = Mathf.RoundToInt((localPosition.y - bottomLeftNode.y) / cellSpacing);
            return new Vector2Int(column, row);
        }

        private Vector3 CellToWorld(Vector2Int cell) {
            Vector3 local = new Vector3(
                bottomLeftNode.x + cell.x * cellSpacing,
                bottomLeftNode.y + cell.y * cellSpacing,
                0f
            );
            return transform.TransformPoint(local);
        }

        private bool IsInsideGrid(Vector2Int cell) {
            return cell.x >= 0 && cell.x < gridSize && cell.y >= 0 && cell.y < gridSize;
        }
    }
}
