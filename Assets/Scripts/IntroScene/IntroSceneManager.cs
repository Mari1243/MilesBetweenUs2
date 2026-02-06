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
     public static bool journalActive;
    //public GameObject Hint1;
    public GameObject book;
    public GameObject Map;
    private Vector3 centeredBookPos = new Vector3(2,-3,0);
    public Vector2 centeredMapPos = new Vector3(302,-16);
    private RectTransform rect;
    private float wait = 3.5f;
    public GameObject TPCam;
    public static event Action<string> OnHintNeeded;
    public Item NOJournal;

    private void Start()
    {
        journalcanvas.enabled = false;
        //Hint1.SetActive(false);
    }

    private void OnEnable()
    {
        Mapinteractable.showJournal += OpenJournalHint;
        Mapinteractable.showJournal += FreezeCam;
        Mapinteractable.nextscene += nextScene;
    }
    private void OnDisable()
    {
        Mapinteractable.showJournal -= OpenJournalHint;
        Mapinteractable.showJournal -= FreezeCam;
        Mapinteractable.nextscene -= nextScene;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        journalActive = true;
        StartCoroutine(hintroutine());
    }

    private IEnumerator hintroutine()
    {
        print("starting");
        Sequence theSequence = DOTween.Sequence();
        rect = Map.gameObject.GetComponent<RectTransform>();
        journalcanvas.enabled = true;
        theSequence.Append(book.transform.DOLocalMove(centeredBookPos, 1).SetEase(Ease.InOutQuad));
        theSequence.Append(rect.DOAnchorPos(centeredMapPos, 1).SetEase(Ease.InOutQuad));
        yield return new WaitForSeconds(wait);
        print("playing sequence");
        theSequence.Play();
    }

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


