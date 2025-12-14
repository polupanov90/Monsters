using System;
using UnityEngine;

public class ChildWay : MonoBehaviour
{
    public WayPoint[] waypoints;
    public WayStatusEnum status = WayStatusEnum.Empty;
    
    public WayPoint firstWayPoint;

    private void Awake() {
        InitWayPoints();
    }

    private void InitWayPoints() {
        waypoints = GetComponentsInChildren<WayPoint>();
        firstWayPoint = waypoints[0];
    }

}
