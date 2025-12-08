using UnityEngine;

public class PointOfInterest : MonoBehaviour {
    public int useTime = 8;
    public bool isUsed;
    public Child usedUnit;
    public int infoRadius = 5;
    public int interactRadius = 2;

    public void StartUsing() {
        isUsed = true;
    }

    public void StopUsing() {
        isUsed = false;
    }

    private void OnDrawGizmos() {

        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, infoRadius);
        Gizmos.color = new Color(0f, 0f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, interactRadius);
    }
}
