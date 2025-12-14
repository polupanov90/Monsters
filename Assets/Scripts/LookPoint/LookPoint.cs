using UnityEngine;

public class LookPoint : MonoBehaviour
{
    private void OnDrawGizmos() {
        Gizmos.color = Color.yellowGreen;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 6);
    }
}
