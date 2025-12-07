using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NeedController : MonoBehaviour {
    public static NeedController Instance { get; private set; }
     public NeedO[] needObjects;
    private Child[] childs;
    private int needIndex;
    private int REPEAT_INTERVAL = 10;
    
    private void Awake() {
        Instance = this;
        childs = FindObjectsOfType<Child>();
        needIndex = needObjects.Length - 1;
        InvokeRepeating("AddNewNeed", REPEAT_INTERVAL, REPEAT_INTERVAL);
    }

    private void Start() {
        AddNewNeed();
    }


    private  Child[] GetChildsOnlyHasNeedType(NeedO _needO) {
        Child[] onlyHasEmptyNeedSlotChilds = childs.Where((child) => child.HasEmptyNeedSlot()).ToArray();
        
        Child[] onlyDontHasNeedTypeChilds = onlyHasEmptyNeedSlotChilds.Where((child) => !child.HasNeedType(_needO)).ToArray();
        
        Array.Sort(onlyDontHasNeedTypeChilds, (child1, child2) => {
            return child1.GetEmptyNeedSlotCount() < child2.GetEmptyNeedSlotCount() ? 1 : -1;
        });
        return onlyDontHasNeedTypeChilds;
    }
    private void AddNewNeed() {
        int initialIndex = needIndex;
        Child[] sortedOnlyDontHasNeedTypeChilds = {};

        int count = 0;
        
        do {
            NeedO currentNeedO = needObjects[initialIndex];
            sortedOnlyDontHasNeedTypeChilds = GetChildsOnlyHasNeedType(currentNeedO);
            if (sortedOnlyDontHasNeedTypeChilds.Length > 0) {
                int currentChildIndex = Random.Range(0, sortedOnlyDontHasNeedTypeChilds.Length - 1);
                sortedOnlyDontHasNeedTypeChilds[currentChildIndex].SetNeed(currentNeedO);
            } 
            initialIndex = GetLastIndex(initialIndex);
            count++;
            if (count > 10) {
                Debug.LogWarning("Цикл не исправен");
                break;
            }

        } while (sortedOnlyDontHasNeedTypeChilds.Length == 0 && initialIndex != needIndex);

        needIndex = sortedOnlyDontHasNeedTypeChilds.Length == 0  ? initialIndex + 1 :  initialIndex;
    }
    
    private int GetLastIndex(int _index) {
        int lastIndex = _index + 1;
        if (lastIndex >= needObjects.Length - 1) {
            lastIndex = 0;
        }
        return lastIndex;
    }

    public NeedO GetNeedOByNeedEnum(NeedEnum _needEnum) {
        return Array.Find(needObjects, (_needO) => _needO.type == _needEnum);
    }
}
