using UnityEngine;
using UnityEngine.Events;

public class EventEmitter : MonoBehaviour {
    [SerializeField] private UnityEvent<bool> OnEventTriggered;
    public void TriggerEventTrue() {
        OnEventTriggered?.Invoke(true);
    }

    public void TriggerEventFalse() {
        OnEventTriggered?.Invoke(false);
    }
}
