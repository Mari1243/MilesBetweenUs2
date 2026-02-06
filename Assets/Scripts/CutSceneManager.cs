using MaskTransitions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;   
    public string sceneToLoad;       

    void Start()
    {
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // When the video ends, load the scene
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void Update()
    {
        // Skip with keyboard
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadScene();
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        LoadScene();
    }

    
    public void SkipVideo()
    {
        LoadScene();
    }

    void LoadScene()
    {
        SceneSwitch.Instance.SwitchScene("IntroScene");
    }
}