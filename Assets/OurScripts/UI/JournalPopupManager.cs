using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JournalPopupManager : MonoBehaviour
{
    public string defaultMessage = "Press space to open journal";
    public string defaultDescription = " has been added to Tasks";
    [Header("UI References")]
    public TextMeshProUGUI itemtext;
    public TextMeshProUGUI descriptortext;
    public float waittime = 6;
    public float openDuration = 1;
    private Sequence _hintSequence;

    private IEnumerator _activeCoroutine;

    private void Start()
    {
        //disableThis();
    } 

    private void OnEnable()
    {
        //listen to ui manager
        GasStationManager.journalNotif += Notify;
        DragonLandManager.journalNotif += Notify;
        SchoolManager.journalNotif += Notify;
    }
    private void OnDisable()
    {
        //listen to ui manager
        GasStationManager.journalNotif -= Notify;
        DragonLandManager.journalNotif -= Notify;
        SchoolManager.journalNotif -= Notify;
    }

   
    public void Notify(string taskName)
    {
        print("calling notify");
        if(taskName != null)
        {
            itemtext.text = taskName.ToString();
            descriptortext.text = defaultDescription;
        }
        else
        {
            print("task name is null, switching to default message");
            itemtext.text = "";
            descriptortext.text = defaultMessage;
        }

        ShowAndHide();
    }

private void ShowAndHide()
{
    DOTween.Pause("OpenHint");
    DOTween.Pause("CloseHint");
    DOTween.Rewind("OpenHint");
    
    _hintSequence = DOTween.Sequence();
    _hintSequence.AppendCallback(() => { DOTween.Rewind("OpenHint"); DOTween.Play("OpenHint"); });
    _hintSequence.AppendInterval(openDuration);
    _hintSequence.AppendInterval(waittime);
    _hintSequence.AppendCallback(() => { DOTween.Rewind("CloseHint"); DOTween.Play("CloseHint"); });
}

}
