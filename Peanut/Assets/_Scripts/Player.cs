using _Scripts.Controllers;
using _Scripts.Entities;
using UnityEngine;

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
        [SerializeField] private PlayerController _controller;

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
    }
}