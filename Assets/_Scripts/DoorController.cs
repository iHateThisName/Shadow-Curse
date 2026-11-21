using UnityEngine;

public class DoorController : MonoBehaviour {

    [SerializeField] private GameObject open;
    [SerializeField] private GameObject closed;
    [field: SerializeField] public bool IsOpen { get; private set; } = false;

    [SerializeReference] private LightSouceController[] OnInteractables; // Array to hold references to IInteractable objects where all must be on for the door to open


    private void Start() {
        CheckIfCanOpen(); // Check the initial state of the door based on the interactables
    }
    public void CheckIfCanOpen() {
        foreach (var interactable in OnInteractables) {
            if (!interactable.IsOn) {
                CloseDoor();
                return;
            }
        }
        OpenDoor();
        return;
    }

    private void OpenDoor() {
        IsOpen = true;
        open.SetActive(true);
        closed.SetActive(false);
    }
    private void CloseDoor() {
        IsOpen = false;
        open.SetActive(false);
        closed.SetActive(true);
    }

}
