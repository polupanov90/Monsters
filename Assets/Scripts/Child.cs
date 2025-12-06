
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class Child : Unit {
    
    
    [SerializeField] private Transform[] targetList;
    [SerializeField] private GameObject interactIndicator;
    
    [SerializeField] private NeedMB firstNeedMb;
    [SerializeField] private NeedMB secondNeedMb;
    [SerializeField] private NeedMB thirdNeedMb;
    [SerializeField] private NeedMB fourthNeedMb;
    
    private int SLEEP_TIMER = 4; 
    public bool isMoving;
    private Transform target;
    private Vector3 destination;
    public NavMeshAgent agent;
    private Player player;
    private bool isTarget;
    public bool isTransfer;
    public bool isSleep;

    public void SetNeed(NeedO needO) {
        NeedMB currentNeed = firstNeedMb;
        if (firstNeedMb.GetIsShow()) {
            currentNeed = secondNeedMb;
            if (secondNeedMb.GetIsShow()) {
                currentNeed = thirdNeedMb;
                if (thirdNeedMb.GetIsShow()) {
                    currentNeed = fourthNeedMb;
                }
            }
        }
        currentNeed.SetImage(needO);
    }

    public void CLearNeed(NeedO needO) {
        if (firstNeedMb.GetIncludeNeedType(needO)) {
            firstNeedMb.ClearImage();
            if (secondNeedMb.GetIsShow()) {
                firstNeedMb.SetImage(secondNeedMb.needO);
                secondNeedMb.ClearImage();
            }
            if (thirdNeedMb.GetIsShow()) {
                secondNeedMb.SetImage(thirdNeedMb.needO);
                thirdNeedMb.ClearImage();
            }
            if (fourthNeedMb.GetIsShow()) {
                thirdNeedMb.SetImage(fourthNeedMb.needO);
                fourthNeedMb.ClearImage();
            }
        }
        if (secondNeedMb.GetIncludeNeedType(needO)) {
            secondNeedMb.ClearImage();
            if (thirdNeedMb.GetIsShow()) {
                secondNeedMb.SetImage(thirdNeedMb.needO);
                thirdNeedMb.ClearImage();
            }
            if (fourthNeedMb.GetIsShow()) {
                thirdNeedMb.SetImage(fourthNeedMb.needO);
                fourthNeedMb.ClearImage();
            }
        }
        if (thirdNeedMb.GetIncludeNeedType(needO)) {
            thirdNeedMb.ClearImage();
            if (fourthNeedMb.GetIsShow()) {
                thirdNeedMb.SetImage(fourthNeedMb.needO);
                fourthNeedMb.ClearImage();
            }
        }
        if (fourthNeedMb.GetIncludeNeedType(needO)) {
            fourthNeedMb.ClearImage();
        }
    }

    public bool HasNeedType(NeedO needO) {
        bool firstHasNeed = firstNeedMb.GetIncludeNeedType(needO);
        bool secondHasNeed = secondNeedMb.GetIncludeNeedType(needO);
        bool thirdHasNeed = thirdNeedMb.GetIncludeNeedType(needO);
        bool fourthHasNeed = fourthNeedMb.GetIncludeNeedType(needO);
        return firstHasNeed || secondHasNeed || thirdHasNeed || fourthHasNeed;
    }
    
    public bool HasEmptyNeedSlot() {
        bool firstIsShow = firstNeedMb.GetIsShow();
        bool secondIsShow = secondNeedMb.GetIsShow();
        bool thirdIsShow = thirdNeedMb.GetIsShow();
        bool fourthIsShow = fourthNeedMb.GetIsShow();
        return !firstIsShow || !secondIsShow || !thirdIsShow || !fourthIsShow;
    } 
    public int GetEmptyNeedSlotCount() {
        if (!firstNeedMb.GetIsShow()) {
            return 4;
        }
        if (!secondNeedMb.GetIsShow()) {
            return 3;
        }
        if (!thirdNeedMb.GetIsShow()) {
            return 2;
        }
        if (!fourthNeedMb.GetIsShow()) {
            return 1;
        }
        return 0;
    } 
    
    Transform GetRandomTarget() {
        System.Random rnd = new System.Random();
        int index = rnd.Next(0, targetList.Length);
        return targetList[index];
    }

    void SetRandomTarget() {
        target = GetRandomTarget();
    }
    
    void Start() {
        player = Player.Instance;
        agent = GetComponent<NavMeshAgent>();
        
        destination = agent.destination;
        SetRandomTarget();
    }

    void Update() {
        SetIsMove();
        Move();
        SetTargetVisual();
    }

    void Move() {
        if (isTransfer || isSleep) {
            if (isTransfer && transform.localPosition != Vector3.zero) {
                transform.localPosition = Vector3.zero;
            }
            if (agent.hasPath) {
                agent.ResetPath();    
            }
        } else {
            if (!agent.hasPath) {
                if (Vector3.Distance(destination, target.position) > 1.0f) {
                    destination = target.position;
                    agent.destination = destination;
                }
                if (!isMoving) {
                    SetRandomTarget();
                }
            }
        }
    }

    public void SleepToBed(Transform bedTransform) {
        agent.ResetPath();
        transform.parent = null;
        isSleep = true;
        isTransfer = false;
        transform.position = bedTransform.position;
        Invoke("WakeUpFromBed", SLEEP_TIMER);
    }

    public void WakeUpFromBed() {
        isSleep = false;
        CLearNeed(NeedController.Instance.GetNeedOByNeedEnum(NeedEnum.Sleep));
    }

    private void SetTargetVisual() {
        if (player.nearestChild == transform) {
            isTarget = true;
            interactIndicator.SetActive(true);
        } else if (isTarget) {
            isTarget = false;
            interactIndicator.SetActive(false);
        }
    }

    public override bool GetIsMove() {
        return isMoving;
    }
    public override void SetIsMove() {
        if (agent.velocity.magnitude != 0) {
            isMoving = true;
        } else {
            isMoving = false;
        }
    }
}
