using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrackerSingleton : MonoBehaviour
{
    public static SceneTrackerSingleton Instance { get; private set; }

    public string CurrentSceneName { get; private set; }
    public string PreviousSceneName { get; private set; }

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize with current scene
            CurrentSceneName = SceneManager.GetActiveScene().name;
            PreviousSceneName = "None";

            // Subscribe to scene loaded event
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Update previous scene before changing current
        PreviousSceneName = CurrentSceneName;
        CurrentSceneName = scene.name;

        Debug.Log($"Scene changed: {PreviousSceneName} -> {CurrentSceneName}");
    }

    void OnDestroy()
    {
        // Unsubscribe when destroyed
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}