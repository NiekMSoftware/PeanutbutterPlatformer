using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Scripts.Controllers
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class CamController : MonoBehaviour
    {
        public PlayerInput PlayerInput;
        [Space(10)]
        public Transform Target;
        [Range(10f, 100f)]
        public float Sensitivity = 5f;

        [Header("Distance Limitations")]
        public float Distance = 5f;
        public float MinDistance = 2f;
        public float MaxDistance = 10f;

        [Header("Zooming Properties")]
        public float ZoomInSpeed = 0.1f;
        public float ZoomOutSpeed = 0.5f;
        public float SmoothTime = 0.1f;

        [Space(10)]
        public LayerMask CollisionLayers;

        private Vector2 lookInput = Vector2.zero;
        private Vector3 desiredPosition;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            desiredPosition = transform.position - Target.position;
        }

        void Update()
        {
            // Read the current look input from the PlayerInput component
            lookInput = PlayerInput.actions["Look"].ReadValue<Vector2>();

            // Check if the input is coming from a gamepad
            if (IsGamepadControlScheme(PlayerInput.currentControlScheme))
                // Apply sensitivity multiplier for gamepad input
                lookInput *= 6.0f;
            else
                // Apply sensitivity multiplier for mouse input
                lookInput *= 1.0f;

            HandleZoom();
        }

        void LateUpdate()
        {
            // make sure the camera follows the player smoothly
            transform.position = Vector3.Lerp(transform.position, GetDesiredPosition(), 
                Time.deltaTime * Sensitivity);

            // make the camera look at the target
            transform.LookAt(Target);
        }

        /// <summary>
        /// Handles the rotation and movement of the third person camera.
        /// </summary>
        private Vector3 GetDesiredPosition()
        {
            // Calculate rotation based on input
            float horizontalLook = lookInput.x * Sensitivity * Time.deltaTime;
            float verticalLook = lookInput.y * Sensitivity * Time.deltaTime;

            // Apply rotation to the camera around the target
            Quaternion rotation = Quaternion.Euler(transform.eulerAngles.x - verticalLook, transform.eulerAngles.y + horizontalLook, 0f);
            desiredPosition = rotation * desiredPosition;

            // Adjust the angleX after rotation calculation
            float angleX = transform.localEulerAngles.x;
            if (angleX > 256)
            {
                angleX -= 360;
            }

            // Apply the corrected vertical rotation directly before rotation calculation
            Quaternion adjustedRotation = Quaternion.Euler(Mathf.Clamp(angleX - verticalLook, -80, 80), transform.eulerAngles.y + horizontalLook, 0f);

            Vector3 position = Target.position + (adjustedRotation * desiredPosition.normalized * Distance);

            // Handle collision detection
            if (Physics.Linecast(Target.position, position, out var hit, CollisionLayers))
                position = hit.point;

            return position;
        }

        /// <summary>
        /// Will handle the zoom of the camera, this basically just moves the player camera closer to the player.
        /// </summary>
        private void HandleZoom()
        {
            float zoomInput = -PlayerInput.actions["Zoom"].ReadValue<float>();

            // Adjust zoom speed based on scroll direction
            float zoomSpeed = zoomInput > 0 ? ZoomInSpeed : ZoomOutSpeed;

            // Scale the scroll input by the appropriate zoom speed
            zoomInput *= zoomSpeed;

            // Calculate target zoom level based on scaled scroll input
            float targetDistance = Distance + zoomInput;
            targetDistance = Mathf.Clamp(targetDistance, MinDistance, MaxDistance);

            // Smoothly interpolate zoom level towards the target
            Distance = Mathf.Lerp(Distance, targetDistance, SmoothTime);
        }

        private bool IsGamepadControlScheme(string controlScheme)
        {
            // Define your control scheme names for gamepads here
            // You may need to adjust these based on your actual control schemes
            string[] gamepadControlSchemes = { "Gamepad", "Gamepad&Mouse" };

            foreach (string scheme in gamepadControlSchemes)
            {
                if (controlScheme.Equals(scheme))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
