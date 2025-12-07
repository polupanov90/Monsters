using System;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Unit {

    public static Player Instance { get; private set; }
    [SerializeField] private int RAYCAST_CHILDREN_RADIUS = 5; 
    [SerializeField] private EatO eatOOne;
    [SerializeField] private EatO eatOTwo;
    [SerializeField] private EatO eatOThree;
    
    [SerializeField] private InputController inputController;
    [SerializeField] private EatItem eatItem;
    [SerializeField] private Transform transferPoint;
    [SerializeField] private float moveSpeed = 10;
    [SerializeField] private float rotateSpeed = 30;
    
    private float moveDistance = 0.5f;
    private float playerRadius = 1f;
    private float playerHeight = 2f;
    
    public float raycastRadius = 1f;
    public float raycastDistance = 2f;
    
    private bool isMoving;
    private bool canMoveX;
    private bool canMoveZ;
    // private Item forwardItem;
    private Transform transferTransform;
    private GameObject transferObject;
    private EatO transferEatO;
    private bool transferEatIsPrepared;
    private Child transferChild;

    public Transform nearestChild;
    public GameObject transferHuitar;
    

    public Transform forwardTransform;

    private void Awake() {
        Instance = this;
    }
    private void Start() {
        inputController.OnTurnOne += TurnOneHandler;
        inputController.OnTurnTwo += TurnTwoHandler;
        inputController.OnTurnThree += TurnThreeHandler;
        inputController.OnUse += UseHandler;
    }

    private void Update() {
        SetIsMove();
        Move();
        Rotate();
        SetCanMove();
        SetForwardItem();
        SetOverlapChildren();
    }

    private void TurnOneHandler(object sender, EventArgs args) {
        if (!transferTransform) {
            if (eatItem.TryGetEatOneHandler()) {
                transferTransform = Instantiate(eatOOne.prefab, transferPoint);
                transferEatO = eatOOne;
            }
        }
    }
    
    private void TurnTwoHandler(object sender, EventArgs args) {
        if (!transferTransform) {
            if (eatItem.TryGetEatTwoHandler()) {
                transferTransform = Instantiate(eatOTwo.prefab, transferPoint);
                transferEatO = eatOTwo;
            }
        }
    }
    
    private void TurnThreeHandler(object sender, EventArgs args) {
        if (!transferTransform) {
            if (eatItem.TryGetEatThreeHandler()) {
                transferTransform = Instantiate(eatOThree.prefab, transferPoint);
                transferEatO = eatOThree;
            }
        }
    }

    private void PutEatInCooker(Cooker cooker) {
        cooker.PutEat(transferEatO.eatType);
    }

    private void UseHandler(object sender, EventArgs args) {
        if (!transferHuitar && forwardTransform != null && forwardTransform.gameObject.TryGetComponent(out Huitar _huitar) && !transferChild && !transferEatO && !transferEatIsPrepared) {
            _huitar.transform.SetParent(transferPoint);
            _huitar.transform.localPosition = Vector3.zero;
            _huitar.transform.rotation = transform.rotation;
            transferHuitar = _huitar.gameObject;
        } else if (transferHuitar) {
            transferHuitar.transform.parent = null;
            transferHuitar.transform.position = new Vector3(transferPoint.position.x, 0f, transferPoint.position.z);
            transferHuitar = null;
            Debug.Log("test2");
        } else if (forwardTransform!= null && forwardTransform.gameObject.TryGetComponent<Cooker>(out  Cooker cooker)) {
            if (transferEatO != null && !transferEatIsPrepared && cooker.cookingStatusEnum == CookingStatusEnum.Empty) {
                PutEatInCooker(cooker);
                DestroyTransfer();
            } else if (transferEatO == null && cooker.cookingStatusEnum == CookingStatusEnum.Done) {
                if (cooker.GetPreparedEatO(out transferEatO)) {
                    
                    transferTransform = Instantiate(transferEatO.preparedPrefab, transferPoint);   
                    transferEatIsPrepared =  true;
                }
            }
        } else if (nearestChild && transferEatIsPrepared) {
            NeedEnum needType = transferEatO.eatType == EatEnum.One ? NeedEnum.EatOne :
                transferEatO.eatType == EatEnum.Two ? NeedEnum.EatTwo : NeedEnum.EatThree;
            Child child = nearestChild.gameObject.GetComponent<Child>();
            NeedO needO = Array.Find(NeedController.Instance.needObjects, (item) => item.type == needType);
            
            if (child.HasNeedType(needO)) {
                child.CLearNeed(needO);
                DestroyTransfer();
            }
        } else if (transferChild) {
            if (
                forwardTransform != null &&
                forwardTransform.gameObject.TryGetComponent(out Bed bed) &&
                transferChild.HasNeedType(NeedController.Instance.GetNeedOByNeedEnum(NeedEnum.Sleep))
            ) {
                transferChild.SleepToBed(bed.transform);
            } else {
                transferChild.transform.parent = null;
                transferChild.isTransfer = false;
            }
            transferChild = null;
        } else if (nearestChild && !transferEatO && nearestChild.gameObject.TryGetComponent<Child>(out Child child) && !child.isSleep) {
            child.agent.ResetPath();
            child.isTransfer =  true;
            child.transform.SetParent(transferPoint);
            child.transform.localPosition = Vector3.zero;
            child.transform.rotation = transform.rotation;
            transferChild = child;
        } 
        else if (forwardTransform != null && forwardTransform.gameObject.TryGetComponent(out Basket basket)) {
            DestroyTransfer();
        }

    }

    private void DestroyTransfer() {
        if (transferTransform != null) {
            Destroy(transferTransform.gameObject);
            transferTransform = null;
            transferObject = null;
            transferEatO = null;
            transferEatIsPrepared = false;
        }
    }

    private void SetForwardItem() {
        RaycastHit hit;
        if (Physics.CapsuleCast(
                transform.position,
                transform.position + Vector3.up * playerHeight,
                raycastRadius,
                transform.forward,
                out hit,
                raycastDistance
            )) {
            forwardTransform = hit.transform;
        } else {
            forwardTransform = null;
        }
    }

    private void SetOverlapChildren() {
        Collider[] colliders = Physics.OverlapSphere(transform.position, RAYCAST_CHILDREN_RADIUS);
        Collider[] onlyChildren = colliders.Where((collider) => {
            bool isChild = collider.gameObject.TryGetComponent<Child>(out Child child);
            return isChild;
        }).ToArray();

        Array.Sort(onlyChildren, (child1, child2) => {
            float child1Distance = Vector3.Distance(transform.position, child1.transform.position);
            float child2Distance = Vector3.Distance(transform.position, child2.transform.position);
            return child1Distance > child2Distance ? 1 : -1;
        });
        if (onlyChildren.Length > 0) {
            nearestChild = onlyChildren[0].transform;
        } else {
            nearestChild = null;
        }
    }
    
    void OnDrawGizmos() {
        // Gizmos.color = Color.red;
        // Vector3 direction = transform.TransformDirection(Vector3.forward) * 0.5f;
        // Gizmos.DrawRay(transform.position, direction);
        // Gizmos.DrawWireSphere(transform.position, RAYCAST_CHILDREN_RADIUS);
    }

    private void SetCanMove() {
        canMoveX = !Physics.CapsuleCast(
            transform.position,
            transform.position,
            playerRadius,
            GetInputControllerMovementVectorX(),
            moveDistance
        );
        canMoveZ = !Physics.CapsuleCast(
            transform.position,
            transform.position,
            playerRadius,
            GetInputControllerMovementVectorZ(),
            moveDistance
        );
    }

    private void Move() {
        MoveX();
        MoveZ();
    }

    private void MoveX() {
        if (canMoveX) {
            Vector3 moveVectorX = GetInputControllerMovementVectorX();
            float moveDeltaX = moveSpeed * Time.deltaTime;
            Vector3 moveDeltaVectorX = moveVectorX * moveDeltaX;
            transform.position += moveDeltaVectorX;    
        }
    }
    private void MoveZ() {
        if (canMoveZ) {
            Vector3 moveVectorZ = GetInputControllerMovementVectorZ();
            float moveDeltaZ = moveSpeed * Time.deltaTime;
            Vector3 moveDeltaVectorZ = moveVectorZ * moveDeltaZ;
            transform.position += moveDeltaVectorZ;   
        }
    }

    private Vector3 GetInputControllerMovementVectorX() {
        return new Vector3(inputController.movementVector.x, 0, 0);
    }
    private Vector3 GetInputControllerMovementVectorZ() {
        return new Vector3(0, 0, inputController.movementVector.y);
    }

    private void Rotate() {
        if (isMoving) {
            Vector2 moveVector = inputController.movementVector;
            Vector3 rotateVector3 = TransformVector3ToVector3(moveVector);
            float rotateDelta = rotateSpeed * Time.deltaTime;
            Vector3 rotateDeltaVector = Vector3.Slerp(transform.forward, rotateVector3, rotateDelta);
            transform.forward += rotateDeltaVector;
        }
    }
    
    private Vector3 TransformVector3ToVector3(Vector2 vector2) {
        return new Vector3(vector2.x, 0, vector2.y);
    }

    public override bool GetIsMove() {
        return isMoving;
    }
    public override void SetIsMove() {
        if (inputController.movementVector != Vector2.zero) {
            isMoving = true;
        } else {
            isMoving = false;
        }
    }
}
