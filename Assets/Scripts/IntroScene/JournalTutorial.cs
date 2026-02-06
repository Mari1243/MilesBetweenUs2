using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using System.Collections;

public class JournalTutorial : MonoBehaviour
{
    private static GameObject journal;
    public Image crosshair;

    public static event Action hideJournal;
    private void Start()
    {
        journal = this.transform.GetChild(0).gameObject;
        print("journal name is " + journal.name);
        this.GetComponent<Canvas>().enabled = false;
    }

      private void OnEnable()
    {
        Mapinteractable.showJournal += JournalScene;
    }
    private void OnDisable()
    {
        Mapinteractable.showJournal -= JournalScene;
    }

    public static void closeJournal(GameObject obj)
    {
        var anim = journal.GetComponent<DOTweenAnimation>();
        if(anim != null)
        {
            // The tween doesn't exist until DORestart() is called
            anim.DORestart();
            
            // NOW access the tween after it's been created
            if(anim.tween != null)
            {
                print(obj.name);
                obj.transform.DOMoveY(-900,2);
                anim.tween.OnComplete(() => {
                    journal.SetActive(false);
                    //unfreeze input
                    //set mouse inactive
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    print("disabling journal");
                    hideJournal?.Invoke();
                });
            }
            else
            {
                print("tween is null after DORestart");
            }
        }
        else
        {
            print("anim is null????");
        }
    }

    public void JournalScene() //SHOWS JOURNAL 
    {

        print("showing journal");
        this.GetComponent<Canvas>().enabled = true;
        this.gameObject.SetActive(true);
    }
}
