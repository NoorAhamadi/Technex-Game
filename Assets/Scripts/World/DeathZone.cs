using UnityEngine;

namespace Chunk.World
{
    /// <summary>
    /// Attach to the DeathTile Tilemap GameObject.
    /// Requires a TilemapCollider2D set to Is Trigger so the physics system fires
    /// OnTriggerEnter2D when the player overlaps a death tile.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Tilemaps.TilemapCollider2D))]
    public class DeathZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            Debug.Log("[DeathZone] Player entered death tile.");
            GameManager.Instance?.Die();
        }
    }
}
