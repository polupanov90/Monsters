using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour {
    [SerializeField] private InputActionAsset inputActionAsset;
    [SerializeField] private float moveSpeed = 2;
    [SerializeField] private float rotateSpreed = 2;
    
    public event EventHandler OnTurnOne;
    public event EventHandler OnTurnTwo;
    public event EventHandler OnTurnThree;
    public event EventHandler OnUse;
    
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
        inputActionAsset.FindAction("TurnOne").started += TurnOne;
        inputActionAsset.FindAction("TurnTwo").started += TurnTwo;
        inputActionAsset.FindAction("TurnThree").started += TurnThree;
        inputActionAsset.FindAction("Use").started += Use;
    }

    private void TurnOne(InputAction.CallbackContext context) {
        OnTurnOne?.Invoke(this, EventArgs.Empty);
    }
    private void TurnTwo(InputAction.CallbackContext context) {
        OnTurnTwo?.Invoke(this, EventArgs.Empty);
    }
    private void TurnThree(InputAction.CallbackContext context) {
        OnTurnThree?.Invoke(this, EventArgs.Empty);
    }
    private void Use(InputAction.CallbackContext context) {
        OnUse?.Invoke(this, EventArgs.Empty);
    }

    private void Update() {
        movementVector = moveAction.ReadValue<Vector2>().normalized;
    }
}
