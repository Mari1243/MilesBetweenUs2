using UnityEngine;

[CreateAssetMenu(fileName = "ToDoListData", menuName = "Game/ToDoListData")]
public class ToDoListData : ScriptableObject
{
    public GameObject[] ToDoListPrefabs;
}