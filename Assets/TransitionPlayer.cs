using UnityEngine;

public class TransitionPlayer : MonoBehaviour
{
    private bool transitiondownstairs;
    public GameObject Player;
    public GameObject DownstairsPos;
    public GameObject UpstairsPos;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        EmptyStealTrigger.EmptyStealBehavior += something;
    }
     private void OnDisable()
    {
        EmptyStealTrigger.EmptyStealBehavior -= something;
    }

    private void something()
    {
        if (!transitiondownstairs)
        {
            transitiondownstairs = true;
            Player.transform.position = DownstairsPos.transform.position;

        }
        else
        {
            transitiondownstairs = false;
            Player.transform.position = UpstairsPos.transform.position;
        }
    }
}
