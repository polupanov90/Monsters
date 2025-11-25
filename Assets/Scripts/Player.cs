using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private InputController inputController;

    private void Update() {
        Debug.Log(inputController.movementVector);
    }
}
