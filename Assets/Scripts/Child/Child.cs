using UnityEngine;
using UnityEngine.AI;

public class Child : MonoBehaviour {
    [SerializeField] private PointOfInterest[] pointsOfInterest;
    [SerializeField] private int minIntervalAwaitPointToInterest = 4;
    [SerializeField] private int maxIntervalAwaitPointToInterest = 9;

    private bool isAwait;
    private NavMeshAgent agent;
    private PointOfInterest targetPointToInterest;

    private void Awake() {
        // инициализация агента навигации
        InitiateNavMeshAgent();
    }
    private void Start() {
        // инициализируем случайную точку интереса (идём к ней)
        InitiateMoveToRandomPointToInterest();
    }
    
    private void Update() {
        if (!isAwait) {
            // если мы дошли до радиуса получания информации о точке интереса
            if (CheckDistanceToInfoRadiusPointToInterest()) {
                // если точка интереса занята и не мной 
                if (targetPointToInterest.isUsed && targetPointToInterest.usedUnit != this) {
                    // остановиться
                    AwaitPointToInterest();
                    // если мы дошли до радиса взаимодействия
                } else if (CheckDistanceToInteractRadiusPointToInterest()) {
                    // остановиться
                    UsePointToInterest();
                }
            }
        }
        
    }

    private void AwaitPointToInterest() {
        if (!isAwait) {
            isAwait = true;
            ClearDestination();
            int awaitTime = new System.Random().Next(minIntervalAwaitPointToInterest, maxIntervalAwaitPointToInterest);
            Invoke(nameof(TryUsePointToInterest), awaitTime);   
        }
    }

    private void TryUsePointToInterest() {
        if (!targetPointToInterest.isUsed) {
            SetDestination(targetPointToInterest.transform.position);
        } else {
            InitiateMoveToRandomPointToInterest();
        }
        isAwait = false;
    }

    private void UsePointToInterest() {
        ClearDestination();
        transform.position = targetPointToInterest.transform.position;
        isAwait = true;
        targetPointToInterest.StartUsing();
        Invoke(nameof(LeavePointToInterest), targetPointToInterest.useTime);
       
    }
    private void LeavePointToInterest() {
        targetPointToInterest.StopUsing();
        isAwait = false;
        InitiateMoveToRandomPointToInterest();
    }

    private void InitiateNavMeshAgent() {
        agent = GetComponent<NavMeshAgent>();
    }

    private void InitiateMoveToRandomPointToInterest() {
        isAwait = false;
        targetPointToInterest = GetRandomPointOfInterest();
        SetDestination(targetPointToInterest.transform.position);
        
    }
    private PointOfInterest GetRandomPointOfInterest() {
        return pointsOfInterest[new System.Random().Next(0, pointsOfInterest.Length)];
    }

    private bool CheckDistanceToInfoRadiusPointToInterest() {
        return Vector3.Distance(transform.position, targetPointToInterest.transform.position) <= targetPointToInterest.infoRadius;
    }
    private bool CheckDistanceToInteractRadiusPointToInterest() {
        return Vector3.Distance(transform.position, targetPointToInterest.transform.position) <= targetPointToInterest.interactRadius;
    }
    
    public void SetDestination(Vector3 pos) {
        agent.SetDestination(pos);
    }
    public  void ClearDestination() {
        agent.ResetPath();
    }
}
