using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TempInstructionsScript : MonoBehaviour
{
    private Vector3 activepos;
    private Vector3 inactivepos;
    private void OnEnable()
    {
        InputManager.instructions += uhh;
        InputManager.Restart += restartgame;
    }
    private void OnDisable()
    {
        InputManager.instructions -= uhh;
        InputManager.Restart -= restartgame;
    }

    void Start()
    {
        inactivepos = this.transform.position;
        activepos = new Vector3(inactivepos.x, inactivepos.y+500,0);
    }

    private void uhh (bool bol)
    {
        if (bol)
        {
            print("enabled");
            this.transform.DOMove(activepos, .5f);
        }
        else
        {
            print("disabled");
            this.transform.DOMove(inactivepos, .5f);
        }
    }

    private void restartgame()
    {
        print("restarting");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
