using UnityEngine;
[CreateAssetMenu()]
public class EatO : ScriptableObject {
    public Transform prefab;
    public Sprite sprite;
    public EatEnum eatType;
    public bool prepared;
    public Transform preparedPrefab;
}
