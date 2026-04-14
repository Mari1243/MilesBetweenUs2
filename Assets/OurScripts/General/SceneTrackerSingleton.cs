using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;

public class SceneTrackerSingleton : MonoBehaviour
{
    public static SceneTrackerSingleton Instance { get; private set; }

    public int carnum = 0;

    public static string CurrentSceneName { get; private set; }
    public string PreviousSceneName { get; private set; }

    public List<SceneScriptables> scenes = new List<SceneScriptables>();
    private SceneScriptables currentscene;
    private GameObject ToDoListPrefab;

    private string currentscenename;

    public static event Action<string> onSceneName;
    public static event Action carOver;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Initialize with current scene
        CurrentSceneName = SceneManager.GetActiveScene().name;
        PreviousSceneName = "None";

        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
        settingstate();
    }


    private void settingstate()
    {
        currentscenename = SceneManager.GetActiveScene().name.ToString();
        foreach(SceneScriptables scene in scenes)
        {
            string scenename = scene.SceneName;

            if(scenename == currentscenename)
            {
                print("found current scene in list");
                currentscene = scene;
                //setting to car if true or false if not
                bool iscar = scene.iscar;
                changestate(iscar);
            }
        }
    }

    public void changestate(bool iscar)
    {
        print("tring to change state bc is car is "+ iscar);
        if(iscar == true)
        {
            //this is to change the start node for which car scene ur in!
            carnum++;
            NewJournalSave.instance.SetState(States.Car);
        }
        else
        {
            NewJournalSave.instance.SetState(States.Gasstation);
            //spawn list
            //getting scenescriptable
            if (currentscene.ToDoList != null)
            {
                print("setting todolist and beginning spawn between scenetracker and new journal save");
                carOver?.Invoke();
                ToDoListPrefab = currentscene.ToDoList;
                NewJournalSave.instance.newspawnlist(ToDoListPrefab);
            }
        }
       
    }

   
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update previous scene before changing current
        PreviousSceneName = CurrentSceneName;
        CurrentSceneName = scene.name;
     
        onSceneName?.Invoke(PreviousSceneName);
        Debug.Log($"Scene changed: {PreviousSceneName} -> {CurrentSceneName}");
        settingstate();
    }

    void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

   
}