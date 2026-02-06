using UnityEngine;

public class BrotherGasStationMovement: MonoBehaviour
{
    [Header("Animator & Movement")]
    public Animator anim;
    public Transform[] walkPoints; // multiple points
    public float speed = 2f;
    public float idleDuration = 3f;

    [Header("Rotation Settings")]
    public Vector3 walkRotation;
    public Vector3 idleBRotation;
    public float rotationSpeed = 5f;

    private bool walking = false;
    private bool finished = false; // NEW: has NPC completed walking
    private float timer = 0f;

    // Rotation
    private Quaternion targetRotation;
    private bool rotating = false;

    private int currentPointIndex = 0; // current point index

    void Update()
    {
        // ROTATION
        if (rotating)
        {
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
                rotating = false;
        }

        // DO NOTHING IF FINISHED
        if (finished) return;

        // IDLE → START WALKING
        if (!walking)
        {
            timer += Time.deltaTime;
            if (timer >= idleDuration)
            {
                StartWalking();
            }
            return; // don't run movement if not walking
        }

        // SAFETY
        if (walkPoints.Length == 0 || currentPointIndex >= walkPoints.Length) return;

        // MOVE TOWARD CURRENT POINT
        Transform targetPoint = walkPoints[currentPointIndex];
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            speed * Time.deltaTime
        );

        // CHECK ARRIVAL
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            currentPointIndex++;

            if (currentPointIndex >= walkPoints.Length)
            {
                // STOP MOVEMENT at last point
                walking = false;
                finished = true; // NEW: lock walking so it won't restart

                // Trigger final idle animation only at last point
                anim.SetTrigger("ReachedDestination");

                // Rotate toward final idle rotation
                targetRotation = Quaternion.Euler(idleBRotation);
                rotating = true;
            }
        }
    }

    void StartWalking()
    {
        if (walkPoints.Length == 0) return;

        walking = true;
        timer = 0f; // reset timer
        currentPointIndex = 0; // start at first point
        anim.SetTrigger("StartWalking");

        // rotate toward walk rotation
        targetRotation = Quaternion.Euler(walkRotation);
        rotating = true;
    }
}