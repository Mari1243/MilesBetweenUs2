namespace MaskTransitions
{
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class SceneSwitch : MonoBehaviour
    {
        public static SceneSwitch Instance;
        private string sceneToLoadName;
        public float totalTransitionTime;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            DialogueCommands.scenename += SwitchScene;
        }
        private void OnDisable()
        {
            DialogueCommands.scenename -= SwitchScene;
        }
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void SwitchScene(string scene)
        {
            if (scene == null)
            {
                print("scene was null");
                int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                int nextSceneIndex = currentSceneIndex + 1;
                string name = SceneManager.GetSceneByBuildIndex(nextSceneIndex).name;
                TransitionManager.Instance.LoadLevel(name, 2f);
            }
           
            else
            {
                sceneToLoadName = scene;
                print("switching scene to " + sceneToLoadName);
                TransitionManager.Instance.LoadLevel(sceneToLoadName, 2f);
            }
            
        }

        public void PlayTransition()
        {
            print("pleughh");
            TransitionManager.Instance.PlayTransition(totalTransitionTime);
        }

        void PlayStartOfTransition()
        {
            TransitionManager.Instance.PlayStartHalfTransition(totalTransitionTime / 2);
        }
        void PlayEndOfTransition()
        {
            TransitionManager.Instance.PlayEndHalfTransition(totalTransitionTime / 2);
        }
    }
}
