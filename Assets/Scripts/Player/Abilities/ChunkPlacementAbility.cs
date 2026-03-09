using UnityEngine;
using UnityEngine.Events;
using Chunk.Player;

namespace Chunk.Player.Abilities
{
    /// <summary>
    /// Manages the scrap meter and instantiates ChunkBlock prefabs at a placement
    /// ghost position in front of Chunk. Available from Stage 2 (Torso) onwards.
    /// </summary>
    public class ChunkPlacementAbility : MonoBehaviour
    {
        private const int ScrapCostPerBlock = 3;
        private const int MaxScrap = 9;
        private const int BaseMaxPlacedChunks = 3;

        [SerializeField] private GameObject chunkBlockPrefab;
        [SerializeField] private Transform placementPoint;

        private FacingController _facingController;

        private const float PlacementOffsetX = 1.2f;

        private int _currentScrap;
        private int _placedChunks;
        private int _maxPlacedChunks = BaseMaxPlacedChunks;

        /// <summary>
        /// Fires whenever scrap changes. Broadcasts (currentScrap, maxScrap).
        /// </summary>
        public UnityEvent<int, int> OnScrapChanged = new UnityEvent<int, int>();

        private void Awake()
        {
            _facingController = GetComponentInParent<FacingController>();
        }

        private void OnEnable()
        {
            // Broadcast current state every time this object is activated (including the first
            // time after the Torso unlock), so the UI always reflects the real value.
            OnScrapChanged.Invoke(_currentScrap, MaxScrap);
        }

        private void Update()
        {
            UpdatePlacementPointOffset();
        }

        /// <summary>
        /// Adds the given scrap amount to the meter and fires OnScrapChanged.
        /// </summary>
        public void CollectScrap(int amount)
        {
            _currentScrap = Mathf.Clamp(_currentScrap + amount, 0, MaxScrap);
            Debug.Log($"[ChunkPlacementAbility] CollectScrap +{amount} → {_currentScrap}/{MaxScrap}. Listeners: {OnScrapChanged.GetPersistentEventCount()} persistent, active={gameObject.activeInHierarchy}");
            OnScrapChanged.Invoke(_currentScrap, MaxScrap);
        }

        /// <summary>
        /// Instantiates a ChunkBlock at the placement point if conditions are met.
        /// </summary>
        public void PlaceChunk()
        {
            Debug.Log($"[ChunkPlacementAbility] PlaceChunk called. Scrap={_currentScrap}/{MaxScrap}, Placed={_placedChunks}/{_maxPlacedChunks}, active={gameObject.activeInHierarchy}");

            if (_currentScrap < ScrapCostPerBlock)
            {
                Debug.Log($"[ChunkPlacementAbility] Not enough scrap. Have {_currentScrap}, need {ScrapCostPerBlock}.");
                return;
            }

            if (_placedChunks >= _maxPlacedChunks)
            {
                Debug.Log($"[ChunkPlacementAbility] Block cap reached ({_placedChunks}/{_maxPlacedChunks}).");
                return;
            }

            if (chunkBlockPrefab == null)
            {
                Debug.LogWarning("[ChunkPlacementAbility] chunkBlockPrefab is not assigned.", this);
                return;
            }

            if (placementPoint == null)
            {
                Debug.LogWarning("[ChunkPlacementAbility] placementPoint is not assigned.", this);
                return;
            }

            Instantiate(chunkBlockPrefab, placementPoint.position, Quaternion.identity);
            _currentScrap -= ScrapCostPerBlock;
            _placedChunks++;
            Debug.Log($"[ChunkPlacementAbility] Block placed. Scrap now={_currentScrap}/{MaxScrap}. Invoking OnScrapChanged with {OnScrapChanged.GetPersistentEventCount()} persistent listeners.");
            OnScrapChanged.Invoke(_currentScrap, MaxScrap);
        }

        /// <summary>
        /// Increases the placed chunk cap. Called by ChunkController on stage unlock.
        /// </summary>
        public void IncreaseBlockCap(int additionalBlocks)
        {
            _maxPlacedChunks += additionalBlocks;
        }

        private void UpdatePlacementPointOffset()
        {
            if (placementPoint == null)
                return;

            bool facingLeft = _facingController != null && _facingController.FacingLeft;
            float offsetX = facingLeft ? -PlacementOffsetX : PlacementOffsetX;
            placementPoint.localPosition = new Vector3(offsetX, 0f, 0f);
        }
    }
}
