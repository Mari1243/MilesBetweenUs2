using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseCanvasAnims : MonoBehaviour
{
    private Vector2 ActivePosition = Vector2.zero;
    private Vector2 InactivePosition = new Vector2(0, -2100);
    private RectTransform background;
    private RectTransform paper;
    private Canvas canvas;
    private Coroutine currentRoutine;
    private bool isPaused = false;
    private bool controlsOpen = false;

    public GameObject Controls;
    

    public static event System.Action<bool> onPause;
    private void Start()
    {
        canvas = GetComponent<Canvas>();
        background = transform.GetChild(0).GetComponent<RectTransform>();
        paper = transform.GetChild(1).GetComponent<RectTransform>();
        background.anchoredPosition = InactivePosition;
        paper.anchoredPosition = InactivePosition;
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        InputManager.Pause += PauseManage;
    }

    private void OnDisable()
    {
        InputManager.Pause -= PauseManage;
    }

    private void PauseManage()
    {
        if (!isPaused)
        {
            isPaused = true;
            PauseRoutine();
            currentRoutine = StartCoroutine(PauseGame());
            print("pawsing");
        }
        else
        {
            isPaused = false;
            PauseRoutine();
            currentRoutine = StartCoroutine(StartGame());
            print("playing");
        }
    }
    private void PauseRoutine()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        background.DOKill();
        paper.DOKill();  
    }

    private IEnumerator PauseGame()
    {
        canvas.enabled = true;
        //inputManager.inputActions["checkJournal"].Disable();
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        background.DOAnchorPos(ActivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.2f);
        paper.DOAnchorPos(ActivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
    
    }

    private IEnumerator StartGame()
    {
        //inputManager.inputActions["checkFJournal"].Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        background.DOAnchorPos(InactivePosition, .5f).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.2f);
        paper.DOAnchorPos(InactivePosition, .5f).SetEase(Ease.OutBack).SetUpdate(true);

        yield return new WaitForSecondsRealtime(1.2f); // Wait for animation to complete
        canvas.enabled = false;
    }

    public void exitGame()
    {
        Debug.Log("EXITING");

        Application.Quit();
    }

    public void backTitle()
    {
        Debug.Log("Loading Title Scene");

        Time.timeScale = 1f; // So new scene isn't frozen
        SceneManager.LoadScene("TitleScreen"); 
    }

    public void controls()
    {
        if (!controlsOpen)
        {
            Controls.SetActive(true);
            print("opening control panel");
            controlsOpen=true;
            DOTween.Restart("optionsIn"); 
            DOTween.Play("optionsIn");
        }
        else
        {
            Controls.SetActive(false);
            print("closing control panel");
            DOTween.Restart("optionsOut"); 
            DOTween.Play("optionsOut");
            controlsOpen = false;
        }
        
    }
}