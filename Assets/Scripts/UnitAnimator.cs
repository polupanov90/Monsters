using UnityEngine;

public class UnitAnimator : MonoBehaviour {
    public Unit unit;
    private Animator animator;
    
    private void Awake() {
        InitializeAnimator();
    }

    private void Update() {
        Animate();
    }
    
    private void InitializeAnimator() {
        animator = GetComponent<Animator>();
    }
    
    private void Animate() {
        animator.SetBool("IsMove", unit.GetIsMove());
    }
}
