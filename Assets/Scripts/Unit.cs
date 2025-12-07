using UnityEngine;

public class Unit : MonoBehaviour, UnitInterface {
    virtual public bool GetIsMove() {
        return true;
    }

    virtual public void SetIsMove() {}
}
