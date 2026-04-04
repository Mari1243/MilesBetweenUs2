using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DG.Tweening;

public class JournalManager_Script : MonoBehaviour
{
    private static bool isFlipping = false;

    //temp
    private static Material temprightpagemat;
    private static Material templeftpagemat;
    private static Material deselectedmat;
    //end temp

    public List<GameObject> pages = new List<GameObject>();
    private static List<GameObject> statpages = new List<GameObject>();

    [SerializeField] private static GameObject leftpage;
    [SerializeField] private static GameObject rightpage;
    private static int lastpage = 0;
    private static int maxpages;

    public static JournalManager_Script JournalManager;
    private void Awake()
    {
        if (JournalManager != null && JournalManager != this)
        {
            Destroy(gameObject);
        }
        else
        {
            JournalManager = this;
        }
    }

    private void Start()
    {
        statpages = pages;
        maxpages = statpages.Count;

        leftpage = statpages[0];
        rightpage = statpages[1];

        deselectedmat = gameObject.transform.parent.GetComponent<Renderer>().materials[0];
        templeftpagemat = gameObject.transform.parent.GetComponent<Renderer>().materials[1];
        temprightpagemat = gameObject.transform.parent.GetComponent<Renderer>().materials[2];

        debugging();
    }

    private Dictionary<GameObject, int> PageList;
    public GameObject journal;
    [SerializeField] private GameObject SelectedPage;
    private Vector3 inactivepos;

    public static void TurnPage(GameObject obj)
    {
        if (isFlipping) return; //  Don�t allow flipping during animation

        if (obj.name == "NextPageCol")
        {
            if (lastpage < maxpages - 2)
            {
                isFlipping = true; //  Lock flipping

                FlipPagePrep();

                var placeholder = rightpage.transform.parent;
                var rotatedpos = new Vector3(0, 180, 0);

                placeholder.DORotate(rotatedpos, 1.2f, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        DoneFlippingPage();
                        isFlipping = false; //  Unlock flipping
                    });

                lastpage++;
                rightpage = statpages[lastpage + 1];
                leftpage = statpages[lastpage];
                debugging();
            }
        }
        else if (obj.name == "PreviousPageCol")
        {
            if (lastpage > 0)
            {
                isFlipping = true;

                FlipPagePrep();

                var placeholder = leftpage.transform.parent;
                var rotatedpos = new Vector3(0, -180, 0);

                placeholder.DORotate(rotatedpos, 1.2f, RotateMode.LocalAxisAdd)
                    .OnComplete(() =>
                    {
                        DoneFlippingPage();
                        isFlipping = false;
                    });

                lastpage--;
                rightpage = statpages[lastpage + 1];
                leftpage = statpages[lastpage];
                debugging();
            }
        }
    }

    private static void debugging()
    {
        foreach (GameObject page in statpages)
        {
            page.GetComponent<MeshRenderer>().sharedMaterial = deselectedmat;
        }
        //rightpage.GetComponent<MeshRenderer>().sharedMaterial = temprightpagemat;
       // leftpage.GetComponent<MeshRenderer>().sharedMaterial = templeftpagemat;

        hidePages();
    }

    private static void hidePages()
    {
        //deactivating deselected pages
        foreach (GameObject page in statpages)
        {
            //moveem backward
            page.SetActive(false);
        }
        //moveem forward
        leftpage.SetActive(true);
        rightpage.SetActive(true);
    }

    //the next thing i need to do is take the selected page that will be turning and parent its children

    public static void FlipPagePrep()
    {
        print("am flipping, parenting now");
        rightpage.GetComponent<PageManager>().ParentItems();
        leftpage.GetComponent<PageManager>().ParentItems();
    }
    public static void DoneFlippingPage()
    {
        print("done flipping, unparent");
        rightpage.GetComponent<PageManager>().UnParentItems();
        leftpage.GetComponent<PageManager>().UnParentItems();
    }
}
