using UnityEngine;

public class PointOfInterestUse : MonoBehaviour {
    public bool isUsed;
    public Child usedChild;
    public int order;
    
    public void StartUsing(Child child) {
        usedChild = child;
        isUsed = true;
    }
    public void StopUsing() {
        isUsed = false;
    }
    private void OnDrawGizmos() {
        Gizmos.color = new Color(0f, 0f, 1f, 1);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 4);
    }
}
