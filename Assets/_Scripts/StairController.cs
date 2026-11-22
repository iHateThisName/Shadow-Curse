using UnityEngine;

public class StairController : MonoBehaviour, IInteractable
{
    public bool IsOn => true;
    [SerializeField] private int loadSceneIndex = 0;

    public void Interact() {
        GameManager.Instance.loadSceneInIndex(this.loadSceneIndex);
    }
}
