using UnityEngine;

public class PageTurnTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnMouseDown()
    {
        JournalManager_Script.TurnPage(gameObject);
    }
}
