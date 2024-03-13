using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Scripts
{
    public static class DebugHelper
    {
        /// <summary>
        /// Debugs the input values for player look direction.
        /// </summary>
        /// <param name="playerInput">The PlayerInput object representing the input controller for the player.</param>
        public static void DebugLookInputValues(PlayerInput playerInput)
        {
            // debug the input value
            Vector2 lookInput = Vector2.zero;

            // check if there's a player input component
            if (playerInput != null)
                lookInput = playerInput.actions["Look"].ReadValue<Vector2>();
            else
            {
                Debug.LogError("Oh no, there's no component like that sir.");
                return;
            }

            float horizontal = lookInput.x;
            float vertical = lookInput.y;

            Debug.Log($"Horizontal: {horizontal}, Vertical: {vertical}");
        }
    }
}