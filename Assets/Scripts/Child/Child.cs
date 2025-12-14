using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = System.Random;


public class Child : MonoBehaviour {
    [SerializeField] private int lookRadius = 10;
    [SerializeField] private PointOfInterestMain[] pointsOfInterest;
    [SerializeField] private ChildWay[] ways;
    [SerializeField] private Toy[] toys;
    
    [SerializeField] private Animator animator;
    [SerializeField] private float awaitTime = 4f;
    
    public Toy toy;
    public ChildWay way;
    public WayPoint nextWayPoint;
    public LookPoint lookPoint;
    
    public PointOfInterestMain pointOfInterestMain;
    private PointOfInterestMain lastPointOfInterestMain;
    private PointOfInterestAwait pointOfInterestAwait;
    private PointOfInterestUse pointOfInterestUse;
    public NavMeshAgent agent;

    private bool gamedWithToy;
    
    public bool isSit;
    public bool isJump;
    public bool isSitGame;
    
    private void Awake() {
        InitiateNavMeshAgent();
    }
    
    private void Start() {
        SomethingToDo();
    }
    
    private void Update() {
        StopMove();
        Animate();
        RotateToPointOfInterestMain();
        RotateToLookPoint();
    }

    private bool TryGetLookPoint(out LookPoint _lookPoint) {
        LookPoint[] lookPoints =  Array.FindAll(FindObjectsOfType<LookPoint>(), (lookPoint) => Vector3.Distance(lookPoint.transform.position, transform.position) < lookRadius) ;
        if (lookPoints.Length > 0) {
            int randomIndex = new Random().Next(0, lookPoints.Length);
            _lookPoint = lookPoints[randomIndex];
            return true;
        }
        _lookPoint = null;
        return false;
    }

    private void BeginLookPoint() {
        if (TryGetLookPoint(out LookPoint _lookPoint)) {
            lookPoint = _lookPoint;
            ClearDestination();
            Debug.Log("BeginLookPoint - " + gameObject.name);
            Invoke(nameof(MoveToNextWayPoint), 10);
        }
    }
    private void SitDownAndGame() {
        isSitGame = true;
        ClearDestination();
        Debug.Log("SitDownAndGame - " + gameObject.name);
        Invoke(nameof(MoveToNextWayPoint), 20);
    }

    private void ChooseWhatToDoInWayPoint() {
        int index = new Random().Next(0, way.waypoints.Length);
        if (index <= 1) {
            SomethingToDoInWayPoint();
        } else {
            Debug.Log("ChooseWhatToDoInWayPoint - " + gameObject.name);
            MoveToNextWayPoint();
        }
    }

    private void SomethingToDoInWayPoint() {
        int index = new Random().Next(0, 2);
        if (index == 0) {
            BeginLookPoint(); 
        }
        SitDownAndGame();
    }

    private void MoveToNextWayPoint() {
        Debug.Log("MoveToNextWayPoint - " + gameObject.name);
        isSitGame = false;
        lookPoint = null;
        int currentWayPointIndex = Array.FindIndex(way.waypoints, point => point == nextWayPoint);
        if (currentWayPointIndex + 1 < way.waypoints.Length) {
            nextWayPoint =  way.waypoints[currentWayPointIndex + 1];
            agent.SetDestination(nextWayPoint.transform.position);
            agent.autoBraking = false;
        } else {
            PutToy();
        }

    } 
    
    private bool TryGoToWay() {
        ChildWay[] emptyWays =  Array.FindAll(ways, way => way.status == WayStatusEnum.Empty);
        if (emptyWays != null &&  emptyWays.Length > 0) {
            int index = new System.Random().Next(0, emptyWays.Length);
            way = emptyWays[index];
            way.status = WayStatusEnum.Employed;
            nextWayPoint = way.firstWayPoint;
            agent.SetDestination(way.firstWayPoint.transform.position);
            agent.autoBraking = false;
            return  way != null;
        }
        return false;
    }
    
    private void TakeToy() {
        ClearDestination();
        Transform[] components =  GetComponentsInChildren<Transform>();
        Transform hearTransform =  Array.Find(components, (component) => component.gameObject.name == "кисть.L_end").gameObject.transform;
        toy.transform.SetParent(hearTransform);
        toy.transform.localPosition = Vector3.zero;
        toy.status = ToyStatusEnum.Used;
        if (!TryGoToWay()) {
            PutToy();
        }
    }

    public void PutToy() {
        if (toy) {
            toy.transform.parent = null;
            toy.transform.position = transform.position +  Vector3.forward;
            toy.status = ToyStatusEnum.Empty;
            toy = null;
        }
        if (way) {
            way.status = WayStatusEnum.Empty;    
            
            Debug.Log("way = null - " + gameObject.name);
            way = null;
        }

        lookPoint = null;
        nextWayPoint = null;
        agent.autoBraking = true;
        gamedWithToy = true;
        SomethingToDo();
    }
    
    private bool TryEmployToy() {
        Toy[] emptyToys =  Array.FindAll(toys, toy => toy.status == ToyStatusEnum.Empty);
        if (emptyToys != null &&  emptyToys.Length > 0) {
            int index = new System.Random().Next(0, emptyToys.Length);
            toy = emptyToys[index];
            toy.status = ToyStatusEnum.Employed;
            agent.SetDestination(toy.transform.position);
            return  toy != null;
        }

        return false;
    }
    
    private void StopMove() {
        if (toy && toy.status == ToyStatusEnum.Employed && agent.hasPath && agent.remainingDistance < 0.1f) {
            TakeToy(); 
        }
        if (nextWayPoint && agent.hasPath && agent.remainingDistance < 0.1f) {
            ChooseWhatToDoInWayPoint();
        }
        if (agent.hasPath && agent.remainingDistance < 0.1f && pointOfInterestAwait) {
            BeginAwaitPointToInterest();
        }
        if (agent.hasPath && agent.remainingDistance < 0.1f && pointOfInterestUse && pointOfInterestUse.status == PointOfInterestUsedStatusEnum.Employed) {
            ReadyUsePointToInterest();
        }
    }

    private void RotateToPointOfInterestMain() {
        if ((pointOfInterestUse && pointOfInterestUse.usedChild == this) || (pointOfInterestAwait && !agent.hasPath)) {
            Rotate(pointOfInterestMain.lookPoint.transform.position);
        }
    }
    private void RotateToLookPoint() {
        if (lookPoint) {
            Rotate(lookPoint.transform.position);
        }
    }

    private void Rotate(Vector3 rotateTarget) {
        Vector3 direction = rotateTarget - transform.position;
        direction.y = 0;
        if (direction.magnitude > 0.01f) {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10 * Time.deltaTime);
        }
    }

    private void Animate() {
        if (agent.hasPath) {
            animator.SetBool("isWalk", true);   
        } else {
            animator.SetBool("isWalk", false);
        }
        if (isSit) {
            animator.SetBool("isSit", true);
        } else {
            animator.SetBool("isSit", false);
        }
        if (isJump) {
            animator.SetBool("isJump", true);
        } else {
            animator.SetBool("isJump", false);
        }
        if (isSitGame) {
            animator.SetBool("isSitGame", true);
        } else {
            animator.SetBool("isSitGame", false);
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
        if (!gamedWithToy) {
            bool isToyGame = TryEmployToy();
            if (!isToyGame) {
                if (TrySetPointOfInterestAwait()) {
                    gamedWithToy = false;
                }
            }
        } else {
            if (TrySetPointOfInterestAwait()) {
                gamedWithToy = false;
            }
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
    
    // private void OnDrawGizmos() {
    //     Gizmos.color = new Color(0.8f, 0.8f, 0f, 0.3f);
    //     Gizmos.DrawSphere(transform.position, lookRadius);
    // }
}
 