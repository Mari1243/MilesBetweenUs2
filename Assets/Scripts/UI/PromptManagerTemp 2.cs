using TMPro;
using UnityEngine;
using System.Collections;

public class PromptManagerTemp : MonoBehaviour
{
    public TextMeshProUGUI steal;
    //public TextMeshProUGUI canI;
    //private float wait = 4f;

    //private bool hasEntered;


    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
    //    steal.enabled = false;
    //    canI.enabled = false;
    //}

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.name == "Player")
    //    {
    //        if (!hasEntered)
    //        {
    //            StartCoroutine(LearntoSteal());
    //            hasEntered = true;
    //        }
    //        else
    //        {
    //             StartCoroutine(stealing());
    //        }

    //    }
    //}

    //private IEnumerator LearntoSteal()
    //{
    //    canI.enabled = true;
    //    yield return new WaitForSeconds(4f); // Pause for 2 seconds
    //    canI.enabled = false;
    //    yield return new WaitForSeconds(1f);
    //    steal.enabled = true;
    //    yield return new WaitForSeconds(4f);
    //    steal.enabled = false;
    //    yield break; // Terminate the coroutine
    //}

    // private IEnumerator stealing()
    //{
    //    steal.enabled = true;
    //    yield return new WaitForSeconds(2f);
    //    steal.enabled = false;
    //    yield break; // Terminate the coroutine
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            steal.text = "'E' to steal :3";
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            steal.text = "";
        }

    }
}
