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

        /// <summary>
        /// Draws a Ray from the origin position.
        /// </summary>
        /// <param name="origin">Original position from where the ray starts.</param>
        /// <param name="distance">The distance from origin to point.</param>
        /// <param name="duration">The duration of how long it would be rendered in the scene view.</param>
        public static void DrawRayDown(Vector3 origin, float distance, float duration)
        {
            Debug.DrawRay(origin, new Vector3(0, -distance), Color.red, duration);
        }

        /// <summary>
        /// Draws a wireframe sphere at the specified position with the given radius and color using Gizmos.
        /// </summary>
        /// <param name="center">The center of the sphere.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <param name="color">The color of the sphere.</param>
        public static void DrawWireframeSphere(Vector3 center, float radius, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}