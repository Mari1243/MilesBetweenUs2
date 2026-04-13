using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

interface IInteractable
{
    public void Interact();
}

public class InputManager : MonoBehaviour
{
    //REMOVE THIS
    private bool Oninstructions;
    public static event System.Action<bool> instructions;
    public static event System.Action Restart;

    public static InputManager Instance;
    private PlayerInput playerInput;
    public InputActionAsset inputActions;
    
    // Events
    public static event Action Pause;
    public static event System.Action<bool> onRotateChanged;
    public static event System.Action<bool> onScaleChanged;
    public static event System.Action OpenJournal;
    public static event Action OnInteractStarted;
    public static event Action OnInteractCanceled;


    public static event Action drop;
    public static event Action openInventory;
    // Bools
    public static bool canRotate = false;
    public static bool canScale = false;

    public bool JournalOpen = false;
    
    private void Awake()
    {
        // Getting the input assets
        playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;
        
        // Enabled both input action maps
        inputActions.FindActionMap("Player").Enable();
        inputActions.FindActionMap("UI").Enable();
        
        // Subscribe to the input actions
        inputActions["DeleteSave"].performed += onClearSave;

        this.enabled = true;
    }
    
    private void OnEnable()
    {
        // Ensure actions are enabled (in case component was disabled/re-enabled)
        if (inputActions != null)
        {
            inputActions.Enable();
            inputActions["Exit"].performed += PauseGame;
            inputActions["checkJournal"].performed += checkJournal;

            inputActions["ToggleInstructions"].performed += ToggleMenu;
            inputActions["Restart"].performed += RestartScene;

            ToggleJournal.OnJournalOpened +=togglebool;
            ToggleJournal.OnJournalClosed +=togglebool;

            inputActions["Drop"].performed += DropItem;
            inputActions["OpenInventory"].performed += OpenInventory;

            //for interactor
            inputActions["Interacted"].started += ctx => OnInteractStarted?.Invoke();
            inputActions["Interacted"].canceled += ctx => OnInteractCanceled?.Invoke();
        
        }
    }
    
    //changed to disable player interaction when the journal is open
    private void togglebool()
    {
    if (!JournalOpen)
    {
        print("toggling player map off");
        JournalOpen = true;
        //maybe instead of disabling UI we just disable interact
        if (inputActions["Interacted"].enabled)
        {
            inputActions["Interacted"].Disable();
            inputActions["Interact"].Disable();
        }
        
        //inputActions.FindActionMap("UI").Disable();
        //inputActions.FindActionMap("Player").Disable();
    }
    else
    {
         print("toggling player map on");
        JournalOpen = false;
            if (!inputActions["Interacted"].enabled)
            {
                inputActions["Interacted"].Enable();
                inputActions["Interact"].Enable();
            }
        // inputActions.FindActionMap("UI").Enable();
        // inputActions.FindActionMap("Player").Disable();
    }
    }
    
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (inputActions != null)
        {
            inputActions["Exit"].performed -= PauseGame;

            inputActions["checkJournal"].performed -= checkJournal;
            inputActions["Rotate"].canceled -= OnRotateCanceled;
            inputActions["DeleteSave"].performed -= onClearSave;

            inputActions["ToggleInstructions"].performed -= ToggleMenu;

            inputActions["Restart"].performed -= RestartScene;
        }
        ToggleJournal.OnJournalOpened -=togglebool;
        ToggleJournal.OnJournalClosed -=togglebool;

        inputActions["Drop"].performed -= DropItem;
        inputActions["OpenInventory"].performed -= OpenInventory;   

        //for interactor
        inputActions["Interacted"].started -= ctx => OnInteractStarted?.Invoke();
        inputActions["Interacted"].canceled -= ctx => OnInteractCanceled?.Invoke();
    }

    private void DropItem(InputAction.CallbackContext context)
    {
       drop?.Invoke();
    }

    private void OpenInventory(InputAction.CallbackContext context)
    {
        openInventory?.Invoke();
    }

    private void checkJournal(InputAction.CallbackContext context)
    {
        if(SceneTrackerSingleton.CurrentSceneName == "Car")
        {
            
        }
        else
        {
            print("invoking journal");
            OpenJournal?.Invoke();

        }
       
    }

    //for tutorial!!
    private void ToggleMenu(InputAction.CallbackContext context)
    {
        if (!Oninstructions)
        {
            Oninstructions = true;
            instructions?.Invoke(Oninstructions);
        }
        else
        {
            Oninstructions = false;
            instructions?.Invoke(Oninstructions);
        }
    }

    private void RestartScene(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Restart?.Invoke();
            restartscene();
        }
    }

    private void restartscene()
    {
        print("restartingscene");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        
    }

    void Start()
    {
        // Already enabled in Awake, but keeping this as backup won't hurt
        inputActions.Enable();
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    // Called when Rotate button is pressed
    private void OnRotateStarted(InputAction.CallbackContext context)
    {
        if (JournalOpen)
        {
            canRotate = true;
            onRotateChanged?.Invoke(canRotate);
        }
    }

    // Called when Rotate button is released
    private void OnRotateCanceled(InputAction.CallbackContext context)
    {
        if (JournalOpen)
        {
        canRotate = false;
        print("Rotate canceled: " + canRotate);
        onRotateChanged?.Invoke(canRotate);
        }
    
    }


    public void onClearSave(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            JournalSave.clearSave();
        }
    }
    
    private void PauseGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Pause?.Invoke();
            //close journal if open
            if (ToggleJournal.journalopen)
            {
                 OpenJournal?.Invoke();
            }
        }
    }


}