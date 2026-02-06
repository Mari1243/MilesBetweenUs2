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
    private InputActionAsset inputActions;
    
    // Events
    public static event Action Pause;
    public static event System.Action<bool> onRotateChanged;
    public static event System.Action<bool> onScaleChanged;

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
            inputActions["Rotate"].started += OnRotateStarted;
            inputActions["Rotate"].canceled += OnRotateCanceled;
            inputActions["ToggleInstructions"].performed += ToggleMenu;
            inputActions["Restart"].performed += RestartScene;


            inputActions["Scale"].started += OnScaleStarted;
            inputActions["Scale"].canceled += OnScaleCanceled;

            interactable.showJournal += togglebool;
            ToggleJournal.hideJournal +=togglebool;

            inputActions["Drop"].performed += DropItem;
            inputActions["OpenInventory"].performed += OpenInventory;
        
        }
    }

    private void togglebool()
    {
        if (!JournalOpen)
        {
            JournalOpen=true;
        }
        else
        {
            JournalOpen = false;
        }
    }
    
    private void OnDisable()
    {
        // Unsubscribe to prevent memory leaks
        if (inputActions != null)
        {
            inputActions["Exit"].performed -= PauseGame;
            inputActions["Rotate"].started -= OnRotateStarted;
            inputActions["Rotate"].canceled -= OnRotateCanceled;
            inputActions["DeleteSave"].performed -= onClearSave;
            inputActions["Scale"].started -= OnScaleStarted;
            inputActions["Scale"].canceled -= OnScaleCanceled;
            inputActions["ToggleInstructions"].performed -= ToggleMenu;

            inputActions["Restart"].performed -= RestartScene;
        }
        interactable.showJournal -= togglebool;
        ToggleJournal.hideJournal -=togglebool;

        inputActions["Drop"].performed -= DropItem;
        inputActions["OpenInventory"].performed -= OpenInventory;   
    }

    private void DropItem(InputAction.CallbackContext context)
    {
       drop?.Invoke();
    }

    private void OpenInventory(InputAction.CallbackContext context)
    {
        openInventory?.Invoke();
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
            //Restart?.Invoke();
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
     private void OnScaleStarted(InputAction.CallbackContext context)
    {
        if (JournalOpen)
        {
        print("hey hi ummm scale started or smthn");
        canScale = true;
        onScaleChanged?.Invoke(canScale);
        }
    }

    private void OnScaleCanceled(InputAction.CallbackContext context)
    {
        if (JournalOpen)
        {
        canScale = false;
        onScaleChanged?.Invoke(canScale);
        print("scale ended");
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
        }
    }


}