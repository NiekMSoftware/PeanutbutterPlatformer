using UnityEngine;

namespace Assets._Scripts
{
    public static class Helper
    {
        /// <summary>
        ///  Returns a Vector3 with a decreased distance based on start, end and speed with delta time.
        /// </summary>
        /// <param name="start">Where you start</param>
        /// <param name="end">Where you want to end up</param>
        /// <param name="speed">How fast you want to change the Vector3</param>
        /// <returns></returns>
        public static Vector3 GoToDelta(Vector3 start, Vector3 end, float speed = 1)
        {
            if (start.x < end.x) { start.x += speed * Time.deltaTime; }
            else if (start.x > end.x) { start.x -= speed * Time.deltaTime; }

            if (start.y < end.y) { start.y += speed * Time.deltaTime; }
            else if (start.y > end.y) { start.y -= speed * Time.deltaTime; }

            if (start.z < end.z) { start.z += speed * Time.deltaTime; }
            else if (start.z > end.z) { start.z -= speed * Time.deltaTime; }

            if (Vector3.Distance(start, end) < 0.5f) { start = end; }

            return start;
        }
    }
}