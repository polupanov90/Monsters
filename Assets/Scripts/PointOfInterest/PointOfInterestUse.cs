using System;
using UnityEngine;

public class PointOfInterestUse : MonoBehaviour {
    public PointOfInterestUsedStatusEnum status = PointOfInterestUsedStatusEnum.Empty;
    public Child usedChild;
    public int order;
    private AbstractUseElement useElement;

    private void Awake() {
        useElement = GetComponent<AbstractUseElement>();
    }

    public void StartUsing(Child child) {
        usedChild = child;
        status = PointOfInterestUsedStatusEnum.Used;
        if (useElement) {
            useElement.Use(child);
        }
    }
    public void ReadyUsing(Child child) {
        usedChild = child;
        status = PointOfInterestUsedStatusEnum.Ready;
    }
    public void EmployUsing(Child child) {
        usedChild = child;
        status = PointOfInterestUsedStatusEnum.Employed;
    }

    public void StopUsing() {
        if (useElement) {
            useElement.Exit(usedChild);
        }
        usedChild = null;
        status =  PointOfInterestUsedStatusEnum.Empty;
    }
    private void OnDrawGizmos() {
        Gizmos.color = new Color(0f, 0f, 1f, 1);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 4);
    }
}
