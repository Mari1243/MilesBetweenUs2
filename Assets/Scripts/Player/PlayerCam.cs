using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections; // Required for Action and delegate


/*
  
 This script controls the first person camera movement, input, and interaction/dialogue with your brother and objects solely within the car

*/

public class PlayerCam : MonoBehaviour
{
    public static event Action<bool> changeInteractPopup;

    [Header("Camera")]
    public float sensX;
    public float sensY;
    public Transform orientation;
    public float xRotation, yRotation;

    [Header("Interact")]
    public Transform InteractorSource;
    public float InteractRange;
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;


    [Header("Cross Hair IMGS")]
    public Image handIcon;
    public Image crossHair;


    [Header("Input")]
    private PlayerInput playerInput;
    private InputActionAsset inputActions;
    
    private bool isInDialogue = false; 

    private bool hasdialogueItem;

    private void OnEnable()
    {
        DialogueManager.DialogStart += TalkingCamera;
        DialogueManager.DialogOver += DefaultCamera;
    }

    private void OnDisable()
    {
        DialogueManager.DialogStart -= TalkingCamera;
        DialogueManager.DialogOver -= DefaultCamera;
    }

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        inputActions = playerInput.actions;

        inputActions["Interacted"].performed += FPInteract;
        handIcon.enabled = false;
        crossHair.enabled = true;
    }

    void Update()
    {
        if (!isInDialogue)
        {
            float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
            float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
            yRotation += mouseX;
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
            orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        }

        showOutline();
    }

    private void showOutline()
    {
        //Highlight
        if (highlight != null)
        {
            highlight.gameObject.GetComponent<Outline>().enabled = false;
            highlight = null;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;
            if (highlight.GetComponent<interactable>() != null && highlight != selection)
            {
                if(highlight.gameObject.tag == "canTalk" && hasdialogueItem)
                {
                    print("YOU CAN ASK ME ABOUT THINGSSSS TALK TO MEEEE");
                    //talk to UI manager
                    changeInteractPopup?.Invoke(true);
                }
              
                    crossHair.enabled= false;
                    handIcon.enabled = true;
                


                if (highlight.gameObject.GetComponent<Outline>() != null)
                {
                    highlight.gameObject.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    Outline outline = highlight.gameObject.AddComponent<Outline>();
                    outline.enabled = true;
                    highlight.gameObject.GetComponent<Outline>().OutlineColor = Color.white;
                    highlight.gameObject.GetComponent<Outline>().OutlineWidth = 7.0f;
                    changeInteractPopup?.Invoke(false);
                }
            }
            else
            {
                changeInteractPopup?.Invoke(false);
                crossHair.enabled = true;
                handIcon.enabled = false;
                highlight = null;
            }
        }
    }

    public void FPInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("INTERACT");
            Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
            if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange))
            {
                if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }

    public void TalkingCamera() 
    {
        isInDialogue = true; 
        gameObject.transform.DOLocalRotate(new Vector3(1.00179132f, 16.0000038f, 0), 2f);
        gameObject.transform.DOLocalMove(new Vector3(-0.104000002f, 1.24899995f, 0.669999957f), 2f);
 
    }
    
    public void DefaultCamera()
    {

        gameObject.transform.DOLocalRotate(new Vector3(359.278015f, 8.36011982f, 0.120773472f), 2f);
        gameObject.transform.DOLocalMove(new Vector3(-0.104000002f, 1.24899995f, -0.675999999f), 2f);
        isInDialogue = false;

    }


}