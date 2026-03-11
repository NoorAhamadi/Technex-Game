using UnityEngine;
using UnityEngine.UI;

namespace Chunk.UI
{
    /// <summary>
    /// Drives the Main Menu scene. Requires a GameManager in the scene or already
    /// alive from DontDestroyOnLoad.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button quitButton;

        private void Awake()
        {
            // Ensure there is a GameManager in the scene if the game was launched
            // directly from the MainMenu scene (Editor flow).
            if (GameManager.Instance == null)
            {
                new GameObject("GameManager").AddComponent<GameManager>();
            }

            playButton?.onClick.AddListener(OnPlay);
            quitButton?.onClick.AddListener(OnQuit);
        }

        private void OnPlay()
        {
            GameManager.Instance.StartGame();
        }

        private void OnQuit()
        {
            GameManager.Instance.QuitGame();
        }
    }
}
