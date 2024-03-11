using UnityEngine;

namespace Assets._Scripts
{
    public static class Helper
    {
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
    }
}