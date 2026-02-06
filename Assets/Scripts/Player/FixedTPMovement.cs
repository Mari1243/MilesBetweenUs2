using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class FixedTPMovement : MonoBehaviour
{
    [SerializeField] public float playerSpeed = 5f;
    [SerializeField] private float turnSpeed = 30f;
    [SerializeField] private float gravity = -9.81f;

    private float currentYRotation = 0f;
    private float verticalVelocity = 0f;
    private bool canMove=true;
    
    public InputSystem_Actions input;
    public CharacterController controller;
    public Animator animator;

    private void Awake()
    {
        input = new InputSystem_Actions();
        currentYRotation = transform.localEulerAngles.y;



    }

    private void OnEnable()
    {
        input.Enable();

        DialogueManager.DialogStart += OnDialogStart;
        DialogueManager.DialogOver += OnDialogOver;
    }

    private void OnDisable()
    {
        input.Disable();

        DialogueManager.DialogStart -= OnDialogStart;
        DialogueManager.DialogOver -= OnDialogOver;
    }

    private void Update()
    {
        
        playerMovement();
        UpdateAnimator();
    }

    public void Turn(float turnDirection)
    {
        float scaleTurnSpeed = turnSpeed * Time.deltaTime;
        float turnAmount = turnDirection * scaleTurnSpeed;

        currentYRotation += turnAmount;

        transform.localEulerAngles = new Vector3(0, currentYRotation, 0);
    }

    private void playerMovement()
    {
        if (canMove)
        {
            Vector2 move = input.Player.Move.ReadValue<Vector2>();



            Turn(move.x);

            Vector3 moveDirection = transform.forward * move.y;
            Vector3 movement = moveDirection * playerSpeed;

            if (controller.isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = -2f;

            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;

            }

            movement.y = verticalVelocity;
            controller.Move(movement * Time.deltaTime);
        }
        

    }


    private void UpdateAnimator()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();

        bool isMoving = Mathf.Abs(move.y) > 0.1f;
        
        animator.SetBool("isWalking", isMoving);

    }

    private void OnDialogStart()
    {
        canMove = false;
    }
    private void OnDialogOver()
    {
        StartCoroutine(delayMove());
    }
    IEnumerator delayMove()
    {
        yield return new WaitForSeconds(2f);
        canMove = true;
    }
}