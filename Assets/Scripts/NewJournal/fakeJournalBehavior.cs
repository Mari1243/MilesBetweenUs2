using System.Collections;
using UnityEngine;

public class fakeJournalBehavior : MonoBehaviour
{
    public static fakeJournalBehavior instance;

    void Awake()
    {
        instance = this;
    }

    public void mapsnapped()
    {
        StartCoroutine(waittt());
    }

    private IEnumerator waittt()
    {
        print("ok doing journal stuff now");
        DialogueManager.instance.LoadDialog("WhatisJournal");
        yield return new WaitForSeconds (.5f);
        DialogueManager.instance.StartDialog();
        DialogueManager.instance.OnDialogOver();
    }
}
