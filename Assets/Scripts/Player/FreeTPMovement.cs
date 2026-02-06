using UnityEngine;
using System.Collections;

public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask isGround;
    bool grounded;


    public Transform orientation;

    float horizontaInput;
    float verticalInput;

    Vector3 moveDirection;
    public Animator animator;

    Rigidbody rb;
    private bool canMove = true;


    private void OnEnable()
    {

        DialogueManager.DialogStart += OnDialogStart;
        DialogueManager.DialogOver += OnDialogOver;
    }

    private void OnDisable()
    {

        DialogueManager.DialogStart -= OnDialogStart;
        DialogueManager.DialogOver -= OnDialogOver;
    }



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * .5f + 2f, isGround);
        PlayerInput();

        SpeedControl();

        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = 0;

    }

    private void FixedUpdate()
    {

        if (canMove)
        MovePlayer();
    }


    private void PlayerInput()
    {
        horizontaInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontaInput;

        bool isMoving = horizontaInput != 0 || verticalInput != 0;
        animator.SetBool("isWalking", isMoving);

        rb.AddForce(moveDirection * moveSpeed * 10f, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
  
    private void OnDialogStart()
    {
        animator.SetBool("isWalking", false);

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