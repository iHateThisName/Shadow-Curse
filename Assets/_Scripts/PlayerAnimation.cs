using UnityEngine;

public class PlayerAnimation : MonoBehaviour {

    public static readonly string WalkFrontTrigger = "WalkFrontTrigger";
    public static readonly string WalkRightTrigger = "WalkRightTrigger";
    public static readonly string WalkLeftTrigger = "WalkLeftTrigger";
    public static readonly string WalkBackTrigger = "WalkBackTrigger";
    public static readonly string IdleTrigger = "IdleTrigger";
    public static readonly string MirrorFrontTrigger = "MirrorFrontTrigger";
    public static readonly string MirrorRightTrigger = "MirrorRightTrigger";
    public static readonly string MirrorLeftTrigger = "MirrorLeftTrigger";
    public static readonly string MirrorBackTrigger = "MirrorBackTrigger";
    public static readonly string DieTrigger = "DieTrigger";

    public static PlayerAnimation Instance { get; private set; }

    [SerializeField] private Animator animator;
    public void SetTrigger(string triggerName) {
        Debug.Log($"Setting trigger: {triggerName}");
        this.animator.SetTrigger(triggerName);
    }
}
