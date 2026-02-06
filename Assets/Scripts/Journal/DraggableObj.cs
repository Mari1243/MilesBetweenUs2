using UnityEngine;

[CreateAssetMenu(fileName = "DraggableObj", menuName = "Scriptable Objects/DraggableObj")]
public class DraggableObj : ScriptableObject
{
    public Vector3 size;
    public Quaternion rotation;
    public Vector3 location;

    public Sprite sprite;
}
