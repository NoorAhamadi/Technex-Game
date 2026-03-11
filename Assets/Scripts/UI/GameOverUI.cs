using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Chunk.UI
{
    /// <summary>
    /// Drives the Game Over overlay (retry / exit) that appears inside the game scene.
    /// The root panel starts hidden; GameManager.Die() calls Show().
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private Button retryButton;
        [SerializeField] private Button exitButton;

        private bool _initialized;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (_initialized)
                return;

            _initialized = true;
            retryButton?.onClick.AddListener(OnRetry);
            exitButton?.onClick.AddListener(OnExit);
        }

        /// <summary>Makes the panel visible and pauses physics.</summary>
        public void Show()
        {
            // Guard: if Awake never ran (component was inactive at startup), init now.
            Initialize();
            panel?.SetActive(true);
            Time.timeScale = 0f;
        }

        private void OnRetry()
        {
            Time.timeScale = 1f;
            panel?.SetActive(false);
            GameManager.Instance?.Respawn();
        }

        private void OnExit()
        {
            Time.timeScale = 1f;
            GameManager.Instance?.GoToMainMenu();
        }
    }
}
