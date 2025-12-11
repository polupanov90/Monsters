using System;
using JetBrains.Annotations;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class Child : MonoBehaviour {
    [SerializeField] private PointOfInterestMain[] pointsOfInterest;
    [SerializeField] private float awaitTime = 4f;
    
    private PointOfInterestMain pointOfInterestMain;
    private PointOfInterestMain lastPointOfInterestMain;
    private PointOfInterestAwait pointOfInterestAwait;
    private PointOfInterestUse pointOfInterestUse;
    public NavMeshAgent agent;
    
    private void Awake() {
        InitiateNavMeshAgent();
    }
    private void Start() {
        SomethingToDo();
    }
    private void Update() {
        StopMove();
    }

    private void StopMove() {
        if (agent.hasPath && agent.remainingDistance < 0.1f && pointOfInterestAwait) {
            BeginAwaitPointToInterest();
        }
        if (agent.hasPath && agent.remainingDistance < 0.1f && pointOfInterestUse && pointOfInterestUse.status == PointOfInterestUsedStatusEnum.Employed) {
           ReadyUsePointToInterest();
        }
    }
    
    private void InitiateNavMeshAgent() {
        agent = GetComponent<NavMeshAgent>();
    }
    
    public void SetDestination(Vector3 pos) {
        agent.SetDestination(pos);
    }
    public  void ClearDestination() {
        agent.ResetPath();
    }

    private void SomethingToDo() {
        if (!TrySetPointOfInterestAwait()) {
            Debug.Log("don't thing to do3");
            SetDestination(Vector3.zero);
        }
    }
    
    private bool TrySetPointOfInterestAwait() {
        if (TryGetRandomPointToInterestMainWithEmptyPointToWait(out PointOfInterestMain _pointOfInterestMain, out PointOfInterestAwait _pointOfInterestAwait)) {
            pointOfInterestMain = _pointOfInterestMain;
            pointOfInterestAwait = _pointOfInterestAwait;
            _pointOfInterestAwait.StartUsing(this);
            agent.SetDestination(_pointOfInterestAwait.transform.position);
            return true;
        }
        
        return false;
    }
    
    private bool TryGetRandomPointToInterestMainWithEmptyPointToWait(
        [CanBeNull] out PointOfInterestMain _pointOfInterestMain,
        [CanBeNull] out PointOfInterestAwait _pointOfInterestAwait
    ) {
        PointOfInterestMain[] emptyPointsOfInterest = Array.FindAll(pointsOfInterest, _pointOfInterest => _pointOfInterest.TryGetEmptyAwaitPoint(out _));
      
        // если на этой точке был недавно и есть другая точка то исключаем точку на которой был недавно
        if (emptyPointsOfInterest.Length >= 2) {
            emptyPointsOfInterest = Array.FindAll(emptyPointsOfInterest, pointOfInterest => pointOfInterest != lastPointOfInterestMain);
        }
        
        if (emptyPointsOfInterest.Length > 0) {
            int index = new System.Random().Next(0, emptyPointsOfInterest.Length);
            _pointOfInterestMain =  emptyPointsOfInterest[index];
            _pointOfInterestMain.TryGetEmptyAwaitPoint(out _pointOfInterestAwait);
            
            return true;
        }

        _pointOfInterestAwait = null;
        _pointOfInterestMain = null;
        return false;
    }
    
    private void ChangePointOfInterestLoop(object sender, LoopChangeArguments loopChangeArguments) {
        if (loopChangeArguments.child == this) {
            pointOfInterestUse = loopChangeArguments.pointOfInterestUse;
            pointOfInterestUse.EmployUsing(this);
            pointOfInterestMain.OutInLine(this);
            pointOfInterestMain.OnLoopChange -= ChangePointOfInterestLoop;  
            CancelInvoke(nameof(EndAwaitPointToInterestAndSomethingToDo));
            EndAwaitPointToInterest();
            SetDestination(pointOfInterestUse.transform.position);
            
        }
    }

    private void BeginAwaitPointToInterest() {
        ClearDestination();
        if (pointOfInterestMain) {
            pointOfInterestMain.GetInLine(this);
            pointOfInterestMain.OnLoopChange += ChangePointOfInterestLoop;
        }
        
        CancelInvoke(nameof(EndAwaitPointToInterestAndSomethingToDo));
        Invoke(nameof(EndAwaitPointToInterestAndSomethingToDo), awaitTime);
    }


    private void EndAwaitPointToInterestAndSomethingToDo() {
        EndAwaitPointToInterest();
        pointOfInterestMain = null;
        SomethingToDo();
    }
    private void EndAwaitPointToInterest() {
        if (pointOfInterestAwait) {
            pointOfInterestAwait.StopUsing();
            pointOfInterestAwait = null;
        }

        if (pointOfInterestMain) {
            pointOfInterestMain.OutInLine(this);
            lastPointOfInterestMain = pointOfInterestMain;
        }
    }

    

    private void ReadyUsePointToInterest() {
        ClearDestination();
        if (pointOfInterestMain && pointOfInterestUse) {
            pointOfInterestUse.ReadyUsing(this);
            pointOfInterestMain.OnStartUse += UsePointToInterestStartHandler;
        }
    }
    
    private void UsePointToInterestStartHandler(object sender, EventArgs args) {
        if (pointOfInterestUse) {
            ClearDestination();
            pointOfInterestMain.OnStartUse -= UsePointToInterestStartHandler;
            pointOfInterestUse.StartUsing(this);
            if (pointOfInterestMain.startUseIfAllUsePointsIsUsed) {
                pointOfInterestMain.OnStopUse += UsePointToInterestStopHandler;
            } else {
                CancelInvoke(nameof(UsePointToInterestStopAndSomethingToDo));
                Invoke(nameof(UsePointToInterestStopAndSomethingToDo), pointOfInterestMain.useTime);
            }
        }
    }

    private void UsePointToInterestStopHandler(object sender, EventArgs args) {
        pointOfInterestMain.OnStopUse -= UsePointToInterestStopHandler;
        UsePointToInterestStop();
        SomethingToDo();
    }

    private void UsePointToInterestStopAndSomethingToDo() {
        UsePointToInterestStop();
        SomethingToDo();
    }
    private void UsePointToInterestStop() {
        if (pointOfInterestUse) {
            pointOfInterestUse.StopUsing();
            pointOfInterestUse = null;
            pointOfInterestMain = null;
        }
    }
    
}
 