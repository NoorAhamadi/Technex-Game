using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Chunk.Player.Abilities;

namespace Chunk.Player
{
    /// <summary>
    /// Master controller on the Chunk root GameObject.
    /// Tracks which stages are unlocked, routes Input System events to the
    /// correct ability, and exposes the unlock API for pickups to call.
    /// </summary>
    public class ChunkController : MonoBehaviour
    {
        [Header("Ability GameObjects")]
        [SerializeField] private GameObject rollAbilityObject;
        [SerializeField] private GameObject moveAbilityObject;
        [SerializeField] private GameObject chunkPlacementObject;
        [SerializeField] private GameObject grappleAbilityObject;

        [Header("Input")]
        [SerializeField] private InputActionAsset inputActions;

        /// <summary>Fires whenever a new stage is unlocked, passing the StageType.</summary>
        public UnityEvent<StageType> OnStageUnlocked = new UnityEvent<StageType>();

        private readonly Dictionary<StageType, bool> _unlockedStages = new Dictionary<StageType, bool>();
        private RollAbility _rollAbility;
        private MovementAbility _moveAbility;
        private ChunkPlacementAbility _placementAbility;
        private GrappleAbility _grappleAbility;

        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _attackAction;
        private InputAction _spawnAction;
        private InputAction _grappleAction;

        private Vector2 _moveInput;

        private void Awake()
        {
            // Cache ability references.
            if (rollAbilityObject != null)
                _rollAbility = rollAbilityObject.GetComponent<RollAbility>();
            if (moveAbilityObject != null)
                _moveAbility = moveAbilityObject.GetComponent<MovementAbility>();
            if (chunkPlacementObject != null)
                _placementAbility = chunkPlacementObject.GetComponent<ChunkPlacementAbility>();
            if (grappleAbilityObject != null)
                _grappleAbility = grappleAbilityObject.GetComponent<GrappleAbility>();

            // Head is always unlocked at start.
            foreach (StageType stage in Enum.GetValues(typeof(StageType)))
                _unlockedStages[stage] = false;
            _unlockedStages[StageType.Head] = true;

            // Bind input actions from asset.
            if (inputActions != null)
            {
                _moveAction   = inputActions.FindAction("Player/Move",    throwIfNotFound: false);
                _jumpAction   = inputActions.FindAction("Player/Jump",    throwIfNotFound: false);
                _attackAction = inputActions.FindAction("Player/Attack",  throwIfNotFound: false);
                _spawnAction  = inputActions.FindAction("Player/Spawn",   throwIfNotFound: false);
                _grappleAction = inputActions.FindAction("Player/Grapple", throwIfNotFound: false);
            }
        }

        private void OnEnable()
        {
            inputActions?.Enable();

            if (_jumpAction    != null) _jumpAction.performed    += OnJump;
            if (_attackAction  != null) _attackAction.performed  += OnAttack;
            if (_spawnAction   != null) _spawnAction.performed   += OnPlace;
            if (_grappleAction != null) _grappleAction.performed += OnGrapple;
        }

        private void OnDisable()
        {
            if (_jumpAction    != null) _jumpAction.performed    -= OnJump;
            if (_attackAction  != null) _attackAction.performed  -= OnAttack;
            if (_spawnAction   != null) _spawnAction.performed   -= OnPlace;
            if (_grappleAction != null) _grappleAction.performed -= OnGrapple;

            inputActions?.Disable();
        }

        private void Update()
        {
            // Move is a 1D Axis composite (A/D → float), not a Vector2.
            float moveX = _moveAction?.ReadValue<float>() ?? 0f;
            _moveInput = new Vector2(moveX, 0f);

            if (HasStage(StageType.Torso))
            {
                _moveAbility?.Move(_moveInput.x);
            }
            else if (HasStage(StageType.Head))
            {
                _rollAbility?.Roll(_moveInput.x);
            }
        }

        /// <summary>
        /// Unlocks the given stage, enables its ability GameObject, and fires OnStageUnlocked.
        /// </summary>
        public void UnlockStage(StageType stage)
        {
            if (_unlockedStages.TryGetValue(stage, out bool alreadyUnlocked) && alreadyUnlocked)
            {
                Debug.LogWarning($"[ChunkController] Stage {stage} is already unlocked.", this);
                return;
            }

            _unlockedStages[stage] = true;
            EnableAbilityForStage(stage);
            OnStageUnlocked.Invoke(stage);
        }

        /// <summary>
        /// Returns true if the given stage has been unlocked.
        /// </summary>
        public bool HasStage(StageType stage)
        {
            return _unlockedStages.TryGetValue(stage, out bool unlocked) && unlocked;
        }

        private void EnableAbilityForStage(StageType stage)
        {
            switch (stage)
            {
                case StageType.Torso:
                    moveAbilityObject?.SetActive(true);
                    // Torso also unlocks block placement.
                    chunkPlacementObject?.SetActive(true);
                    CachePlacementAbility();
                    break;
                case StageType.ArmGrapple:
                    grappleAbilityObject?.SetActive(true);
                    if (_grappleAbility == null && grappleAbilityObject != null)
                        _grappleAbility = grappleAbilityObject.GetComponent<GrappleAbility>();
                    break;
                case StageType.ArmBeam:
                case StageType.ArmSaw:
                case StageType.Legs:
                    chunkPlacementObject?.SetActive(true);
                    CachePlacementAbility();
                    break;
            }
        }

        private void CachePlacementAbility()
        {
            if (_placementAbility == null && chunkPlacementObject != null)
                _placementAbility = chunkPlacementObject.GetComponent<ChunkPlacementAbility>();
        }

        private void OnJump(InputAction.CallbackContext ctx)
        {
            if (HasStage(StageType.Torso))
                _moveAbility?.Jump();
        }

        private void OnAttack(InputAction.CallbackContext ctx)
        {
            // Reserved for future attack ability.
        }

        private void OnGrapple(InputAction.CallbackContext ctx)
        {
            if (HasStage(StageType.ArmGrapple))
                _grappleAbility?.UseGrapple();
        }

        private void OnPlace(InputAction.CallbackContext ctx)
        {
            if (_placementAbility == null && chunkPlacementObject != null)
                _placementAbility = chunkPlacementObject.GetComponent<ChunkPlacementAbility>();

            Debug.Log($"[ChunkController] OnPlace fired. _placementAbility={((_placementAbility == null) ? "NULL" : "found")}, chunkPlacementObject active={chunkPlacementObject?.activeSelf}");
            _placementAbility?.PlaceChunk();
        }
    }
}
