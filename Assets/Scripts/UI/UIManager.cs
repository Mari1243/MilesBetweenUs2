using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static StealingManager;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class UIManager : MonoBehaviour
{
    [Header("Public UI References")]
    public Image progressbar;
    [Tooltip("contains the dog background and dog image as children in that order")]
    private GameObject stealingUI;
    public GameObject StealingCanvas;
    public Canvas journalCanvas;
    public GameObject journal;
    public TextMeshProUGUI statelabel;
    [Tooltip("Different dog sprites in order of safe to danger")]
    public List<Sprite> textures;
    private Color previouscolor;

    private Image DogspriteRen;
    private Image BackgroundRen;




    private Image Hintimage;
    public GameObject LoreitemPopup;
    private GameObject journalnotification;
    public static Sprite[] hintsprites;


    private bool hintneeded = false;
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
        InputManager.OpenJournal += inventory;

        StealingManager.OnStateChanged += UpdateDangerUI;
        StealingManager.OnStealingActionChanged += UpdateStealingUI;

        interactable.onPickedUp += ShowItemHUD;
        interactable.onPickedUp += rewardText;
        interactable.coinPickedUp += collectCoin;
        interactable.coinPickedUp += ShowItemHUD;
        Interactor.StealWarning += dangerState;

        //IntroSceneManager.OnHintNeeded += hint;
        //Interactor.HintNeeded += hint;
        InputManager.Pause += pausegame;
    }

    private void OnDisable()
    {
        Interactor.OnHoldProgress -= UpdateHoldUI;
        Interactor.OnHoldCompleted -= HideHoldUI;
        Interactor.OnHoldCanceled -= HideHoldUI;
        Interactor.StealWarning -= dangerState;
        InputManager.OpenJournal -= inventory;

        StealingManager.OnStateChanged -= UpdateDangerUI;
        StealingManager.OnStealingActionChanged -= UpdateStealingUI;

        interactable.onPickedUp -= ShowItemHUD;
        interactable.onPickedUp -= rewardText;
        interactable.coinPickedUp -= collectCoin;
        interactable.coinPickedUp -= ShowItemHUD;
        
        //IntroSceneManager.OnHintNeeded -= hint;
        //Interactor.HintNeeded -= hint;
        InputManager.Pause -= pausegame;
    }

    private void inventory()
    {
        if(journalCanvas != null)
        {
            if(!journalopen)
            {
                journalopen = true;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                print("open inventory");
                journalCanvas.enabled = true;
                StartCoroutine(journalIN());

            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                StartCoroutine(journalOUT());
                journalopen = false;
            }
        }
    }

    private IEnumerator journalIN()
    {
        journal.transform.DOKill();
        journal.transform.DOLocalMoveY(-77, 1f).SetUpdate(true).SetEase(Ease.OutCirc);
        yield return new WaitForSecondsRealtime(.3f);
        Time.timeScale = 0f;

    }
    private IEnumerator journalOUT()
    {
        journal.transform.DOKill();
        journal.transform.DOLocalMoveY(-2000, .7f).SetUpdate(true).SetEase(Ease.InCirc);
        yield return new WaitForSecondsRealtime(.3f);
        Time.timeScale = 1f;

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

    private void collectCoin(Item itemdata)
    {
        Debug.Log("coin picked up!");
        InventoryManager.instance.moneyAmount += .50f;
        moneyText.text = "$" + InventoryManager.instance.moneyAmount.ToString("f2");
    }




}