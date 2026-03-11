using Chunk.World;
using UnityEngine;

namespace Chunk.Player.Abilities
{
    /// <summary>
    /// Manages the full grapple lifecycle: finding the nearest GrapplePoint,
    /// attaching, pulling, releasing, and rendering the rope via a LineRenderer.
    /// Starts inactive; activated by ChunkController when ArmGrapple is unlocked.
    /// Auto-releases when the player drifts outside detectionRadius.
    /// </summary>
    public class GrappleAbility : MonoBehaviour
    {
        private const float HorizontalPullFactor = 0.3f;

        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float pullForce = 5f;
        [SerializeField] private LayerMask grappleLayer;
        [SerializeField] private LineRenderer ropeRenderer;

        [Header("Debug")]
        [SerializeField] private Color gizmoIdleColor   = new Color(0f, 1f, 0f, 0.25f);
        [SerializeField] private Color gizmoHookedColor = new Color(1f, 0.5f, 0f, 0.4f);

        private Rigidbody2D _rb;
        private Transform _hookedPoint;
        private bool _isHooked;

        private void Awake()
        {
            _rb = GetComponentInParent<Rigidbody2D>();

            if (ropeRenderer == null)
                ropeRenderer = GetComponent<LineRenderer>();
        }

        private void OnEnable()
        {
            Release();
        }

        private void FixedUpdate()
        {
            if (!_isHooked || _rb == null)
                return;

            // Pull upward each physics tick.
            _rb.AddForce(Vector2.up * pullForce, ForceMode2D.Force);

            // Apply a small horizontal pull toward the grapple point for a natural arc.
            if (_hookedPoint != null)
            {
                Vector2 toPoint = ((Vector2)_hookedPoint.position - _rb.position).normalized;
                _rb.AddForce(new Vector2(toPoint.x * pullForce * HorizontalPullFactor, 0f), ForceMode2D.Force);
            }
        }

        private void Update()
        {
            if (!_isHooked)
                return;

            // Auto-release if the hooked point is destroyed.
            if (_hookedPoint == null)
            {
                Release();
                return;
            }

            // Auto-release when the player drifts outside the detection radius.
            if (Vector2.Distance(transform.position, _hookedPoint.position) > detectionRadius)
            {
                Release();
                return;
            }

            ropeRenderer.SetPosition(0, transform.position);
            ropeRenderer.SetPosition(1, _hookedPoint.position);
        }

        /// <summary>
        /// Toggles the grapple: hooks onto the nearest GrapplePoint when free,
        /// or releases when already hooked. Called by ChunkController on Attack input.
        /// </summary>
        public void UseGrapple()
        {
            if (_isHooked)
            {
                Release();
                return;
            }

            Transform nearest = FindNearestPoint();
            if (nearest == null)
                return;

            _hookedPoint = nearest;
            _isHooked = true;

            ropeRenderer.enabled = true;
            ropeRenderer.SetPosition(0, transform.position);
            ropeRenderer.SetPosition(1, _hookedPoint.position);
        }

        /// <summary>
        /// Returns the nearest GrapplePoint Transform within detectionRadius, or null if none found.
        /// </summary>
        private Transform FindNearestPoint()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, grappleLayer);

            Transform nearest = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider2D col in hits)
            {
                if (col.GetComponent<GrapplePoint>() == null)
                    continue;

                float distance = Vector2.Distance(transform.position, col.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearest = col.transform;
                }
            }

            return nearest;
        }

        /// <summary>
        /// Releases the grapple and hides the rope.
        /// </summary>
        private void Release()
        {
            _isHooked = false;
            _hookedPoint = null;

            if (ropeRenderer != null)
                ropeRenderer.enabled = false;
        }

        // Draws the detection radius in the Scene view.
        // Orange disc when hooked, green disc when idle.
        private void OnDrawGizmos()
        {
            Gizmos.color = _isHooked ? gizmoHookedColor : gizmoIdleColor;
            Gizmos.DrawSphere(transform.position, detectionRadius);

            Gizmos.color = _isHooked ? new Color(1f, 0.5f, 0f, 1f) : new Color(0f, 1f, 0f, 1f);
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}
