using System.Collections;
using UnityEngine;

public class SchoolManager : MonoBehaviour
{
    private WaitForSeconds wait = new WaitForSeconds(1f);

    void OnEnable()
    {
        interactable.onEND += triggerEND;
    }
    void OnDisable()
    {
        interactable.onEND += triggerEND;
    }

    private void triggerEND()
    {
        print("game over yayyyy");
        StartCoroutine(endanimation());
    } 

    private IEnumerator endanimation()
    {
        yield return wait;
        //call a cutscene cam here
    }
}
