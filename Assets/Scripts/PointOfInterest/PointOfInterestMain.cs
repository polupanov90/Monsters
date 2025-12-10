using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class PointOfInterestMain : MonoBehaviour {
    public PointOfInterestUse[] usePoints;
    public PointOfInterestAwait[] awaitPoints;
    public List<Child> childLoop;
    public int useTime = 4;
    

    public void GetInLine(Child child) {
        childLoop.Add(child);
    }
    public void OutInLine(Child child) {
        childLoop.RemoveAll((_child) => _child == child);
    }

    public bool CheckFirstPlaceInLoop(Child child) {
        return childLoop[0] == child;
    }

    public bool TryGetEmptyUsePoint([CanBeNull] out PointOfInterestUse  usePoint) {
        PointOfInterestUse[] emptyUsePoints = Array.FindAll(usePoints, _usePoint => !_usePoint.isUsed);
        if (emptyUsePoints.Length > 0) {
            Array.Sort(emptyUsePoints, (usePoint1, usePoint2) => {
                return usePoint1.order > usePoint2.order ? 1 : -1;
            });
            usePoint = emptyUsePoints[0];
            return true;
        }
        usePoint = null;
        return false; 
    }
    
    public bool TryGetEmptyAwaitPoint([CanBeNull] out PointOfInterestAwait  awaitPoint) {
        PointOfInterestAwait[] emptyAwaitPoints = Array.FindAll(awaitPoints, _awaitPoint => !_awaitPoint.isUsed);
        if (emptyAwaitPoints.Length > 0) {
            Array.Sort(emptyAwaitPoints, (awaitPoint1, awaitPoint2) => {
                return awaitPoint1.order > awaitPoint2.order ? 1 : -1;
            });
            awaitPoint = emptyAwaitPoints[0];
            return true;
        }
        awaitPoint = null;
        return false; 
    }
    
    
    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0f, 0f, 1);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 6);
    }
}
