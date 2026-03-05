using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public enum ToDoItemState { Incomplete, Completed}

public class ToDoItemBehavior : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private Image strikethrough;
    [SerializeField] private TextMeshProUGUI objectiveText;

    private ToDoItemState _currentState = ToDoItemState.Incomplete;

    private void Awake()
    {
        strikethrough = gameObject.transform.GetChild(0).GetComponent<Image>();
        animator = strikethrough.gameObject.GetComponent<Animator>();

        objectiveText = gameObject.transform.GetChild(2).GetComponent<TextMeshProUGUI>(); 

        SetState(ToDoItemState.Incomplete);
    }

    private void OnEnable()
    {
        // Trigger animation if this item is opened while already completed
        if (_currentState == ToDoItemState.Completed)
        {
                animator.SetTrigger("CompletedTask");
        }
    }

    //this is what youll ref from another script to check off the item
    public void SetState(ToDoItemState newState)
    {
        _currentState = newState;
        HandleStateChange();
    }

    public ToDoItemState GetState() => _currentState;

    private void HandleStateChange()
    {
        switch (_currentState)
        {
            case ToDoItemState.Incomplete:
            animator.enabled = false;
            strikethrough.fillAmount = 0;
            //print("enabling to do item " + gameObject.name + " is in state " + _currentState);
            break;
            case ToDoItemState.Completed:
            animator.enabled = true;
            //print("enabling to do item " + gameObject.name + " is in state " + _currentState);
            break;
        }
    }

}
