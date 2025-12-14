using System;
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    
    
    // public void SetNextWayPoint(WayPoint _nextWayPoint) {
    //     nextWayPoint =  _nextWayPoint;
    // }
    
    public WayPoint nextWayPoint;


    private void SetNextWayPoint() {
        WayPoint[] wayPoints = GetComponentInParent<ChildWay>().gameObject.GetComponentsInChildren<WayPoint>();
        if (wayPoints.Length > 0) {
            for (int i = 0; i < wayPoints.Length; i++) {
                if (i < wayPoints.Length -1 && wayPoints[i] == this && wayPoints[i + 1] != null) {
                    nextWayPoint = wayPoints[i + 1];
                }
            }
        }
        
    }
    
    private void OnDrawGizmos() {
        SetNextWayPoint();
        if (nextWayPoint) {
            Gizmos.color = Color.purple;
            Gizmos.DrawLine(transform.position + Vector3.up/10, nextWayPoint.transform.position + Vector3.up/10);
        }
    }
}
