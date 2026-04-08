using BookCurlPro;
using UnityEngine;

public class button : MonoBehaviour
{
    public int pageIndex;
    public BookPro book;

    public void OnTabButtonPressed()
    {
        print("triggered trying to turn to page " + pageIndex);
        book.TurnToPage(pageIndex);
    }
}
