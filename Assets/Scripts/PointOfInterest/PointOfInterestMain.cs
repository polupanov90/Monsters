using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class LoopChangeArguments {
    public  LoopChangeArguments(Child child, PointOfInterestUse pointOfInterestUse) {
        this.child = child;
        this.pointOfInterestUse = pointOfInterestUse;
    }
    public Child child;
    public PointOfInterestUse pointOfInterestUse;
    
}

public class PointOfInterestMain : MonoBehaviour {
    public PointOfInterestUse[] usePoints;
    public PointOfInterestAwait[] awaitPoints;
    public LoockPoint lookPoint;
    public List<Child> childLoop;
    public bool startUseIfAllUsePointsIsUsed;
    public int useTime = 4;
    
    public event EventHandler OnStartUse;
    public event EventHandler OnStopUse;
    public event EventHandler<LoopChangeArguments> OnLoopChange;
    

    private void StopUseInvoke() {
        OnStopUse?.Invoke(this, EventArgs.Empty);
    }

    private void Update() {
        if (OnStartUse != null) {
            if (startUseIfAllUsePointsIsUsed) {
                if (usePoints.All(usePoint => usePoint.status == PointOfInterestUsedStatusEnum.Ready)) {
                    OnStartUse.Invoke(this, EventArgs.Empty);
                    if (OnStopUse != null) {
                        CancelInvoke(nameof(StopUseInvoke));
                        Invoke(nameof(StopUseInvoke), useTime);   
                    }
                }
            } else {
                OnStartUse.Invoke(this, EventArgs.Empty);
            }
        }

        if (OnLoopChange != null && childLoop.Count > 0) {
            PointOfInterestUse[] emptyUsePoints = Array.FindAll(usePoints, usePoint => usePoint.status == PointOfInterestUsedStatusEnum.Empty);
            if (emptyUsePoints.Length > 0) {
                Array.Sort(emptyUsePoints, (usePoint1, usePoint2) => usePoint1.order > usePoint2.order ? 1 : -1);
                OnLoopChange.Invoke(this, new LoopChangeArguments(childLoop[0], emptyUsePoints[0]));
            }  
        }
    }


    // Встать в очередь
    public void GetInLine(Child child) {
        childLoop.Add(child);
    }
    
    // Выйти их очереди
    public void OutInLine(Child child) {
        childLoop.RemoveAll((_child) => _child == child);
    }

    // Проврека, является ли переданный ребёнок первым в очереди
    public bool CheckFirstPlaceInLoop(Child child) {
        return childLoop[0] == child;
    }

    // Получить пустую точку использования
    public bool TryGetEmptyUsePoint([CanBeNull] out PointOfInterestUse  usePoint) {
        PointOfInterestUse[] emptyUsePoints = Array.FindAll(usePoints, _usePoint => _usePoint.status == PointOfInterestUsedStatusEnum.Empty);
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
    
    // Получить пустую точку ожидания
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
