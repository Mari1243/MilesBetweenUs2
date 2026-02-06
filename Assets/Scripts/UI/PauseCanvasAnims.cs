using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PauseCanvasAnims : MonoBehaviour
{
    private Vector2 ActivePosition = Vector2.zero;
    private Vector2 InactivePosition = new Vector2(0, -2100);
    private RectTransform background;
    private RectTransform paper;
    private RectTransform doodle;
    private Canvas canvas;
    private Coroutine currentRoutine; 

    private void Start()
    {
        canvas = GetComponent<Canvas>();
        background = transform.GetChild(0).GetComponent<RectTransform>();
        paper = transform.GetChild(1).GetComponent<RectTransform>();
        doodle = transform.GetChild(2).GetComponent<RectTransform>();
        background.anchoredPosition = InactivePosition;
        paper.anchoredPosition = InactivePosition;
        doodle.anchoredPosition = InactivePosition;
        canvas.enabled = false;
    }

    private void OnEnable()
    {
        UIManager.onPause += PauseRoutine;
        IntroUIManager.onPause +=PauseRoutine;
    }

    private void OnDisable()
    {
        UIManager.onPause -= PauseRoutine;
         IntroUIManager.onPause -=PauseRoutine;
    }

    private void PauseRoutine(bool paused)
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        background.DOKill();
        paper.DOKill();
        doodle.DOKill();

        if (paused)
            currentRoutine = StartCoroutine(PauseGame());
        else
            currentRoutine = StartCoroutine(StartGame());
    }

    private IEnumerator PauseGame()
    {
        canvas.enabled = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        background.DOAnchorPos(ActivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.2f);
        paper.DOAnchorPos(ActivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.2f);
        doodle.DOAnchorPos(ActivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
    
    }

    private IEnumerator StartGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        background.DOAnchorPos(InactivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.2f);
        paper.DOAnchorPos(InactivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(.2f);
        doodle.DOAnchorPos(InactivePosition, 1).SetEase(Ease.OutBack).SetUpdate(true);
        yield return new WaitForSecondsRealtime(1.2f); // Wait for animation to complete
        canvas.enabled = false;
    }

    public void exitGame()
    {
        Debug.Log("EXITING");

        Application.Quit();
    }
}