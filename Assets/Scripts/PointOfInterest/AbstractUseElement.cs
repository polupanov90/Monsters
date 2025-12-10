using UnityEngine;

public abstract class AbstractUseElement : MonoBehaviour {
    abstract public void Use(Child child);
    abstract public void Exit(Child child);
}
