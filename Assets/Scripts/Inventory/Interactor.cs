using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections;
using DG.Tweening;
using System.Runtime.InteropServices;

public class Interactor : MonoBehaviour
{
    private Coroutine holdProgressRoutine;
    private float holdProgress = 0f;
    private bool isHeld = false;
    private bool isDanger;
    private bool hasStartedStealing = false;
    private bool isInStealingConfirmMode;
    private bool isInWarningPeriod;
    private float warningStartTime;
    [SerializeField] private float warningDuration = 1f;
    
    public GameObject pickupUI;
    [SerializeField] private PickupUIVariants pickupUIVariants;
    private GameObject instantiatedUI;
    private GameObject pickedUpObj;

    // Speed controls
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

    private Transform highlight;
    private Transform selection;
    private interactable Interactable;
    private PlayerInput playerInput;
    private InputActionAsset inputActions;
    private GameObject interactableItem;
    [SerializeField] bool canInteract;

    private bool inventoryActive = false;
    public GameObject inventoryHUD;
    public ThirdPersonMovement movement;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;

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
    }

    private void OnEnable()
    {
        StealingManager.OnStateChanged += CheckState;
    }

    void OnDisable()
    {
        StealingManager.OnStateChanged -= CheckState;
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
            inputActions["Interacted"].Enable();
            highlight = other.transform;
            
            Interactable = other.gameObject.GetComponent<interactable>();
            interactableItem = Interactable.gameObject;
            canInteract = true;
            pickedUpObj = highlight.gameObject;
            
            if (highlight.gameObject.GetComponent<Outline>() != null)
            {
                highlight.gameObject.GetComponent<Outline>().enabled = true;
                
                if(other.gameObject.tag == "canSteal")
                {
                    SpawnPickupUI(true);
                }
                else
                {
                    SpawnPickupUI(false);
                }
            }
            else
            {
                Outline outline = highlight.gameObject.AddComponent<Outline>();
                outline.enabled = true;
                outline.OutlineColor = Color.white;
                outline.OutlineWidth = 7.0f;
                if (other.gameObject.tag == "canSteal")
                {
                    SpawnPickupUI(true);
                }
                else
                {
                    SpawnPickupUI(false);
                }
            }
        }
    }

    private void SpawnPickupUI(bool stealable)
    {
        DestroyPickupUI();
        Vector3 spawnPosition = new Vector3(0, 1.5f, .5f);
        instantiatedUI = Instantiate(pickupUI, this.transform);
        instantiatedUI.transform.localPosition = spawnPosition;

        if (stealable)
        {
            PickupUIVariants pickupUI = instantiatedUI.GetComponent<PickupUIVariants>();
            pickupUI.ChangeUI(stealable);
        }
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
            
            canInteract = false;
        }
    }

    private IEnumerator FailedStealing()
    {
        print("failing stealing");
        yield return new WaitForSeconds(1f);
        
        if (StealingManager.Instance != null)
        {
            StealingManager.Instance.StopStealin();
        }
        
        CleanupAfterInteraction();
        
        yield return new WaitForSeconds(.5f);
        inputActions["Interacted"].Enable();
        movement.moveSpeed = 6f;
        print("Input re-enabled!");
    }

    public void Interacted(InputAction.CallbackContext context)
    {
        // Add null checks at the start
        if (pickedUpObj == null || Interactable == null)
        {
            Debug.LogWarning("Interaction attempted but object or interactable is null");
            return;
        }

        if (context.started)
        {
            if (pickedUpObj.tag == "canSteal")
            {
                if (!isInStealingConfirmMode)
                {
                    print("First press: Entering stealing confirmation mode");
                    isInStealingConfirmMode = true;
                    if (StealingManager.Instance != null)
                    {
                        StealingManager.Instance.StartStealin();
                    }
                    return;
                }
                else
                {
                    print("Second press: Starting steal timer");
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
                Interactable.Interact();
            }
        }
    }

    private void StartDrain(InputAction.CallbackContext context)
    {
        if (holdProgress > 0)
        {
            if (context.canceled)
            {
                holdDirection = -1;
            }
        }
    }
    
    private IEnumerator HoldProgressLoop(float fillspeed)
    {
        print("activating hold progress loop");
        while (true)
        {
            // CRITICAL: Check if object still exists
            if (pickedUpObj == null || Interactable == null)
            {
                print("Object destroyed during hold - cleaning up");
                StopHoldRoutine();
                yield break;
            }

            if (holdDirection == +1)
            {
                holdProgress += fillspeed * Time.deltaTime;
                movement.moveSpeed = 0f;
                
                if (isDanger)
                {
                    print("is danger");
                    if (warningStartTime <= 0)
                    {
                        warningStartTime = Time.time;
                        print("WARNING! Release E now or you'll get caught!");
                    }
                    
                    if (Time.time - warningStartTime >= warningDuration)
                    {
                        print("NOOO u were CAUGHTTTT");
                        inputActions["Interacted"].Disable();
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
                print("DESTROYING PICKEDUP OBJECT");
                
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
                
                StopHoldRoutine();
                OnHoldCanceled?.Invoke();

                if (hasStartedStealing || isInStealingConfirmMode)
                {
                    if (StealingManager.Instance != null)
                    {
                        print("hold progress is less than 0, activating Stop stealing function");
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
        print("StopHoldRoutine called");
        if (holdProgressRoutine != null)
        {
            print("stopping hold progress routine");
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
            print("resetting state");
            warningStartTime = 0;
        }
    }


}