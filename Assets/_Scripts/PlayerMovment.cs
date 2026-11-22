using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour {

    [SerializeField] private InputActionReference moveInputRefrence;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 moveInput;

    [SerializeField] private float moveSpeed = 5f;
    [field: SerializeField] public bool IsFacingRight { get; private set; } = true;
    [SerializeField] private PlayerAnimation playerAnimation;

    public DirectionEnum currentDirection { get; private set; } = DirectionEnum.None;
    public DirectionEnum previousDirection { get; private set; } = DirectionEnum.None;


    private void OnEnable() {
        moveInputRefrence.action.Enable();
        moveInputRefrence.action.performed += OnMove;
        moveInputRefrence.action.canceled += OnMove;
    }


    private void OnDisable() {
        moveInputRefrence.action.performed -= OnMove;
        moveInputRefrence.action.canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context) {
        this.moveInput = context.ReadValue<Vector2>();
        this.previousDirection = this.currentDirection;
        if (this.moveInput.x > 0) {
            this.IsFacingRight = true;
            this.currentDirection = DirectionEnum.Right;
            this.playerAnimation.SetTrigger(PlayerAnimation.WalkRightTrigger);

        } else if (this.moveInput.x < 0) {
            this.IsFacingRight = false;
            this.currentDirection = DirectionEnum.Left;
            this.playerAnimation.SetTrigger(PlayerAnimation.WalkLeftTrigger);

        } else if (this.moveInput.y > 0) {
            this.currentDirection = DirectionEnum.Up;
            this.playerAnimation.SetTrigger(PlayerAnimation.WalkBackTrigger);

        } else if (this.moveInput.y < 0) {
            this.currentDirection = DirectionEnum.Down;
            this.playerAnimation.SetTrigger(PlayerAnimation.WalkFrontTrigger);

        } else if (this.moveInput.x == 0 && this.moveInput.y == 0) {
            this.currentDirection = DirectionEnum.None;
            this.playerAnimation.SetTrigger(PlayerAnimation.IdleTrigger);
        }
    }

    private void Update() {
        this.rb.linearVelocity = new Vector2(this.moveInput.x * this.moveSpeed, this.moveInput.y * this.moveSpeed);
    }

    public enum DirectionEnum {
        None,
        Up,
        Down,
        Left,
        Right
    }
}
