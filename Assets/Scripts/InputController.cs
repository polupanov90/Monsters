using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour {
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float rotateSpreed = 2;
    
    private InputAction moveAction;
    public Vector2 movementVector;
    
    private void OnEnable() {
        inputActionAsset.FindActionMap("Player").Enable();
    }

    private void OnDisable() {
        inputActionAsset.FindActionMap("Player").Disable();
    }

    private void Awake() {
        moveAction = inputActionAsset.FindAction("Move");
    }

    private void Update() {
        movementVector = moveAction.ReadValue<Vector2>();
    }
}
