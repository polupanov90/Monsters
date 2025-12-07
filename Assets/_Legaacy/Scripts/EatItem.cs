using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class EatItem : MonoBehaviour {
    
    [SerializeField] private EatController eatController;
    Player player;
    
    private void Start() {
        player = Player.Instance;
    }
    public bool TryGetEatOneHandler() {
        if (player.forwardTransform == transform) {
            return eatController.GetEatOne();
        }
        return false;
    }
    public bool TryGetEatTwoHandler() {
        if (player.forwardTransform == transform) {
            return eatController.GetEatTwo();
        }
        return false;
    }
    public bool TryGetEatThreeHandler() {
        if (player.forwardTransform == transform) {
            return eatController.GetEatThree();
        }
        return false;
    }
}
