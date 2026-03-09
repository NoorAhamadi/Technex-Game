using UnityEngine;
using Chunk.Player;

namespace Chunk.Player
{
    /// <summary>
    /// Listens to ChunkController.OnStageUnlocked and enables the matching
    /// visual child GameObject to display the newly attached body part.
    /// </summary>
    public class ChunkAssemblyVisuals : MonoBehaviour
    {
        /// <summary>
        /// Array of visual GameObjects indexed by (int)StageType.
        /// Assign Head, Torso, ArmGrapple, ArmBeam, Legs, ArmSaw in order.
        /// </summary>
        [SerializeField] private GameObject[] stageVisuals;

        private ChunkController _controller;

        private void Start()
        {
            _controller = GetComponent<ChunkController>();
            if (_controller == null)
            {
                Debug.LogWarning("[ChunkAssemblyVisuals] No ChunkController found on this GameObject.", this);
                return;
            }

            // Deactivate all visuals except Head (index 0) at runtime.
            for (int i = 0; i < stageVisuals.Length; i++)
            {
                if (stageVisuals[i] != null)
                    stageVisuals[i].SetActive(i == (int)StageType.Head);
            }

            _controller.OnStageUnlocked.AddListener(OnStageUnlocked);
        }

        private void OnDestroy()
        {
            if (_controller != null)
                _controller.OnStageUnlocked.RemoveListener(OnStageUnlocked);
        }

        private void OnStageUnlocked(StageType stage)
        {
            int index = (int)stage;
            if (index < stageVisuals.Length && stageVisuals[index] != null)
            {
                stageVisuals[index].SetActive(true);
            }
        }
    }
}
