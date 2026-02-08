using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;
public class IntroInteractor : MonoBehaviour
{
    private Coroutine holdProgressRoutine;
    private float holdProgress = 0f;
    private bool isHeld = false;
     private bool isDanger;
    private bool hasStartedStealing = false;
    
    public GameObject pickupUI;
    private GameObject instantiatedUI;

    private GameObject pickedUpObj;

    // Speed controls
    [SerializeField] private float fillSpeed = 0.5f;
    [SerializeField] private float drainSpeed = 0.2f;
    private int holdDirection = 0;

    // Events to communicate with UI or sound scripts
    public static event Action<float> OnHoldProgress;   // Sends progress 0–1
    public static event Action OnHoldCompleted;
    public static event Action OnHoldCanceled;
    public static event Action<string> HintNeeded;

    //END changes

    private Transform highlight;
    private Transform selection;
    
    private Mapinteractable Interactable;
    private PlayerInput playerInput;
    private InputActionAsset inputActions;
    private GameObject interactableItem;
    [SerializeField]bool canInteract;

    private bool inventoryActive = false;
    public GameObject inventoryHUD;
    public ThirdPersonMovement movement; 
    private void Awake()
    {
      
        playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;

        // Subscribe to the input actions
        inputActions["Interacted"].started += Interacted;
        inputActions["Interacted"].canceled += StartDrain;


        if (inputActions["Interacted"] == null)
        {
            Debug.Log("Cant find this input action");
        }
        if (inputActions["Drop"] == null)
        {
            Debug.Log("Cant find this input action");
        }
    }
    private void Start()
    {
        inputActions["Interacted"].Disable();
        pickedUpObj = null;
        Interactable = null;
        canInteract = false;
        isHeld = false;

        //Assign pickup ui if it CAN
    }

    private void OnEnable()
    {
        StealingManager.OnStateChanged += CheckState;
        InputManager.drop += DropItem;
        InputManager.openInventory += OpenInventory;
        Mapinteractable.showJournal += disableinput;
        JournalTutorial.hideJournal += enableinput;
    }

    void OnDisable()
    {
       StealingManager.OnStateChanged -= CheckState;
        InputManager.drop -= DropItem;
        InputManager.openInventory -= OpenInventory;
        Mapinteractable.showJournal -= disableinput;
        JournalTutorial.hideJournal -= enableinput;
    }

    private void disableinput()
    {
        this.inputActions["Interacted"].Disable();
        this.inputActions["Move"].Disable(); // if you have this
        this.inputActions["Drop"].Disable();
        print("disabled input actions");
    }

    private void enableinput()
    {
        this.inputActions["Interacted"].Enable();
        this.inputActions["Move"].Enable();
        this.inputActions["Drop"].Enable();
    }


    private void OnTriggerEnter(Collider other)
{
    if (highlight != null)
    {
        highlight.gameObject.GetComponent<Outline>().enabled = false;
        highlight = null;
    }

    if (other.GetComponent<Mapinteractable>() != null)
    {
        inputActions["Interacted"].Enable();
        highlight = other.transform;
        
        //crucial
        Interactable = other.gameObject.GetComponent<Mapinteractable>();
        interactableItem = Interactable.gameObject;
        canInteract = true;

        // Removed the redundant check and the selection comparison
        pickedUpObj = highlight.gameObject;
        
        if (highlight.gameObject.GetComponent<Outline>() != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = true;
            //E to interact indicator
            Vector3 spawnPosition = new Vector3(0, 1.5f, .5f);
            instantiatedUI = Instantiate(pickupUI, this.transform);
            instantiatedUI.transform.localPosition = spawnPosition;
        }
        else
        {
            Outline outline = highlight.gameObject.AddComponent<Outline>();
            outline.enabled = true;
            outline.OutlineColor = Color.white;
            outline.OutlineWidth = 7.0f;
            
            // Add UI here too since outline was just created
            Vector3 spawnPosition = new Vector3(0, 1.5f, .5f);
            instantiatedUI = Instantiate(pickupUI, this.transform);
            instantiatedUI.transform.localPosition = spawnPosition;
        }
    }
}

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Mapinteractable>() != null && highlight.gameObject.GetComponent<Outline>())
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            Debug.Log("can no longer interact");
            //set pickup ui false
            Destroy(instantiatedUI);
            
            canInteract = false;
        }
    }

    public void DropItem()
    {
        print("dropping");
        if (pickedUpObj.tag != "canTalk" && isHeld )
        {
            pickedUpObj.transform.parent = null;

            pickedUpObj.transform.DOMoveY(pickedUpObj.transform.position.y - 2f, 0.3f).SetEase(Ease.InQuad);
            pickedUpObj.transform.DOPunchPosition(Vector3.up * 0.3f, 0.5f, 1, .5f).SetDelay(0.3f);


            pickedUpObj = null;
            isHeld = false;
        }
     
    }
   
    public void Interacted(InputAction.CallbackContext context)
    {

        if (context.started)
        {
            if (pickedUpObj.tag == "canPickUp" || pickedUpObj.tag == "coin")
            {
                if (canInteract)
                {
                    pickedUpObj = highlight.gameObject;
                    Interactable.Interact(); 
                    Destroy(instantiatedUI);
                }
            }
            else if(pickedUpObj.tag == "canSteal")
            {
                if (holdProgressRoutine == null)
                    {
                        holdDirection = +1;
                        holdProgressRoutine = StartCoroutine(HoldProgressLoop());
                    }
                    else
                    {
                        holdDirection = +1;
                    }
                    
            }
        }

    }

    private void StartDrain(InputAction.CallbackContext context)
    {
        print("starting drain");
        if (holdProgress > 0)
        {
            // RELEASE E + object is stealable → begin draining
            if (context.canceled)
            {
                holdDirection = -1; // start draining
            }
        }
        else
        {
            print("TRYING TO DRAIN");
        }
    }
    
   private IEnumerator HoldProgressLoop()
    {
        print("activating hold progress loop");
        while (true)
        {
            if (holdDirection == +1)
            {
                holdProgress += fillSpeed * Time.deltaTime;
                movement.moveSpeed = 0f;
                // Check if danger just started
            }
            else if (holdDirection == -1)
            {
                holdProgress -= drainSpeed * Time.deltaTime;
                movement.moveSpeed = 6f;
            }

            holdProgress = Mathf.Clamp01(holdProgress);

            // Send value to UI (if present)
            OnHoldProgress?.Invoke(holdProgress);

            // If fully filled → pick up
            if (holdProgress >= 1f)
            {
                movement.moveSpeed = 6f;
                print("DESTROYING PICKEDUP OBJECT");
                Interactable.Interact();   // adds to inventory + destroys object
                Destroy(instantiatedUI);

                // 2️⃣ Stop coroutine + cleanup
                StopHoldRoutine();

                // 3️⃣ Finally free the hands
                isHeld = false;
                pickedUpObj = null;
                Interactable = null;
                hasStartedStealing = false;
                holdProgress = 0;
                holdDirection = 0;

                yield break;
            }

            // If fully drained → stop to save CPU
            if (holdProgress <= 0f && holdDirection == -1)
            {
                //print("I have not picked up le objecttttt");
                StopHoldRoutine();
                OnHoldCanceled?.Invoke();

                if (hasStartedStealing)
                {
                    StealingManager.Instance.StopStealin();
                    hasStartedStealing = false;
                }


                yield break;
            }

            yield return null;

        }
    }

    private void StopHoldRoutine()
    {
        if (holdProgressRoutine != null)
        {
            StopCoroutine(holdProgressRoutine);
            holdProgressRoutine = null;
        }
        holdDirection = 0;
        OnHoldCompleted?.Invoke();
        Destroy(instantiatedUI);
        
        if (pickedUpObj.tag == "canSteal")
        {
            StealingManager.Instance.StopStealin();
            hasStartedStealing = false; // Reset the flag
        }
    }

    private void CheckState(StealingManager.DangerState newState)
    {
        print("check state activated");
        if (newState == StealingManager.DangerState.Caught)
        {
            isDanger = true;
            print("state is danger");
        }
        else
        {
            isDanger = false;
        }
    }

    public void OpenInventory()
    {
        if (inventoryActive == false)
        {
            inventoryHUD.transform.DOScale(new Vector3(1, 1, 1), .5f).SetEase(Ease.OutBounce);
            inventoryHUD.transform.DOLocalMoveY(-195.6f, .5f).SetEase(Ease.OutBounce);
            inventoryActive = true;
        }
        else if (inventoryActive == true)
        {
            inventoryHUD.transform.DOLocalMoveY(-307.1f, .4f).SetEase(Ease.InOutQuart);
            inventoryHUD.transform.DOScale(new Vector3(0, 0, 0), .5f).SetEase(Ease.InOutQuart);
            inventoryActive = false;
        }
    }
}