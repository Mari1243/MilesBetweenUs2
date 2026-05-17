using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using Unity.VisualScripting;

public class SceneTrackerSingleton : MonoBehaviour
{
    public static SceneTrackerSingleton Instance { get; private set; }

    public int carnum = 1;

    public static string CurrentSceneName { get; private set; }
    public string PreviousSceneName { get; private set; }

    public List<SceneScriptables> scenes = new List<SceneScriptables>();
    public SceneScriptables currentscene;
    private GameObject ToDoListPrefab;

   public static event Action<string, int> CurrentSceneEvent;

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
                print("FOUND current scene in list");
                
                currentscene = scene;
                //setting to car if true or false if not
                bool iscar = scene.iscar;
                changestate(iscar);
                changemusic(currentscene);
            }
        }
    }

    public void changemusic(SceneScriptables scene)
    {
        print("changing music to " + scene.sceneMusic.name.ToString());
        if(scene.sceneMusic!=null){
        SoundManager.Instance.changeMusic(scene.sceneMusic);
        }
        else
        {
            print("this scene has no music assigned");
        }
    }

    public void changestate(bool iscar)
    {
        if(NewJournalSave.instance!= null)
        {
             if(iscar == true)
            {
            //this is to change the start node for which car scene ur in!
            print("scene name is " + currentscene.name + " and the car num is " + carnum);
            carnum++;
            NewJournalSave.instance.SetState(States.Car);
            CurrentSceneEvent?.Invoke(CurrentSceneName, carnum);
            }
            else
            {
            NewJournalSave.instance.SetState(States.Gasstation);
            CurrentSceneEvent?.Invoke(CurrentSceneName, carnum);
            //spawn list
            //getting scenescriptable
            if (currentscene.ToDoList != null)
            {
                print("setting todolist and beginning spawn between scenetracker and new journal save");
                carOver?.Invoke();
                ToDoListPrefab = currentscene.ToDoList;
                NewJournalSave.instance.newspawnlist(ToDoListPrefab);
            }
            else
            {
                Debug.LogError("THIS SCENE HAS NO TODOLIST ASSIGNED");
            }
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