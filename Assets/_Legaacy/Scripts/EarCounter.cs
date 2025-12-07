using System;
using TMPro;
using UnityEngine;

public class EarCounter : MonoBehaviour
{
    [SerializeField] private EatController eatController;
    public TMP_Text oneEatText;
    public TMP_Text twoEatText;
    public TMP_Text threeEatText;
    private void Start() {
        SetEatText();
    }

    private void Update() {
        SetEatText();
    }

    private void SetEatText() {
        if (oneEatText.text != eatController.eatOneCount.ToString()) {
            oneEatText.text = eatController.eatOneCount.ToString();    
        }
        if (twoEatText.text != eatController.eatTwoCount.ToString()) {
            twoEatText.text = eatController.eatTwoCount.ToString();    
        }
        if (threeEatText.text != eatController.eatThreeCount.ToString()) {
            threeEatText.text = eatController.eatThreeCount.ToString();    
        }
    }
}
