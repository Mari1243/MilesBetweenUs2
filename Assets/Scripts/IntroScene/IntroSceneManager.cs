using UnityEngine;
using System.Collections;
using MaskTransitions;
using System;
using UnityEngine.UI;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    public Canvas journalcanvas;
    private bool journalActive;

    public GameObject book;
    public GameObject Map;
    private RectTransform rect;
    private float wait = 3.5f;
    public GameObject TPCam;
    public static event Action<string> OnHintNeeded;
    public Item NOJournal;
    public static bool journalopen = false;

    public GameObject instructionss;


    private void OnEnable()
    {
        // Mapinteractable.showJournal += OpenJournalHint;
        // Mapinteractable.showJournal += FreezeCam;
        Mapinteractable.nextscene += nextScene;
        interactable.onMap += stuff;
    }
    private void OnDisable()
    {
        // Mapinteractable.showJournal -= OpenJournalHint;
        // Mapinteractable.showJournal -= FreezeCam;
        Mapinteractable.nextscene -= nextScene;
        interactable.onMap -= stuff;

        
    }


    //journal behavior
    public void stuff()
    {
            if(!journalopen)
            {
                instructionss.SetActive(false);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                journalcanvas.enabled = true;
                DOTween.Restart("animateIn"); 
                DOTween.Play ("animateIn");
                journalopen = true;
                instructions();
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                DOTween.Restart("animateOut"); 
                DOTween.Play ("animateOut");
                journalopen = false;
                instructionss.SetActive(true);
            }
    }

    private void instructions()
    {
        print("trying to start dialogue");
        DialogueManager.instance.LoadDialog("StoreMap");
        DialogueManager.instance.StartDialog();
        DialogueManager.instance.OnDialogOver();
    }

    private void nextScene()
    {
        if (InventoryManager.instance.HasItem("Map"))
        {
            SceneSwitch.Instance.SwitchScene("Car");
        }
        else
        {
            DialogueManager.instance.TalkInteraction(NOJournal);
        }
    }

    private void OpenJournalHint()
    {
        print("freezing stuff");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        //StartCoroutine(hintroutine());
    }

    // private IEnumerator hintroutine()
    // {
    //     print("starting");
    //     Sequence theSequence = DOTween.Sequence();
    //     rect = Map.gameObject.GetComponent<RectTransform>();
    //     journalcanvas.enabled = true;
    //     theSequence.Append(book.transform.DOLocalMove(centeredBookPos, 1).SetEase(Ease.InOutQuad));
    //     theSequence.Append(rect.DOAnchorPos(centeredMapPos, 1).SetEase(Ease.InOutQuad));
    //     yield return new WaitForSeconds(wait);
    //     print("playing sequence");
    //     theSequence.Play();
    // }

    void OnTriggerEnter(Collider hit)
    {
        print("hitting");
        if (hit.name == "Player")
        {
            bool hasJournal = HasItemByName("Journal");
            //print(hasJournal);
            if (hasJournal)
            {
                NextScene();
            }
            else
            {
                sendingHint("I should grab my journal first");
            }
        }
    }

    private void sendingHint(string str)
    {
        OnHintNeeded?.Invoke("nojournal");
        //this should invoke an image instead??
    }

    public bool HasItemByName(string itemName)
    {
        foreach (var kvp in InventoryManager.instance.itemDictionary)
        {
            if (kvp.Key.itemName == itemName) // assuming Item has an itemName field
            {
                return true;
            }
        }
        return false;
    }
    private void NextScene()
    {
        print("next scene");
        //play car door open noise
        //TransitionManager.Instance.PlayTransition(2f, 2f);
        SceneSwitch.Instance.SwitchScene("Car");
    }

    private void FreezeCam()
    {
        TPCam.SetActive(false);
    }
    public void UnFreezeCam()
    {
        TPCam.SetActive(true);
       
    }


}


