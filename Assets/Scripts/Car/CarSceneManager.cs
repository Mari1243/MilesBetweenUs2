using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

public class CarSceneManager : MonoBehaviour
{
    [Header("Inventory")]
    public List<InventoryItem> currentInventory = new List<InventoryItem>();
    public static event System.Action<Sprite> assignsprite;
    public static event Action<bool> HoldingTalkItem;
    public GameObject tabholder;

    [Header("Cursor")]
    public Texture2D cursor;
    public Image crosshair;

    [Header("Journal")]
    public GameObject journalItem;
    public static bool journalActive;
    public GameObject DraggableItemPrefab;
    public GameObject spawningRange;



    [Header("Player Input")]
    public PlayerInput playerInp;

    [Header("Camera")]
    public GameObject playercam;

    [Header("Brother")]
    public Animator brother;

    

    private void Awake()
    {

        
    }
    private void OnEnable()
    {
        interactable.showJournal += JournalScene;
        //interactable.onTalkCar += ActivateBroDiag;
        ToggleJournal.hideJournal += ExitJournal;
    }
    private void OnDisable()
    {
        interactable.showJournal -= JournalScene;
        //interactable.onTalkCar-= ActivateBroDiag;  
        ToggleJournal.hideJournal -= ExitJournal;
    }

    private void Start()
    {
        brother.Play("Armature_BigBro_Drive");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentInventory = InventoryManager.instance.inventory;
        if (currentInventory.Count == 0)
        {
            print("Empty");
        }
        else
        {
            print("Theres sum in this hoe");
            foreach (InventoryItem items in currentInventory)
            {
                if (items.itemData.itemName == "Journal") 
                {
                    currentInventory.Clear();
                }

                var journalItem = Instantiate(DraggableItemPrefab, Vector3.one, Quaternion.identity); //the connecting data

                //getting tab 1
                GameObject Tab1 = tabholder.transform.GetChild(0).gameObject;
                journalItem.transform.SetParent(Tab1.transform);
                journalItem.transform.localPosition = Vector2.zero;
 
                //this is all stuff that assigns image
                var imgComponent = journalItem.gameObject.GetComponent<Image>();
                imgComponent.sprite = items.itemData.img;
                if (items.itemData.loreItem)
                {
                    journalItem.gameObject.tag = "LoreItem";
                }
                //scaling is required so that the proportions of the image stay the same
                float desiredWidth = 100; // Your target size
                float aspectRatio = items.itemData.img.rect.height / items.itemData.img.rect.width;
                journalItem.GetComponent<RectTransform>().sizeDelta = new Vector2(desiredWidth, desiredWidth * aspectRatio);

                // *** ADD THIS: Normalize the scale after sizing ***
                journalItem.transform.localScale = Vector3.one;
                //then you have to resize the rotation icon which got scaled with the parent
                var rotationIcon = journalItem.transform.GetChild(0);
                rotationIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(25,25);
                var scaleIcon = journalItem.transform.GetChild(1);
                scaleIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(25,25);
            }
        }

        Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);  
    }

    

    public void JournalScene() //SHOWS JOURNAL 
    {
        print("journal scene in carscene manager activated");
        crosshair.enabled = false;
        //Turns on the journal
        Debug.Log("Showing Journal");
        journalItem.SetActive(false);

        //changes camera angle
        playercam.GetComponent<PlayerCam>().enabled = false;
        playercam.transform.DOLocalRotate(new Vector3(11f, 20f, 1f), 1f);

        //Locks the screen and enables you to interact with the journal
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        journalActive = true;

    }

    public void ExitJournal() //DISABLES JOURNAL 
    {
        crosshair.enabled = true;
        if (journalActive)
        {
            journalItem.SetActive(true);

            playercam.transform.DOLocalRotate(new Vector3(13f, 70f, 4f), 1f)
                .OnComplete(() =>
                {
                    PlayerCam camScript = playercam.GetComponent<PlayerCam>();
                    camScript.enabled = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                });

            journalActive = false;
        }

    }

    //public void ActivateBroDiag()
    //{
    //    holdItemData.diagPos = new Vector3(-0.126f, 7.327f, -5.666f);
    //    DialogueManager.instance.TalkInteraction(holdItemData);

    //}


}
