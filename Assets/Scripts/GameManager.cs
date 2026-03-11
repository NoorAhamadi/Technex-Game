using Chunk.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Chunk
{
    /// <summary>
    /// Singleton that persists across scenes. Owns death/respawn flow and scene loading.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        private const string GameSceneName = "Test";
        private const string MainMenuSceneName = "MainMenu";

        public static GameManager Instance { get; private set; }

        /// <summary>World-space spawn position set when the game scene loads.</summary>
        private Vector3 _spawnPosition;
        private Quaternion _spawnRotation;
        private bool _spawnCaptured;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        // ── Scene loading ────────────────────────────────────────────────────────

        /// <summary>Loads the main game scene from the start menu or anywhere else.</summary>
        public void StartGame()
        {
            SceneManager.LoadScene(GameSceneName);
        }

        /// <summary>Loads the main menu scene.</summary>
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(MainMenuSceneName);
        }

        /// <summary>Quits the application (works in builds; stops play mode in Editor).</summary>
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // ── Spawn tracking ───────────────────────────────────────────────────────

        /// <summary>
        /// Called by the player controller (or any boot script) once the game scene
        /// is ready to record where Chunk should respawn.
        /// </summary>
        public void RegisterSpawn(Vector3 position, Quaternion rotation)
        {
            _spawnPosition = position;
            _spawnRotation = rotation;
            _spawnCaptured = true;
        }

        // ── Death / Respawn ──────────────────────────────────────────────────────

        /// <summary>Triggered by DeathZone when the player touches a death tile.</summary>
        public void Die()
        {
            Debug.Log("[GameManager] Die() called.");
            GameOverUI ui = FindFirstObjectByType<GameOverUI>(FindObjectsInactive.Include);
            Debug.Log($"[GameManager] GameOverUI found: {ui != null}");
            ui?.Show();
        }

        /// <summary>
        /// Moves Chunk back to the spawn position and re-enables normal play.
        /// Called by the GameOverUI Retry button.
        /// </summary>
        public void Respawn()
        {
            if (!_spawnCaptured)
            {
                // Fallback: reload the full scene.
                SceneManager.LoadScene(GameSceneName);
                return;
            }

            // Find Chunk by tag — the GameOverUI has already hidden itself.
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.linearVelocity = Vector2.zero;

                player.transform.SetPositionAndRotation(_spawnPosition, _spawnRotation);
            }
        }
    }
}
