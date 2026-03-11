using UnityEngine;

namespace Chunk.World
{
    /// <summary>
    /// Place on the Chunk root GameObject.
    /// Registers the initial world position with GameManager so Respawn can
    /// teleport Chunk back without reloading the scene.
    /// </summary>
    public class SpawnPoint : MonoBehaviour
    {
        private void Start()
        {
            GameManager.Instance?.RegisterSpawn(transform.position, transform.rotation);
        }
    }
}
