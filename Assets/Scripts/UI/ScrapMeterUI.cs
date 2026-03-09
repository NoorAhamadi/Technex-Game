using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Chunk.Player.Abilities;

namespace Chunk.UI
{
    /// <summary>
    /// Displays the current scrap count and whether Chunk can place a block.
    /// Subscribes to ChunkPlacementAbility.OnScrapChanged.
    /// </summary>
    public class ScrapMeterUI : MonoBehaviour
    {
        private const int ScrapCostPerBlock = 3;

        [SerializeField] private ChunkPlacementAbility placementAbility;
        [SerializeField] private TextMeshProUGUI scrapLabel;

        /// <summary>
        /// Individual pip images indexed 0-8. Green = filled, grey = empty.
        /// </summary>
        [SerializeField] private Image[] pips;

        private static readonly Color PipFilled = new Color(0.25f, 0.85f, 0.35f);
        private static readonly Color PipEmpty   = new Color(0.25f, 0.25f, 0.25f);
        private static readonly Color PipReady   = new Color(1f,    0.85f, 0.1f);

        private void OnEnable()
        {
            if (placementAbility != null)
                placementAbility.OnScrapChanged.AddListener(OnScrapChanged);
        }

        private void OnDisable()
        {
            if (placementAbility != null)
                placementAbility.OnScrapChanged.RemoveListener(OnScrapChanged);
        }

        private void Start()
        {
            // Initialise to zero.
            OnScrapChanged(0, pips.Length);
        }

        private void OnScrapChanged(int current, int max)
        {
            Debug.Log($"[ScrapMeterUI] OnScrapChanged received: {current}/{max}. Label null={scrapLabel == null}, Pips={pips.Length}");

            if (scrapLabel != null)
                scrapLabel.text = $"Scrap  {current}/{max}";

            bool canPlace = current >= ScrapCostPerBlock;

            for (int i = 0; i < pips.Length; i++)
            {
                if (pips[i] == null) continue;

                if (i < current)
                    pips[i].color = canPlace ? PipReady : PipFilled;
                else
                    pips[i].color = PipEmpty;
            }
        }
    }
}
