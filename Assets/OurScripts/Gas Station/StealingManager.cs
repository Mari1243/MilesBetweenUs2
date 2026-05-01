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
    public AudioClip endStealNoise;
    public AudioClip shortSteal;

    public static event Action<DangerState> OnStateChanged;
    public static event Action<bool> OnStealingActionChanged;

    public DangerState CurrentState { get; private set; }

    public Vector2 stateDurationRange = new Vector2(1f, 4f);


    private bool stealingActive = false;
    private int activeCamIndex = 0;
    private int activeDefaultCamIndex = 0;

    public GameObject player;

    private DangerState[] stateCycle = new DangerState[]
    {
        DangerState.Safe,
        DangerState.Suspicious,
        DangerState.Caught,
        DangerState.Suspicious
    };

    private void OnEnable()
    {
        Interactor.OnStopStealing += StopStealin;
    }

    private void OnDisable()
    {
        Interactor.OnStopStealing -= StopStealin;
    }



    private int cycleIndex;
    private Coroutine cycleRoutine;

   private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    
    public void StartStealin(int camIndex, int defaultCamIndex = 0)
    {
        //sound
        SoundManager.Instance.musicAudioSource.Stop();
        SoundManager.Instance.playLoopingAudio(shortSteal);


        if (stealingActive) return;
        stealingActive = true;
        activeCamIndex = camIndex;
        activeDefaultCamIndex = defaultCamIndex;
    
        cycleIndex = 0;
        SetState(stateCycle[cycleIndex]);
        OnStealingActionChanged?.Invoke(true);
        ChangeCamera.instance.changeCamera(activeCamIndex);
        cycleRoutine = StartCoroutine(StateCycleRoutine());
    }

    public void StopStealin() //THIS IS WHAT HAPPENS WHEN STEALING IS OVER
    {
        print("stop stealin");
        stealingActive = false;
        if (cycleRoutine != null)
        StopCoroutine(cycleRoutine);
        ChangeCamera.instance.changeCamera(activeDefaultCamIndex); // uses whatever was passed in
        OnStealingActionChanged?.Invoke(false);

        //sound
        //sound
        SoundManager.Instance.musicAudioSource.Stop();
        SoundManager.Instance.effectAudioSource.Stop();
        SoundManager.Instance.PlayAudio(endStealNoise);
        SoundManager.Instance.playmusic();
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