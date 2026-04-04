using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneScript : MonoBehaviour
{
   public void GotoNextScene()
    {
        // Get the index of the current active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the index of the next scene
        int nextSceneIndex = currentSceneIndex + 1;

        // Optional: Check if the next scene index is valid (prevents errors if it's the last scene)
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene by its index
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("This is the last scene in the build settings!");
            // Optionally, load the first scene (index 0) to loop the game
            // SceneManager.LoadScene(0);
        }
    }
}
