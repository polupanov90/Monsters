using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class Cooker : MonoBehaviour {
    private int PREPARED_TIMER = 2;
    [SerializeField] private Transform statusPointer;
    [SerializeField] private CookingStatusO[] cookingStatusObjects;
    [SerializeField] private EatO[] eatObjects;
    
    private Transform currentStatusVisual;
    public CookingStatusEnum  cookingStatusEnum;
    private EatO currentEatO; 
    
    private void Awake() {
        SetStatusVisual(CookingStatusEnum.Empty);
    }

    private void Update() {
        SetStatusVisual(cookingStatusEnum);
    }

    public void PutEat(EatEnum eatType) {
        currentEatO = GetEatOByEatType(eatType);
        cookingStatusEnum = CookingStatusEnum.Preparing;
        Invoke("BeginToPrepareEat", PREPARED_TIMER);
    }

    public bool GetPreparedEatO([CanBeNull] out EatO eatO) {
        if (currentEatO != null && cookingStatusEnum == CookingStatusEnum.Done) {
            cookingStatusEnum = CookingStatusEnum.Empty;
            eatO = currentEatO;
            currentEatO = null;
            return true;
        }
        eatO = null;
        return false;
    }

    public void BeginToPrepareEat() {
        cookingStatusEnum = CookingStatusEnum.Done;
    }
    
    private Transform GetTransformStatusByStatusType(CookingStatusEnum cookingStatusEnum) {
        return Array.Find(cookingStatusObjects, (item) => item.status == cookingStatusEnum).panel;
    }
    private EatO GetEatOByEatType(EatEnum eatType) {
        return Array.Find(eatObjects, (item) => item.eatType == eatType);
    }

    private void SetStatusVisual(CookingStatusEnum cookingStatusEnum) {
        cookingStatusEnum = cookingStatusEnum;
        if (currentStatusVisual != null) {
            Destroy(currentStatusVisual.gameObject);
        }
        currentStatusVisual = Instantiate(GetTransformStatusByStatusType(cookingStatusEnum), statusPointer);
    }

}