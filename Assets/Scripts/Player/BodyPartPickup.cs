using UnityEngine;
using Chunk.Player;

namespace Chunk.Player
{
    /// <summary>
    /// Placed on each body part prefab in the scene.
    /// On player trigger enter, unlocks the matching stage and destroys itself.
    /// </summary>
    public class BodyPartPickup : MonoBehaviour
    {
        [SerializeField] private StageType stageToUnlock;
        [SerializeField] private ParticleSystem snapVFX;
        [SerializeField] private AudioClip snapSFX;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            ChunkController controller = other.GetComponentInParent<ChunkController>();
            if (controller == null)
            {
                Debug.LogWarning($"[BodyPartPickup] No ChunkController found on {other.name}.", this);
                return;
            }

            if (snapVFX != null)
            {
                snapVFX.transform.SetParent(null);
                snapVFX.Play();
            }

            if (snapSFX != null)
            {
                AudioSource.PlayClipAtPoint(snapSFX, transform.position);
            }

            controller.UnlockStage(stageToUnlock);
            Destroy(gameObject);
        }
    }
}
