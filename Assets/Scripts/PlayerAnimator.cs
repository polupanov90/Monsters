using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    [SerializeField] Player player;
    
    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        Animate();
    }

    public void Animate() {
        Debug.Log(player.isMoving);
        animator.SetBool("IsMove", player.isMoving);
    }

}
