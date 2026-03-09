using UnityEngine;
using Chunk.Player.Abilities;

namespace Chunk.World
{
    /// <summary>
    /// Placed on scrap collectible GameObjects.
    /// On player trigger enter, adds scrap to ChunkPlacementAbility and destroys itself.
    /// </summary>
    public class ScrapPickup : MonoBehaviour
    {
        [SerializeField] private int scrapValue = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            // ChunkPlacementAbility lives on a child of the root — search downward.
            ChunkPlacementAbility placement = other.GetComponentInChildren<ChunkPlacementAbility>(includeInactive: true);
            if (placement == null)
                placement = other.GetComponentInParent<ChunkPlacementAbility>();

            if (placement == null)
            {
                Debug.LogWarning("[ScrapPickup] No ChunkPlacementAbility found on the player.", this);
                return;
            }

            placement.CollectScrap(scrapValue);
            Destroy(gameObject);
        }
    }
}
