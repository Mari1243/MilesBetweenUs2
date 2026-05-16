using UnityEngine;

[CreateAssetMenu(fileName = "Scene", menuName = "Scriptable Objects/Scene")]
public class SceneScriptables: ScriptableObject
{
    public string SceneName;
    public GameObject ToDoList;
    public bool locationOverride;
    public bool iscar;
    public Vector3 dialogueposition;
    public AudioClip sceneMusic;
}
