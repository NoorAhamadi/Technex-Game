using UnityEngine;

namespace Chunk.Player
{
    /// <summary>
    /// Single source of truth for the player's horizontal facing direction.
    /// Flips all assigned SpriteRenderers when direction changes.
    /// </summary>
    public class FacingController : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        /// <summary>True when the player is facing left.</summary>
        public bool FacingLeft { get; private set; }

        /// <summary>
        /// Updates the facing direction and flips all sprite renderers accordingly.
        /// Only processes when direction actually changes.
        /// </summary>
        public void SetFacing(float horizontalDirection)
        {
            if (horizontalDirection == 0f)
                return;

            bool wantsLeft = horizontalDirection < 0f;

            if (wantsLeft == FacingLeft)
                return;

            FacingLeft = wantsLeft;

            foreach (SpriteRenderer sr in spriteRenderers)
            {
                if (sr != null)
                    sr.flipX = FacingLeft;
            }
        }
    }
}
