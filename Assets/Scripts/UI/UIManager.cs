using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static StealingManager;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Public UI References")]
    public Image progressbar;
    [Tooltip("contains the dog background and dog image as children in that order")]
    private GameObject stealingUI;
    public GameObject StealingCanvas;
    public TextMeshProUGUI statelabel;
    [Tooltip("Different dog sprites in order of safe to danger")]
    public List<Sprite> textures;
    private Color previouscolor;

    private Image DogspriteRen;
    private Image BackgroundRen;



    private Image Hintimage;
    public GameObject loreitemhint;

    public GameObject hintUI;
    public static Sprite[] hintsprites;


    private bool hintneeded = false;


    [Header("Public References")]
    public GameObject InventoryHUD, ItemHUD;
    public TextMeshProUGUI itemText, RewardText, moneyText;
    public Image itemImg;
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
        stealingUI = StealingCanvas.transform.GetChild(1).gameObject;
        if(progressbar != null)
        {
            progressbar.fillAmount = 0;
            progressbar.enabled = false;
        }
       
        SetSprites();
       
        hintUI.SetActive(false);
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
        
        StartCoroutine(AnimateItemHUD());
        itemImg.sprite = itemData.img;
        itemImg.preserveAspect = true;
        itemText.text = itemData.itemName + "!";
        if(itemData.loreItem == true)
        {
            StartCoroutine(animatejournalHud());
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
        print("animating journal lore pickip indicator");
        yield return new WaitForSeconds(2.6f);
        loreitemhint.transform.DOLocalMoveX(198.3f, 1f);
        yield return new WaitForSeconds(2f);
        loreitemhint.transform.DOLocalMoveX(542f, 1f);
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

    // private void hint(string str)
    // {
    //     //print("hint calleed");
    //     print(str);
    //     Hintimage = hintUI.transform.GetChild(0).GetComponent<Image>(); 
    //     hintneeded = true;
    //     if (str == "nojournal")
    //     {
    //         Hintimage.sprite = hintsprites[0];
    //         print(hintsprites[0].name);

    //     }
    //     if(str == "willsteal")
    //     {
    //         Hintimage.sprite = hintsprites[1];
    //         print(hintsprites[0].name);
    //     }
    //     StartCoroutine(Hintroutine());
    // }

    // private IEnumerator Hintroutine()
    // {
    //     //print("starting hint routine with " + Hintimage.sprite.name);
    //     hintUI.SetActive(true);
    //     hintUI.transform.DOLocalMoveY(300, 1.5f);
    //     yield return new WaitForSeconds(1f);
    //     hintUI.transform.DOLocalMoveY(0, 3f);
    //     //hintUI.GetComponentInChildren<TextMeshProUGUI>().text = prompt;
    //     yield return new WaitForSeconds(.4f);
    //     hintUI.SetActive(false);
    // }

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