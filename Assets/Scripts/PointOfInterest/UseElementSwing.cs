using UnityEngine;

public class UseElementSwing: AbstractUseElement {
    [SerializeField] Transform sweengChair;
    [SerializeField] Animator sweengAnimator;
    
    public override void Use(Child child) {
        child.agent.enabled = false;
        child.transform.SetParent(sweengChair);
        child.transform.localPosition = Vector3.zero;
        
        
        sweengAnimator.SetBool("isActive", true);
        
    }
    public override void Exit(Child child) {
        child.transform.parent = null;
        child.transform.position = transform.position;
        child.agent.enabled = true;
        sweengAnimator.SetBool("isActive", false);
    }
    
}