using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractorController : MonoBehaviour {
    [SerializeField] private InputActionReference interactionActionRefrence;
    private List<IInteractable> interactables = new List<IInteractable>(); // Collection of interactable that are currently in range of the interactor

    private void OnEnable() {
        this.interactionActionRefrence.action.Enable();
        this.interactionActionRefrence.action.performed += OnInteraction;
    }

    private void OnDisable() {
        this.interactionActionRefrence.action.performed -= OnInteraction;
        this.interactionActionRefrence.action.Disable();
    }

    private void OnTriggerEnter(Collider other) {
        other.TryGetComponent(out IInteractable interactable);

        if (interactable == null) {
            interactable = other.GetComponentInParent<IInteractable>();
            if (interactable == null) {
                interactable = other.GetComponentInChildren<IInteractable>();
            }
        }

        if (interactable != null) {
            interactables.Add(interactable);
        }
    }

    private void OnTriggerExit(Collider other) {
        other.TryGetComponent(out IInteractable interactable);
        if (interactable == null) {
            interactable = other.GetComponentInParent<IInteractable>();
            if (interactable == null) {
                interactable = other.GetComponentInChildren<IInteractable>();
            }
        }
        if (interactable != null) {
            interactables.Remove(interactable);
        }
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
}
