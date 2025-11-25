using System;
using UnityEngine;

public class Player : Character {
    [SerializeField] private InputController inputController;
    
    public bool isMoving = false;
    
    private void Update() {
        SetIsMoving();
        InnerMove();
    }

    private void SetIsMoving() {
        isMoving = inputController.movementVector != Vector2.zero;
    }
    private void InnerMove() {
        if (isMoving) {
            Move(inputController.movementVector);
            Rotate(inputController.movementVector);
        }
    }
}
