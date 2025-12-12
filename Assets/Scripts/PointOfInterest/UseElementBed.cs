using UnityEngine;

public class UseElementBed: AbstractUseElement {
    public override void Use(Child child) {
        child.agent.enabled = false;
        child.transform.position += Vector3.up;
        child.isJump = true;
    }
    public override void Exit(Child child) {
        child.transform.position = transform.position;
        child.isJump = false;
        child.agent.enabled = true;
        
    }
    
}