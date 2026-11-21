using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour {

    [SerializeField] private InputActionReference moveInputRefrence;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 moveInput;

    [SerializeField] private float moveSpeed = 5f;


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
    }

    private void FixedUpdate() {
        this.rb.linearVelocity = new Vector2(this.moveInput.x * this.moveSpeed, this.moveInput.y * this.moveSpeed);
    }
}
