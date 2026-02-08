using UnityEngine;
using System;
using Unity.Cinemachine;
using System.Collections;
using DG.Tweening;
using System.Runtime.InteropServices;


public class StealingManager : MonoBehaviour
{
    public enum DangerState
    {
        Safe,
        Suspicious,
        Caught
    }
    public static StealingManager Instance;

    public static event Action<DangerState> OnStateChanged;
    public static event Action<bool> OnStealingActionChanged;

    public DangerState CurrentState { get; private set; }

    public Vector2 stateDurationRange = new Vector2(1f, 4f);


    private CinemachineBrain brain;
    private ICinemachineCamera activeVCam;
    private float regFOV;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private bool stealingActive = false;

    public GameObject player;

    private DangerState[] stateCycle = new DangerState[]
    {
        DangerState.Safe,
        DangerState.Suspicious,
        DangerState.Caught,
        DangerState.Suspicious
    };

    private void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    private int cycleIndex;
    private Coroutine cycleRoutine;

    private void Awake()
    {
        Instance = this;
    }

    
    public void StartStealin()
    {
        if (stealingActive) return;
        //if (!stealingActive) { stealingCamera(); }
        stealingActive = true;
        print("enabling stealing manager");
        // Reset and begin the cycle when enabled.
        cycleIndex = 0;
        SetState(stateCycle[cycleIndex]);
        //this will enable the UI
        OnStealingActionChanged?.Invoke(true);
        cycleRoutine = StartCoroutine(StateCycleRoutine());
    }

    public void StopStealin() //THIS IS WHAT HAPPENS WHEN STEALING IS OVER
    {
        print("calling stop stealing void");
        stealingActive = false;
        // Stop immediately when disabled.
        if (cycleRoutine != null)
            StopCoroutine(cycleRoutine);
        print("disabling stealing manager");
        OnStealingActionChanged?.Invoke(false);
        //StartCoroutine(CamChange());
    }

    public IEnumerator CamChange()
    {
        //not called anymore
        yield return new WaitForSeconds(.5f);
        regularCamera();
    }

    private System.Collections.IEnumerator StateCycleRoutine()
    {
        while (true)
        {
            float wait = UnityEngine.Random.Range(stateDurationRange.x, stateDurationRange.y);
            yield return new WaitForSeconds(wait);
            CycleToNextState();
        }
    }

    void CycleToNextState()
    {
        cycleIndex = (cycleIndex + 1) % stateCycle.Length;
        SetState(stateCycle[cycleIndex]);
    }

    public void SetState(DangerState newState)
    {
        if (newState == CurrentState) return;
        CurrentState = newState;
        //Debug.Log("State → " + CurrentState);
        OnStateChanged?.Invoke(CurrentState);
    }

    public void stealingCamera()
    {
        //never called
        activeVCam = brain.ActiveVirtualCamera;
        CinemachineCamera cam = activeVCam as CinemachineCamera;
        
        if (cam != null)
        {
            // Store original transform
            originalPosition = cam.transform.position;
            originalRotation = cam.transform.rotation;
            regFOV = cam.Lens.FieldOfView;
            
            // Set LookAt to aim camera AT the player (centered)
            cam.LookAt = player.transform;

            // Optionally also set Follow to track player position
            // cam.Follow = player.transform;

            // Wait for Cinemachine to reposition, then zoom
            StartCoroutine(ChangeFOV(cam, regFOV-20, .5f));
        }
    }

    private void regularCamera()
    {
        //never called
        CinemachineCamera cam = activeVCam as CinemachineCamera;

        if (cam != null)
        {
            // Set LookAt back to null
            Transform playerPos = player.transform;
            

            cam.LookAt = playerPos;
            cam.Follow = playerPos; // if you're using Follow too

            // Reset transform and FOV
            StartCoroutine(ChangeFOV(cam, regFOV, .5f));
            //cam.Lens.FieldOfView = regFOV;
            cam.transform.position = originalPosition;
            cam.transform.rotation = originalRotation;
        }
    }
    IEnumerator ChangeFOV(CinemachineCamera cam, float endFOV, float duration)
    {
        //never called
        print("changing FOV");
        float startFOV = cam.Lens.FieldOfView;
        float time = 0;
        while(time < duration)
        {
            cam.Lens.FieldOfView = Mathf.Lerp(startFOV, endFOV, time / duration);
            yield return null;
            time += Time.deltaTime;
        }
    }
}