using UnityEngine;
using Chunk.Player;

namespace Chunk.Player.Abilities
{
    /// <summary>
    /// Handles walk and jump for Stage 2 (Torso) and beyond.
    /// </summary>
    public class MovementAbility : MonoBehaviour
    {
        private const float MoveSpeed = 5f;
        private const float JumpForce = 10f;
        private const float GroundCheckRadius = 0.1f;

        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody2D _rb;
        private FacingController _facingController;
        private float _moveDirection;
        private bool _isGrounded;
        private bool _canJump;
        private ContactFilter2D _groundFilter;

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody2D>();
            _facingController = GetComponentInParent<FacingController>();

            _groundFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask    = groundLayer,
                useTriggers  = false
            };
        }

        /// <summary>
        /// Applies horizontal movement in the given direction and updates facing.
        /// </summary>
        public void Move(float direction)
        {
            _moveDirection = direction;
            _facingController?.SetFacing(direction);
        }

        /// <summary>
        /// Applies a single vertical impulse when grounded. Prevents jumping again until landing.
        /// </summary>
        public void Jump()
        {
            if (!_canJump)
                return;

            _rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
            _canJump = false;
        }

        private void FixedUpdate()
        {
            // Rebuild filter each FixedUpdate in case groundLayer changes in Inspector.
            _groundFilter.layerMask = groundLayer;

            _isGrounded = groundCheck != null &&
                          Physics2D.OverlapCircle(groundCheck.position, GroundCheckRadius, _groundFilter.layerMask);

            if (_isGrounded && _rb.linearVelocity.y <= 0.01f)
                _canJump = true;

            _rb.linearVelocity = new Vector2(_moveDirection * MoveSpeed, _rb.linearVelocity.y);
        }
    }
}
