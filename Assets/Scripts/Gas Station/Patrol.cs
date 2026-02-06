using DG.Tweening;
using UnityEngine;
using System.Collections;

public class Patrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private Ease moveEase = Ease.Linear;

    [Header("Options")]
    [SerializeField] private bool loopPatrol = true;
    [SerializeField] private bool startOnAwake = true;
    [SerializeField] private bool IsOntoYou = false;

    [Header("Animator")]
    [SerializeField] private Animator dogAnimator;


    public GameObject player;


    private int currentPointIndex = 0;
    private Coroutine patrolCoroutine;

    private void Start()
    {
        if (startOnAwake && patrolPoints.Length > 0)
        {
            StartPatrol();
        }
    }

    public void StartPatrol()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
        }
        patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    public void StopPatrol()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            transform.DOKill();
        }
    }

    private IEnumerator PatrolRoutine()
    {
        while (true)
        {
            // Get the target position
            Vector3 targetPos = patrolPoints[currentPointIndex].position;

            // Rotate to face the target direction
            Vector3 direction = (targetPos - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.DORotateQuaternion(targetRotation, 1f / moveSpeed);
            }

            // Calculate duration based on distance and speed
            float distance = Vector3.Distance(transform.position, targetPos);
            float duration = distance / moveSpeed;

            // Move to the target point using DOTween
            dogAnimator.SetBool("isWalking", true);
            yield return transform.DOMove(targetPos, duration)
                .SetEase(moveEase)
                .WaitForCompletion();



            // Wait at the patrol point
            dogAnimator.SetBool("isWalking", false);
            yield return new WaitForSeconds(waitTime);

            //50% chance of him finding you sus
            IsOntoYou = Random.value > 0.5f;
            if (IsOntoYou)
            {
                sus();
            }

            // Move to next point
            currentPointIndex++;

            // Handle loop or ping-pong behavior
            if (currentPointIndex >= patrolPoints.Length)
            {
                
                if (loopPatrol)
                {
                    currentPointIndex = 0;
                }
                else
                {
                    sus();

                    // For ping-pong, reverse direction
                    currentPointIndex = patrolPoints.Length - 2;
                    System.Array.Reverse(patrolPoints);
                }
            }
        }
    }

    private IEnumerator sus()
    {
        // Store the current rotation
        Quaternion originalRotation = transform.rotation;

        // Look at the target object
        Transform targetToLookAt = player.transform; // assign this in inspector or find it
        Vector3 lookDirection = (targetToLookAt.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(lookDirection);

        // Rotate to look at it
        yield return transform.DORotateQuaternion(lookRotation, 0.5f).WaitForCompletion();

        // Stay looking at it for a while
        yield return new WaitForSeconds(2f);

        // Rotate back to original direction
        yield return transform.DORotateQuaternion(originalRotation, 0.5f).WaitForCompletion();
    }

    private void OnDestroy()
    {
        // Clean up DOTween on destroy
        transform.DOKill();
    }

    
}
