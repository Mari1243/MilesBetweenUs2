using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static StealingManager;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class UIManager : MonoBehaviour
{
    [Header("Public UI References")]
    public Image progressbar;
    [Tooltip("contains the dog background and dog image as children in that order")]
    private GameObject stealingUI;
    public GameObject StealingCanvas;
    private Canvas journalCanvas;
    private GameObject journal;
    public TextMeshProUGUI statelabel;
    [Tooltip("Different dog sprites in order of safe to danger")]
    public List<Sprite> textures;
    private Color previouscolor;

    private Image DogspriteRen;
    private Image BackgroundRen;


    public GameObject NotifPopup;
    public GameObject LoreitemPopup;
    private GameObject journalnotification;
    public static Sprite[] hintsprites;


    private bool journalopen = false;

    [Header("Public References")]
    public GameObject InventoryHUD, ItemHUD;
    public TextMeshProUGUI itemText, RewardText, moneyText, loreText;
    public Image itemImg;
    public Image loreImg;
    public AudioClip pickupsound;

    //for pause
    private bool isPaused = false;
    public static event System.Action<bool> onPause;

    //private refs to dog sprites
    private Texture2D CurrentDog;

   
    private void OnEnable()
    {
        Interactor.OnHoldProgress += UpdateHoldUI;
        Interactor.OnHoldCompleted += HideHoldUI;
        Interactor.OnHoldCanceled += HideHoldUI;


        StealingManager.OnStateChanged += UpdateDangerUI;
        StealingManager.OnStealingActionChanged += UpdateStealingUI;

        interactable.onPickedUp += ShowItemHUD;
        InventoryManager.AddedItem += ShowItemHUD;
        interactable.onPickedUp += rewardText;
        Interactor.StealWarning += dangerState;

        InputManager.Pause += pausegame;

        GasStationManager.journalNotif += INjournalNotif;
        InputManager.OpenJournal += OUTjournalNotif;


    }

    private void OnDisable()
    {
        Interactor.OnHoldProgress -= UpdateHoldUI;
        Interactor.OnHoldCompleted -= HideHoldUI;
        Interactor.OnHoldCanceled -= HideHoldUI;
        Interactor.StealWarning -= dangerState;

        StealingManager.OnStateChanged -= UpdateDangerUI;
        StealingManager.OnStealingActionChanged -= UpdateStealingUI;

        interactable.onPickedUp -= ShowItemHUD;
        interactable.onPickedUp -= rewardText;
        InventoryManager.AddedItem -= ShowItemHUD;
        InputManager.Pause -= pausegame;

        GasStationManager.journalNotif -= INjournalNotif;
        InputManager.OpenJournal -= OUTjournalNotif;
    }


    private void Start()
    {
        if(LoreitemPopup != null)
        {
            journalnotification = LoreitemPopup.transform.GetChild(0).gameObject;
            journalnotification.SetActive(false);
        }
        stealingUI = StealingCanvas.transform.GetChild(1).gameObject;
        if(progressbar != null)
        {
            progressbar.fillAmount = 0;
            progressbar.enabled = false;
        }
       
        SetSprites();

        if (journalCanvas != null){
            journalCanvas.enabled = false;
        }

        LoreitemPopup.SetActive(false);
        journalCanvas = GameObject.Find("journalCanvas").GetComponent<Canvas>();
        journal = journalCanvas.transform.GetChild(0).gameObject;

        NotifPopup.transform.localScale = new Vector3(0, 0, 0);

    }


    private void dangerState(bool status)
    {
        DOTweenAnimation test = stealingUI.gameObject.GetComponent<DOTweenAnimation>();
        
        if (status)
        {
            if(test != null)
            {
                print("Found DOTweenAnimation, playing...");
                test.DORestart();
                test.DOPlay();
            }
        }
        else
        {
            if(test != null)
            {
                test.DOPause();
            }
        }
    }

    void INjournalNotif()
    {
        NotifPopup.transform.DOScale(new Vector3(.4f, .4f, .4f), .5f).SetEase(Ease.InBounce) ;
    }
    void OUTjournalNotif()
    {
        NotifPopup.transform.DOScale(new Vector3(0,0,0), 1f);

    }

    private void SetSprites()
    {
        if(textures != null && textures.Count >= 3)
        {
            BackgroundRen = stealingUI.transform.GetChild(0).GetComponent<Image>();
            print(BackgroundRen.gameObject.name);
            DogspriteRen = stealingUI.transform.GetChild(1).GetComponent<Image>();
            StealingCanvas.SetActive(false);
        }
        else
        {
            //stealing ui is not set up for stealing
            StealingCanvas.SetActive(false);
        }
    }

    void ShowItemHUD(Item itemData)
    {
        Debug.Log("Showing Item HUD");
        //make noise
        SoundManager.Instance.PlayAudio(pickupsound);
        //setting the image and name
       
        if(itemData.loreItem == true)
        {
            loreImg.sprite = itemData.img;
            loreImg.preserveAspect = true;
            loreText.text = itemData.itemName + "!";
            StartCoroutine(animatejournalHud());
        }
        else
        {
            print("trying to animate reg item");
            itemImg.sprite = itemData.img;
            itemImg.preserveAspect = true;
            itemText.text = itemData.itemName + "!";
            StartCoroutine(AnimateItemHUD());
        }
       
    }

    private IEnumerator AnimateItemHUD()
    {
        ItemHUD.transform.DOLocalMoveX(198.3f, 1f);
        yield return new WaitForSeconds(2.5f);
        ItemHUD.transform.DOLocalMoveX(542f, 1f);
    }

    private IEnumerator animatejournalHud()
    {
        print("lore item is popping up");
        LoreitemPopup.SetActive(true);
        
        print("animating journal lore pickip indicator");
        yield return new WaitForSeconds(2.6f);
        LoreitemPopup.SetActive(false);
        
        //if journal is not active
        yield return new WaitForSeconds(1f);
        journalNotification();
    }

    private void journalNotification()
    {

        journalnotification.SetActive(true);
    }

    public void rewardText(Item itemdata)
    {
        if (itemdata.loreItem)
        {
            InventoryManager.instance.loreItemAmt++;

            RewardText.text = InventoryManager.instance.loreItemAmt.ToString() + "/3";
        }
        
    }

    private void UpdateStealingUI(bool stealcheck)
    {
        if (stealcheck == true)
        {
             StealingCanvas.SetActive(true);
        }
        else
        {
             StealingCanvas.SetActive(false);
        }
    }

    private void UpdateHoldUI(float progress)
    {
        if (progressbar.enabled == false)
        {
            progressbar.enabled = true;
        }
        progressbar.fillAmount = progress;
    }

    private void HideHoldUI()
    {
        print("hiding hold UI");
        progressbar.fillAmount = 0;
        progressbar.enabled = false;
        StealingCanvas.SetActive(false);
    }


    void UpdateDangerUI(DangerState newState)
    {
        switch (newState)
        {
            case DangerState.Safe: SafeUI(); break;
            case DangerState.Suspicious: SusUI(); break;
            case DangerState.Caught: DangerUI(); break;
        }
    }
    private void SafeUI()
{
    DOVirtual.Color(previouscolor, Color.green, 1, (value) =>
    {
        BackgroundRen.color = value;
    });
    previouscolor = Color.green;
    DogspriteRen.sprite = textures[0];
    statelabel.text = "SAFE";
}

private void SusUI()
{
    DOVirtual.Color(previouscolor, Color.yellow, 1, (value) =>
    {
        BackgroundRen.color = value;
    });
    previouscolor = Color.yellow;
    DogspriteRen.sprite = textures[1];
    statelabel.text = "CAUTION";
}

private void DangerUI()
{
    DOVirtual.Color(previouscolor, Color.red, 1, (value) =>
    {
        BackgroundRen.color = value;
    });
    previouscolor = Color.red;
    DogspriteRen.sprite = textures[2];
    statelabel.text = "STOP!";
}

    
    //listening for pause
    private void pausegame()
    {
        if (!isPaused)
        {
            print("pausing");
                isPaused = true;
                onPause?.Invoke(isPaused);
        }
        else
        {
            print("playing");
            isPaused = false;
            onPause?.Invoke(isPaused);
        }
    }






}