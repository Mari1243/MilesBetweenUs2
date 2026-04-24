using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class Interactor : MonoBehaviour
{

    [Header("stealing")]
        private Coroutine holdProgressRoutine;
        private float holdProgress = 0f;
        private bool isHeld = false;
        private bool isDanger;
        private bool hasStartedStealing = false;
        private bool isInStealingConfirmMode;
        private bool isInWarningPeriod;
        private float warningStartTime;
        [SerializeField] private float warningDuration = 1f;

        public static event Action<int> StealStep;


    [Header("Pickup References")]
        public GameObject pickupUi;
        [SerializeField] private PickupUIVariants pickupUIVariants;
        private GameObject instantiatedUI;
        private GameObject pickedUpObj;


    [Header("Speed Controls")]
        [SerializeField] private float fillSpeedPickup = 0.7f;
        [SerializeField] private float fillSpeedSteal = .2f;
        [SerializeField] private float drainSpeed = 0.2f;
        private int holdDirection = 0;

    // Events
    public static event Action<float> OnHoldProgress;
    public static event Action OnHoldCompleted;
    public static event Action OnHoldCanceled;
    public static event Action<bool> StealWarning;
    public static event Action<string> HintNeeded;
    public static event Action<int> OnStartedStealing;
    public static event Action OnStopStealing;


    private Transform highlight;
    private Transform selection;
    private interactable Interactable;
    private PlayerInput playerInput;
    //private InputActionAsset inputActions;
    private GameObject interactableItem;
    [SerializeField] bool canInteract;

    private bool inventoryActive = false;
    public GameObject inventoryHUD;
    public ThirdPersonMovement movement;

    //to prevent double fire
    private int lastInteractFrame = -1;

    private void OnEnable()
{
    StealingManager.OnStateChanged += CheckState;
    InputManager.OnInteractStarted += HandleInteractStarted;
    InputManager.OnInteractCanceled += HandleInteractCanceled;
}

void OnDisable()
{
    StealingManager.OnStateChanged -= CheckState;
    InputManager.OnInteractStarted -= HandleInteractStarted;
    InputManager.OnInteractCanceled -= HandleInteractCanceled;
}

public void alternativeInteract(GameObject obj)
{
    pickedUpObj = obj;
    HandleInteractStarted();
}

private void HandleInteractStarted()
{
    //check to prevent double firing in build
    if (Time.frameCount == lastInteractFrame) return;
    lastInteractFrame = Time.frameCount;

    // Add null checks at the start
        if (pickedUpObj == null || Interactable == null)
        {
            Debug.LogWarning("Interaction attempted but object or interactable is null");
            return;
        }
            DestroyPickupUI();
            if (pickedUpObj.tag == "canSteal")
            {
                if (!isInStealingConfirmMode)
                {
                    //print("First press: Entering stealing confirmation mode");
                    isInStealingConfirmMode = true;
                    OnStartedStealing?.Invoke(StealableItemBehavior.instance.camIndex);

                    if (StealingManager.Instance != null)
                    {
                        StealingManager.Instance.StartStealin(StealableItemBehavior.instance.camIndex,StealableItemBehavior.instance.defaultCamIndex);
                        StealStep?.Invoke(1);
                    }
                    return;
                }
                else
                {
                   
                    if (holdProgressRoutine == null)
                    {
                        holdDirection = +1;
                        hasStartedStealing = true;
                        holdProgressRoutine = StartCoroutine(HoldProgressLoop(fillSpeedSteal));
                    }
                    else
                    {
                        holdDirection = +1;
                    }
                }
            }    
            else if (pickedUpObj.tag == "canPickUp" || pickedUpObj.tag == "coin")
            {
               if (canInteract)
                {
                    pickedUpObj = highlight.gameObject;
                    Interactable.Interact(); 
                    Destroy(instantiatedUI);
                }
            }
            else if (canInteract)
            {
                //print("calling interactable.interact");
                Interactable.Interact();
            }
    // move the contents of your Interacted() method here
    // you no longer need the InputAction.CallbackContext parameter
}

private void HandleInteractCanceled()
{
    // move the contents of your StartDrain() method here
      if (holdProgress > 0)
        {
            holdDirection = -1;
        }
}
    
  private void Start()
{
    pickedUpObj = null;
    Interactable = null;
    canInteract = false;
    isHeld = false;
}
    

    private void OnTriggerEnter(Collider other)
    {
        // Clean up previous highlight
        if (highlight != null)
        {
            if (highlight.gameObject != null && highlight.gameObject.GetComponent<Outline>() != null)
            {
                highlight.gameObject.GetComponent<Outline>().enabled = false;
            }
            highlight = null;
        }

        if (other.GetComponent<interactable>() != null)
        {
            //inputActions["Interacted"].Enable();
            highlight = other.transform;
            
            Interactable = other.gameObject.GetComponent<interactable>();
            interactableItem = Interactable.gameObject;
            canInteract = true;
            pickedUpObj = highlight.gameObject;
            
            if (highlight.gameObject.GetComponent<Outline>() != null)
            {
                highlight.gameObject.GetComponent<Outline>().enabled = true;
                checkstate(other.gameObject);
            }
            else
            {
                Outline outline = highlight.gameObject.AddComponent<Outline>();
                outline.enabled = true;
                outline.OutlineColor = Color.yellow;
                outline.OutlineWidth = 7.0f;
                checkstate(other.gameObject);
            }
        }
    }

    private void checkstate(GameObject other)
    {
        print("checking state");
        if (other.tag == "canSteal")
        {
            SpawnPickupUI("canSteal");
        }
        else if (other.tag == "END")
        {
            SpawnPickupUI("END");
        }
        else
        {
            SpawnPickupUI("canInteract");
        }
    }

    private void SpawnPickupUI(string str)
{
    Debug.Log("SpawnPickupUI: " + str);
    DestroyPickupUI();
    Vector3 spawnPosition = new Vector3(0, 1f, .5f);
    instantiatedUI = Instantiate(pickupUi, this.transform);
    instantiatedUI.transform.localPosition = spawnPosition;

    PickupUIVariants pickupUI = instantiatedUI.GetComponent<PickupUIVariants>();
    if (pickupUI == null)
    {
        Debug.LogError("PickupUIVariants component missing on instantiated prefab!");
        return;
    }
    if (pickupUI.stealSprite == null) Debug.LogWarning("stealSprite is null on prefab!");
    pickupUI.ChangeUI(str);
}
    public void DestroyPickupUI()
    {
        if (instantiatedUI != null)
        {
            Destroy(instantiatedUI);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<interactable>() != null)
        {
            // Clean up outline
            if (highlight != null && highlight.gameObject != null)
            {
                Outline outline = highlight.gameObject.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }
            
            Debug.Log("can no longer interact");
            
            // Clean up UI
            if (instantiatedUI != null)
            {
                Destroy(instantiatedUI);
                instantiatedUI = null;
            }

            //check if im stealing and if i am get me tf out!
            if (isInStealingConfirmMode)
            {
                StartCoroutine(FailedStealing());
                print("on trigger exit stopped stealing??");
            }
            
            canInteract = false;
        }
    }

    private IEnumerator FailedStealing()
    {
        print("ENDING stealing");
        yield return new WaitForSeconds(1f);

        OnStopStealing?.Invoke();
        
        CleanupAfterInteraction();
        
        yield return new WaitForSeconds(.5f);
        //inputActions["Interacted"].Enable();
        movement.moveSpeed = 6f;
        print("Input re-enabled!");
    }


    
    private IEnumerator HoldProgressLoop(float fillspeed)
    {
        while (true)
        {
            // CRITICAL: Check if object still exists
            if (pickedUpObj == null || Interactable == null)
            {
                StopHoldRoutine();
                yield break;
            }

            if (holdDirection == +1)
            {
                holdProgress += fillspeed * Time.deltaTime;
                movement.moveSpeed = 0f;
                
                if (isDanger)
                {
                    if (warningStartTime <= 0)
                    {
                        warningStartTime = Time.time;
                    }
                    
                    if (Time.time - warningStartTime >= warningDuration)
                    {
                        //inputActions["Interacted"].Disable();
                        StealStep?.Invoke(3);
                        StartCoroutine(FailedStealing());
                        StopHoldRoutine();
                        warningStartTime = -999f;
                        yield break;
                    }
                }
            }
            else if (holdDirection == -1)
            {
                holdProgress -= drainSpeed * Time.deltaTime;
                movement.moveSpeed = 6f;
            }

            holdProgress = Mathf.Clamp01(holdProgress);
            OnHoldProgress?.Invoke(holdProgress);

            if (holdProgress >= 1f)
            {
                movement.moveSpeed = 6f;
                
                // Call interact BEFORE cleanup
                if (Interactable != null)
                {
                    Interactable.Interact();
                }
                
                // Clean up UI
                if (instantiatedUI != null)
                {
                    Destroy(instantiatedUI);
                    instantiatedUI = null;
                }

                // Clean up and reset
                StopHoldRoutine();
                CleanupAfterInteraction();
                
                yield break;
            }

            if (holdProgress <= 0f && holdDirection == -1)
            {
                //ran out
                StopHoldRoutine();
                OnHoldCanceled?.Invoke();
                StealStep?.Invoke(4);
                CleanupAfterInteraction();
                if (hasStartedStealing || isInStealingConfirmMode)
                {
                    if (StealingManager.Instance != null)
                    {
                        StealingManager.Instance.StopStealin();
                    }
                    hasStartedStealing = false;
                    isInStealingConfirmMode = false;
                    
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
        isInWarningPeriod = false;
        OnHoldCompleted?.Invoke();
        
        if (pickedUpObj != null)
        {  
            if (StealingManager.Instance != null)
            {
                StealingManager.Instance.StopStealin();
            }
            hasStartedStealing = false;
            isInStealingConfirmMode = false;
        }
    }

    // NEW METHOD: Centralized cleanup
    private void CleanupAfterInteraction()
    {
        isHeld = false;
        pickedUpObj = null;
        Interactable = null;
        hasStartedStealing = false;
        isInStealingConfirmMode = false;
        holdProgress = 0;
        holdDirection = 0;
        canInteract = false;
        highlight = null;
        print("clean up after interaction");
        StealStep?.Invoke(6);
    }

    private void CheckState(StealingManager.DangerState newState)
    {
        if (newState == StealingManager.DangerState.Caught)
        {
            isDanger = true;
            StealStep?.Invoke(2);
        }
        else
        {
            isDanger = false;
            warningStartTime = 0;
        }
    }


}