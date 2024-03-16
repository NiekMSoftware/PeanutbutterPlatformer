using _Scripts.Controllers;
using _Scripts.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts
{
    /// <summary>
    /// The class that would contain all the important Player matters.
    /// For example this would see if the player has died or not so the player can't do anything anymore!
    /// Check out <see cref="ControlPlayer"/> for more information!
    /// <seealso cref="PlayerController"/>
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class Player : MonoBehaviour
    {
        [Header("Player Specific Data")]
        public PlayerData PlayerEntity;
        public PlayerInput PlayerInputSystem;
        private PlayerController _controller;

        #region Player Input
        
        [HideInInspector] public float XMovement;
        [HideInInspector] public float ZMovement;
        [HideInInspector] public bool PressedJump;
        [HideInInspector] public bool HoldingJump;
        [HideInInspector] public bool ReleasedJump;
        [HideInInspector] public Vector2 LookInput;

        #endregion

        /// <summary>
        /// Initializes all the data from the player.
        /// </summary>
        private void Init()
        {
            // initialize the player's health to the max health
            PlayerEntity.Health = PlayerEntity.MaxHealth;

            _controller = GetComponent<PlayerController>();
        }
        
        void Start()
        {
            Init();
        }

        void Update()
        {
            GatherInput();
            ControlPlayer();
        }

        /// <summary>
        /// The function to control the player. This is done by checking if the player has died or not.
        /// If they did then player is unable to give in inputs to the game to move their character!
        /// <seealso cref="PlayerController"/>
        /// </summary>
        private void ControlPlayer()
        {
            // check if the player is not dead, if so they can move!
            if (!PlayerEntity.HasDied())
            {
                // move!
                _controller.HandlePlayer();
            }
            else
            {
                // player has died!
                Debug.LogWarning("Player can not do anything anymore!" +
                                 "\nThey have died!");
            }
        }

        private void GatherInput()
        {
            XMovement = PlayerInputSystem.actions["Move"].ReadValue<Vector2>().x;
            ZMovement = PlayerInputSystem.actions["Move"].ReadValue<Vector2>().y;

            PressedJump = PlayerInputSystem.actions["Jump"].WasPressedThisFrame();
            HoldingJump = PlayerInputSystem.actions["Jump"].IsPressed();
            ReleasedJump = PlayerInputSystem.actions["Jump"].WasReleasedThisFrame();

            LookInput = PlayerInputSystem.actions["Look"].ReadValue<Vector2>();
        }
    }
}