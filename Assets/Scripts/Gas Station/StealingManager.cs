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
    public static event Action OnStartedStealing;

    public DangerState CurrentState { get; private set; }

    public Vector2 stateDurationRange = new Vector2(1f, 4f);


    private bool stealingActive = false;

    public GameObject player;

    private DangerState[] stateCycle = new DangerState[]
    {
        DangerState.Safe,
        DangerState.Suspicious,
        DangerState.Caught,
        DangerState.Suspicious
    };

  
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
        // Reset and begin the cycle when enabled.
        cycleIndex = 0;
        SetState(stateCycle[cycleIndex]);
        //this will enable the UI
        OnStealingActionChanged?.Invoke(true);
        cycleRoutine = StartCoroutine(StateCycleRoutine());
    }

    public void StopStealin() //THIS IS WHAT HAPPENS WHEN STEALING IS OVER
    {
        stealingActive = false;
        // Stop immediately when disabled.
        if (cycleRoutine != null)
            StopCoroutine(cycleRoutine);
        OnStealingActionChanged?.Invoke(false);
        //StartCoroutine(CamChange());
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

}