using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static StealingManager;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;

public class IntroUIManager : MonoBehaviour
{
    [Header("Public UI References")]
    public Image progressbar;
    [Tooltip("contains the dog background and dog image as children in that order")]
    public GameObject stealingUI;
    [Tooltip("Different dog sprites in order of safe to danger")]
    public List<Sprite> textures;

    private Image DogspriteRen;
    private Image BackgroundRen;

    private Image Hintimage;

    public GameObject hintUI;
    public static Sprite[] hintsprites;


    private bool hintneeded = false;


    [Header("Public References")]
    public GameObject InventoryHUD, ItemHUD;
    public GameObject loreitemhint;
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
        IntroInteractor.OnHoldProgress += UpdateHoldUI;
        IntroInteractor.OnHoldCompleted += HideHoldUI;
        IntroInteractor.OnHoldCanceled += HideHoldUI;

        StealingManager.OnStateChanged += UpdateDangerUI;
        StealingManager.OnStealingActionChanged += UpdateStealingUI;

        Mapinteractable.onPickedUp += ShowItemHUD;
        Mapinteractable.onPickedUp += rewardText;


        IntroSceneManager.OnHintNeeded += hint;
        IntroInteractor.HintNeeded += hint;
        InputManager.Pause += pausegame;
    }

    private void OnDisable()
    {
        IntroInteractor.OnHoldProgress -= UpdateHoldUI;
        IntroInteractor.OnHoldCompleted -= HideHoldUI;
        IntroInteractor.OnHoldCanceled -= HideHoldUI;

        StealingManager.OnStateChanged -= UpdateDangerUI;
        StealingManager.OnStealingActionChanged -= UpdateStealingUI;

        Mapinteractable.onPickedUp -= ShowItemHUD;
        Mapinteractable.onPickedUp -= rewardText;

        IntroSceneManager.OnHintNeeded -= hint;
        IntroInteractor.HintNeeded -= hint;
        InputManager.Pause -= pausegame;
    }

    private void Start()
    {
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
            DogspriteRen = stealingUI.transform.GetChild(1).GetComponent<Image>();
            stealingUI.SetActive(false);
        }
        else
        {
            //stealing ui is not set up for stealing
            stealingUI.SetActive(false);
        }
    }

    void ShowItemHUD(Item itemData)
    {
        Debug.Log("Showing Item HUD");
        //make noise
        SoundManager.Instance.PlayAudio(pickupsound);
        if(itemData.loreItem == true)
        {
            print("this is a lore item");
            StartCoroutine(animatejournalHud());
        }
        else
        {
            print("it isnt a lore item");
        }

        StartCoroutine(AnimateItemHUD());
        itemImg.sprite = itemData.img;
        itemImg.preserveAspect = true;
        itemText.text = itemData.itemName + "!";
    }

    private IEnumerator AnimateItemHUD()
    {
        ItemHUD.transform.DOLocalMoveX(198.3f, 1f);
        yield return new WaitForSeconds(2f);
        ItemHUD.transform.DOLocalMoveX(542f, 1f);
    }
    private IEnumerator animatejournalHud()
    {
        print("animating journal lore pickip indicator");
        yield return new WaitForSeconds(2f);
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
            stealingUI.SetActive(true);
        }
        else
        {
             stealingUI.SetActive(false);
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
        stealingUI.SetActive(false);
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
        //this will need to change the sprite, and then the color of the background sprite
        BackgroundRen.color = Color.green;
        DogspriteRen.sprite = textures[0];

    }
    private void SusUI()
    {
        BackgroundRen.color = Color.yellow;
        DogspriteRen.sprite = textures[1];
    }
    private void DangerUI()
    {
        BackgroundRen.color = Color.red;
        DogspriteRen.sprite = textures[2];
    }

    private void hint(string str)
    {
        //print("hint calleed");
        print(str);
        Hintimage = hintUI.transform.GetChild(0).GetComponent<Image>(); 
        hintneeded = true;
        if (str == "nojournal")
        {
            Hintimage.sprite = hintsprites[0];
            print(hintsprites[0].name);

        }
        if(str == "willsteal")
        {
            Hintimage.sprite = hintsprites[1];
            print(hintsprites[0].name);
        }
        StartCoroutine(Hintroutine());
    }

    private IEnumerator Hintroutine()
    {
        //print("starting hint routine with " + Hintimage.sprite.name);
        hintUI.SetActive(true);
        hintUI.transform.DOLocalMoveY(300, 1.5f);
        yield return new WaitForSeconds(1f);
        hintUI.transform.DOLocalMoveY(0, 3f);
        //hintUI.GetComponentInChildren<TextMeshProUGUI>().text = prompt;
        yield return new WaitForSeconds(.4f);
        hintUI.SetActive(false);
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