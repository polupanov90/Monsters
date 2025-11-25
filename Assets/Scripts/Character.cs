using System;
using UnityEngine;

public class Character : MonoBehaviour {
    protected float moveSpeed = 12;
    protected float rotateSpeed = 17;
    
    protected void Move(Vector2 moveVector) {
        Vector3 moveVector3 = transformVector3ToVector3(moveVector);
        float moveDelta = moveSpeed * Time.deltaTime;
        Vector3 moveDeltaVector = moveVector3 * moveDelta;
        
        transform.position += moveDeltaVector;
    }
    protected void Rotate(Vector2 moveVector) {
        Vector3 rotateVector3 = transformVector3ToVector3(moveVector);
        float rotateDelta = rotateSpeed * Time.deltaTime;
        Vector3 rotateDeltaVector = Vector3.Slerp(transform.forward, rotateVector3, rotateDelta);
        
        transform.forward += rotateDeltaVector;
    }

    private Vector3 transformVector3ToVector3(Vector2 vector2) {
        return new Vector3(vector2.x, 0, vector2.y);
    }
}
