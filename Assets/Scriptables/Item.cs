using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{

    [SerializeField] public Sprite img;
    [SerializeField] public string itemName;
    [SerializeField] public bool loreItem;

    //used in ontalk action and called in dialogueManager
    [SerializeField] public string node;
    [SerializeField] public Vector3 diagPos;

}
