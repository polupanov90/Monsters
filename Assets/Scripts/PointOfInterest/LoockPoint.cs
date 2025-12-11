using UnityEngine;

public class LoockPoint : MonoBehaviour {
    private void OnDrawGizmos() {
        Gizmos.color = Color.orange;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 4);
    }
}
