using System.Data;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Collections;
using NUnit.Framework.Internal;

public class mapsnapscript : MonoBehaviour
{
    public Image OurAdventure;
    public GameObject map;
    public GameObject exitButton;
    public GameObject customCursor;
    private void Start()
    {
        exitButton.SetActive(false);
    }
    public void OnTriggerEnter2D(Collider2D map)
    {
        if(map.name == "Map")
        {
            print("map has snapped");
            map.gameObject.GetComponent<RectTransform>().anchoredPosition = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
            DOTweenAnimation test = map.GetComponent<DOTweenAnimation>();

            if(test != null)
            {
                print("Found DOTweenAnimation, playing...");
                test.DORestart();
                test.DOPlay();

                // test.DOPlayForward();
                //map.transform.SetParent(this.transform,false); doesnnt work well
                StartCoroutine(cute());
            }
            else
            {
                print("No DOTweenAnimation component found");
            }
        }
        
    }

    private IEnumerator cute()
    {
        OurAdventure.DOFillAmount(1f, 2f).SetEase(Ease.InOutQuad).OnComplete(() => Debug.Log("Fill complete!"));
        yield return new WaitForSeconds(4f);

        exitButton.SetActive(true);
        exitButton.transform.DOScale(new Vector3(1, 1, 1), .5f).SetEase(Ease.OutBounce);

    }


    public void closeJournal() //call button
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        customCursor.SetActive(false);
        JournalTutorial.closeJournal(map);
        JournalTutorial.closeJournal(exitButton);
    }
}
