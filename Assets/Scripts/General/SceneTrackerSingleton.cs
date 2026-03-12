using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;

public class SceneTrackerSingleton : MonoBehaviour
{
    public static SceneTrackerSingleton Instance { get; private set; }

    private int carnum = 0;

    public static string CurrentSceneName { get; private set; }
    public string PreviousSceneName { get; private set; }

    public List<SceneScriptables> scenes = new List<SceneScriptables>();

    private string currentscenename;

    public static event Action<string> onSceneName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void Start()
    {
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
                bool iscar = scene.iscar;
                changestate(iscar);
            }
            else
            {
                print("current scene isnt registed in scene tracker script");
            }
        }
    }

    public void changestate(bool iscar)
    {
        print("tring to change state bc is car is "+ iscar);
        if(iscar == true)
        {
            carnum++;
            changeStartNode(carnum);
            NewJournalSave.instance.SetState(NewJournalSave.States.Car);
        }
        else
        {
             NewJournalSave.instance.SetState(NewJournalSave.States.Gasstation);
        }
       
    }

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize with current scene
            CurrentSceneName = SceneManager.GetActiveScene().name;
            PreviousSceneName = "None";

            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
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

    private void changeStartNode(int carnum)
    {
        if(carnum == 0)
        {
            
        }
    }

    
}