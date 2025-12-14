using System;
using UnityEngine;

public class UseElementToyHorse: AbstractUseElement {
    private Vector3 oldToyPosition;
    private Quaternion oldToyRotation;
    [SerializeField] Transform toy;
    
    
    
    public override void Use(Child child) {
        oldToyPosition = toy.transform.position;
        oldToyRotation = toy.transform.rotation;
        
        Transform[] components =  child.GetComponentsInChildren<Transform>();
        Transform hearTransform =  Array.Find(components, (component) => component.gameObject.name == "кисть.L_end").gameObject.transform;
        toy.transform.SetParent(hearTransform);
        toy.transform.localPosition = Vector3.zero;
        child.isSitGame = true;
    }
    public override void Exit(Child child) {
        toy.transform.parent = null;
        toy.transform.position = oldToyPosition;
        toy.transform.rotation = oldToyRotation;
        child.isSitGame = false;
    }
}