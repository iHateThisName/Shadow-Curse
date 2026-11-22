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

            LightPuzzleController controller = root.GetComponent<LightPuzzleController>();
            if (controller == null) controller = Undo.AddComponent<LightPuzzleController>(root);
            Undo.RecordObject(controller, "Configure Light Puzzle");
            controller.Configure(lightPrefab);
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

            EditorSceneManager.MarkSceneDirty(scene);
            Debug.Log("Light puzzle installed on " + mirrorCount + " mirrors. Press Space near a mirror to rotate it.", root);
            Selection.activeGameObject = root;
        }
    }
}
#endif
