using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EatController : MonoBehaviour {
    private int MIN_EAT_ADD_COUNT = 1;
    private int MAX_EAT_ADD_COUNT = 3;
    private int REPEAT_INTERCAL = 10;
    
    public int eatOneCount = 2;
    public int eatTwoCount = 2;
    public int eatThreeCount = 2;

    private void Start() {
        InvokeRepeating("AddEatCounts", REPEAT_INTERCAL, REPEAT_INTERCAL);
    }

    public bool GetEatOne() {
        if (eatOneCount > 0) {
            eatOneCount--;
            return true;
        }   
        return false;
    }
    public bool GetEatTwo() {
        if (eatTwoCount > 0) {
            eatTwoCount--;
            return true;
        }   
        return false;
    }
    public bool GetEatThree() {
        if (eatThreeCount > 0) {
            eatThreeCount--;
            return true;
        }
        return false;
    }

    private int GetRandomNumber() {
        return Random.Range(MIN_EAT_ADD_COUNT, MAX_EAT_ADD_COUNT);
    }

    private void AddEatCounts() {
        eatOneCount += GetRandomNumber();
        eatTwoCount += GetRandomNumber();
        eatThreeCount += GetRandomNumber();
    }
    
}
