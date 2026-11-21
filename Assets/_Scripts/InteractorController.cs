using System.Collections.Generic;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorController : MonoBehaviour {
    [SerializeField] private InputAction interactionAction;
    private List<IInteractable> interactables = new List<IInteractable>(); // Collection of interactable that are currently in range of the interactor

    private void OnEnable() {
        this.interactionAction.Enable();
        this.interactionAction.performed += OnInteraction;
    }

    private void OnDisable() {
        this.interactionAction.performed -= OnInteraction;
        this.interactionAction.Disable();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        IInteractable interactable = GetInteractable(collision);

        if (interactable != null) {
            interactables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        IInteractable interactable = GetInteractable(collision);

        if (interactable != null) {
            interactables.Remove(interactable);
        }
    }

    [ContextMenu("Debug Available Interactions")]
    public void DebugAvaiableInteractions() {
        StringBuilder debugMessage = new StringBuilder();
        debugMessage.AppendLine($"Available Interactions: {interactables.Count}");
        foreach (IInteractable interactable in interactables) {
            if (interactable is MonoBehaviour monoBehaviour) {
                debugMessage.AppendLine($"- {monoBehaviour.gameObject.name}");
            } else {
                debugMessage.AppendLine($"- {interactable.GetType().Name}");
            }
        }
        Debug.Log(debugMessage.ToString());
    }
    private void OnInteraction(InputAction.CallbackContext context) {
        IInteractable closestInteractable = null;
        this.interactables.ForEach(interactable => {
            if (interactable != null) {
                if (interactable is MonoBehaviour monoBehaviour) {
                    if (monoBehaviour.gameObject.activeInHierarchy) {
                        if (closestInteractable == null) {
                            closestInteractable = interactable;
                        } else {
                            float currentDistance = Vector3.Distance(monoBehaviour.transform.position, this.transform.position);
                            float closestDistance = Vector3.Distance((closestInteractable as MonoBehaviour).transform.position, this.transform.position);
                            if (currentDistance < closestDistance) {
                                closestInteractable = interactable;
                            }
                        }
                    }
                }
            }
        });

        closestInteractable?.Interact();
    }

    private static IInteractable GetInteractable(Collider2D other) {
        other.TryGetComponent(out IInteractable interactable);
        if (interactable == null) {
            interactable = other.GetComponentInParent<IInteractable>();
            if (interactable == null) {
                interactable = other.GetComponentInChildren<IInteractable>();
            }
        }

        return interactable;
    }
}
