using UnityEngine;
using Chunk.Player;

namespace Chunk.Player.Abilities
{
    /// <summary>
    /// Handles rolling movement for Stage 1 (Head only).
    /// Disables itself when Torso is unlocked and MovementAbility takes over.
    /// </summary>
    public class RollAbility : MonoBehaviour
    {
        private const float RollForce = 5f;
        private const float MaxRollSpeed = 8f;

        private Rigidbody2D _rb;
        private ChunkController _controller;
        private FacingController _facingController;

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody2D>();
            _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _facingController = GetComponentInParent<FacingController>();
        }

        private void OnEnable()
        {
            _controller = GetComponentInParent<ChunkController>();
            if (_controller != null)
                _controller.OnStageUnlocked.AddListener(OnStageUnlocked);
        }

        private void OnDisable()
        {
            if (_controller != null)
                _controller.OnStageUnlocked.RemoveListener(OnStageUnlocked);
        }

        /// <summary>
        /// Adds horizontal rolling force; speed is capped at MaxRollSpeed.
        /// Updates facing direction via FacingController.
        /// </summary>
        public void Roll(float direction)
        {
            _facingController?.SetFacing(direction);

            _rb.AddForce(Vector2.right * direction * RollForce);

            Vector2 velocity = _rb.linearVelocity;
            velocity.x = Mathf.Clamp(velocity.x, -MaxRollSpeed, MaxRollSpeed);
            _rb.linearVelocity = velocity;
        }

        private void OnStageUnlocked(StageType stage)
        {
            if (stage == StageType.Torso)
                gameObject.SetActive(false);
        }
    }
}
