using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTracker : MonoBehaviour
{
    public List<SceneScriptables> scenes = new List<SceneScriptables>();

    private string currentscenename;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void Start()
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
            NewJournalSave.instance.SetState(NewJournalSave.States.Car);
        }
        else
        {
             NewJournalSave.instance.SetState(NewJournalSave.States.Gasstation);
        }
       
    }

}
