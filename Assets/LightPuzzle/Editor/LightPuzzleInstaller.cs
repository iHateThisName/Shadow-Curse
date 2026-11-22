#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowCurse.LightPuzzle.Editor {
    public static class LightPuzzleInstaller {
        private const string RootName = "RefractionPuzzle";
        private const string LightPrefabPath = "Assets/LightPuzzle/Light.prefab";
        private static readonly Vector2 BottomLeft = new Vector2(-15f, 6f);
        private const float Spacing = 5f;

        [MenuItem("Tools/Light Puzzle/Install Into Current Level3")]
        public static void Install() {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name != "Level3") {
                EditorUtility.DisplayDialog("Light Puzzle", "Open Level3 before running the installer.", "OK");
                return;
            }

            GameObject root = scene.GetRootGameObjects()
                .SelectMany(item => item.GetComponentsInChildren<Transform>(true))
                .FirstOrDefault(item => item.name == RootName)?.gameObject;

            if (root == null) {
                EditorUtility.DisplayDialog("Light Puzzle", "Could not find the RefractionPuzzle object.", "OK");
                return;
            }

            GameObject lightPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(LightPrefabPath);
            if (lightPrefab == null) {
                EditorUtility.DisplayDialog("Light Puzzle", "Could not load " + LightPrefabPath, "OK");
                return;
            }

            EnsureLevelGameManager(scene);

            LightPuzzleController controller = root.GetComponent<LightPuzzleController>();
            if (controller == null) controller = Undo.AddComponent<LightPuzzleController>(root);

            GameObject armObstacle = FindSceneObject(scene, "ArmObstacle");
            Undo.RecordObject(controller, "Configure Light Puzzle");
            controller.Configure(lightPrefab, armObstacle);
            EditorUtility.SetDirty(controller);

            int mirrorCount = 0;
            foreach (Transform child in root.GetComponentsInChildren<Transform>(true)) {
                if (!child.name.StartsWith("Mirror")) continue;

                PuzzleMirror mirror = child.GetComponent<PuzzleMirror>();
                if (mirror == null) mirror = Undo.AddComponent<PuzzleMirror>(child.gameObject);

                Collider2D interactionCollider = child.GetComponent<Collider2D>();
                if (interactionCollider == null) {
                    CircleCollider2D circle = Undo.AddComponent<CircleCollider2D>(child.gameObject);
                    circle.radius = 1.25f;
                    interactionCollider = circle;
                }
                interactionCollider.isTrigger = true;

                int column = Mathf.RoundToInt((child.localPosition.x - BottomLeft.x) / Spacing);
                int row = Mathf.RoundToInt((child.localPosition.y - BottomLeft.y) / Spacing);
                bool initialSlashOrientation = (column + row) % 2 == 0;

                Undo.RecordObject(mirror, "Configure Puzzle Mirror");
                Undo.RecordObject(child, "Orient Puzzle Mirror");
                mirror.ConfigureInitialOrientation(initialSlashOrientation);
                EditorUtility.SetDirty(mirror);
                EditorUtility.SetDirty(child);
                mirrorCount++;
            }

            GameObject startPoint = FindSceneObject(scene, "StartPoint");
            if (startPoint == null) {
                EditorUtility.DisplayDialog("Light Puzzle", "Could not find the StartPoint object.", "OK");
                return;
            }

            PuzzleStartPoint puzzleStart = startPoint.GetComponent<PuzzleStartPoint>();
            if (puzzleStart == null) puzzleStart = Undo.AddComponent<PuzzleStartPoint>(startPoint);

            Collider2D startCollider = startPoint.GetComponent<Collider2D>();
            if (startCollider == null) {
                CircleCollider2D circle = Undo.AddComponent<CircleCollider2D>(startPoint);
                circle.radius = 1.25f;
                startCollider = circle;
            }
            startCollider.isTrigger = true;

            Undo.RecordObject(puzzleStart, "Configure Puzzle Start Point");
            puzzleStart.Configure(controller);
            EditorUtility.SetDirty(puzzleStart);

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Light puzzle installed on " + mirrorCount + " mirrors. Activate StartPoint with one charge, then rotate mirrors with Space.", root);
            Selection.activeGameObject = root;
        }

        private static GameObject FindSceneObject(Scene scene, string objectName) {
            return scene.GetRootGameObjects()
                .SelectMany(item => item.GetComponentsInChildren<Transform>(true))
                .FirstOrDefault(item => item.name == objectName)?.gameObject;
        }

        private static void EnsureLevelGameManager(Scene scene) {
            GameManager manager = Object.FindFirstObjectByType<GameManager>(FindObjectsInactive.Include);
            if (manager == null) {
                GameObject managerObject = new GameObject("GameManager");
                Undo.RegisterCreatedObjectUndo(managerObject, "Create Level3 GameManager");
                SceneManager.MoveGameObjectToScene(managerObject, scene);
                manager = Undo.AddComponent<GameManager>(managerObject);
            }

            SerializedObject serializedManager = new SerializedObject(manager);
            SerializedProperty currentLights = serializedManager.FindProperty("<CurrentLights>k__BackingField");
            if (currentLights != null && currentLights.intValue < 1) {
                currentLights.intValue = 1;
                serializedManager.ApplyModifiedProperties();
                EditorUtility.SetDirty(manager);
            }
        }
    }
}
#endif
