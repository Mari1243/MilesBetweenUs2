using UnityEngine;

public class ScreenShotScript : MonoBehaviour
{
    public void screenshot()
    {
        //play a flash
        ScreenCapture.CaptureScreenshot("JournalPic.png");
    }
}
