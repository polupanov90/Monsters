using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class Child : MonoBehaviour {
    [SerializeField] private PointOfInterestMain[] pointsOfInterest;
    [SerializeField] private float awaitTime = 4f;
    
    private PointOfInterestMain pointOfInterestMain;
    private PointOfInterestMain lastPointOfInterestMain;
    private PointOfInterestAwait pointOfInterestAwait;
    private PointOfInterestUse pointOfInterestUse;
    private NavMeshAgent agent;
    
    public bool pointInterestAwaitInvoked; 
    public bool pointInterestUseInvoked; 
    
    private void Awake() {
        InitiateNavMeshAgent();
    }
    private void Start() {
        SomethingToDo();
    }
    private void Update() {
        StopMove();
        if (pointInterestAwaitInvoked) {
            TryGoToUsePointFromAwaitPoint();
        }
    }

    private void StopMove() {
        if (agent.hasPath && agent.remainingDistance < 0.1f && pointOfInterestAwait) {
            BeginAwaitPointToInterest();
        }
        if (agent.hasPath && agent.remainingDistance < 0.1f && pointOfInterestUse) {
            BeginUsePointToInterest();
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

    private void TryGoToUsePointFromAwaitPoint() {
        if (pointOfInterestMain && pointOfInterestMain.TryGetEmptyUsePoint(out PointOfInterestUse _pointOfInterestUse) && pointOfInterestMain.CheckFirstPlaceInLoop(this)) {
            pointOfInterestUse = _pointOfInterestUse;
            _pointOfInterestUse.StartUsing(this);
            CancelInvoke(nameof(EndAwaitPointToInterestAndSomethingToDo));   
            EndAwaitPointToInterest();
            SetDestination(_pointOfInterestUse.transform.position);
        }
    }
    
    private void BeginAwaitPointToInterest() {
        ClearDestination();
        if (pointOfInterestMain) {
            pointOfInterestMain.GetInLine(this);
        }
        if (pointInterestAwaitInvoked) {
            CancelInvoke(nameof(EndAwaitPointToInterestAndSomethingToDo));    
        }
        pointInterestAwaitInvoked = true;
        Invoke(nameof(EndAwaitPointToInterestAndSomethingToDo), awaitTime);
    }


    private void EndAwaitPointToInterestAndSomethingToDo() {
        EndAwaitPointToInterest();
        pointOfInterestMain = null;
        SomethingToDo();
    }
    private void EndAwaitPointToInterest() {
        pointInterestAwaitInvoked = false;
        pointOfInterestAwait.StopUsing();
        pointOfInterestMain.OutInLine(this);
        lastPointOfInterestMain = pointOfInterestMain;
        pointOfInterestAwait = null;
    }

    private void BeginUsePointToInterest() {
        ClearDestination();
        if (pointInterestUseInvoked) {
            CancelInvoke(nameof(EndUsePointToInterestAndSomethingToDo));    
        }
        if (pointOfInterestUse) {
            pointInterestUseInvoked = true;
            Invoke(nameof(EndUsePointToInterestAndSomethingToDo), pointOfInterestMain.useTime);
        }
    }

    private void EndUsePointToInterestAndSomethingToDo() {
        EndUsePointToInterest();
        SomethingToDo();
    }

    private void EndUsePointToInterest() {
        pointInterestUseInvoked = false;
        pointOfInterestUse.StopUsing();
        pointOfInterestUse = null;
        pointOfInterestMain = null;
    }
}
