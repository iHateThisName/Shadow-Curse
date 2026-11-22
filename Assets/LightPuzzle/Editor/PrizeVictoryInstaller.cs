using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ShadowCurse.LightPuzzle.Editor {
    public static class PrizeVictoryInstaller {
        private const string VictoryScenePath = "Assets/Scenes/VICTORY.unity";

        [MenuItem("Tools/Light Puzzle/Add Victory Interaction To Prize")]
        public static void Install() {
            GameObject prize = FindSceneObject("Prize");
            if (prize == null) {
                Debug.LogError("Could not find a GameObject named Prize in the current scene.");
                return;
            }

            Undo.RecordObject(prize, "Add victory interaction to Prize");
            if (prize.GetComponent<PrizeVictoryInteractable>() == null) {
                Undo.AddComponent<PrizeVictoryInteractable>(prize);
            }

            Collider2D prizeCollider = prize.GetComponent<Collider2D>();
            if (prizeCollider == null) {
                CircleCollider2D circle = Undo.AddComponent<CircleCollider2D>(prize);
                circle.radius = 1.25f;
                prizeCollider = circle;
            }
            prizeCollider.isTrigger = true;

            EnsureVictorySceneIsBuildable();
            EditorSceneManager.MarkSceneDirty(prize.scene);
            Selection.activeGameObject = prize;
            Debug.Log("Prize now loads the VICTORY scene when interacted with.", prize);
        }

        private static GameObject FindSceneObject(string exactName) {
            Scene scene = SceneManager.GetActiveScene();
            foreach (GameObject root in scene.GetRootGameObjects()) {
                Transform[] transforms = root.GetComponentsInChildren<Transform>(true);
                Transform match = transforms.FirstOrDefault(t => t.name == exactName);
                if (match != null) return match.gameObject;
            }
            return null;
        }

        private static void EnsureVictorySceneIsBuildable() {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes.Any(scene => scene.path == VictoryScenePath && scene.enabled)) return;

            var updatedScenes = scenes.Where(scene => scene.path != VictoryScenePath).ToList();
            updatedScenes.Add(new EditorBuildSettingsScene(VictoryScenePath, true));
            EditorBuildSettings.scenes = updatedScenes.ToArray();
        }
    }
}
