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
            HandleZoom();
            ThirdPersonCamera();
        }

        private void HandleZoom()
        {
            float zoomInput = -PlayerInput.actions["Zoom"].ReadValue<Vector2>().y;

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

        private void ThirdPersonCamera()
        {
            // Read the current look input from the PlayerInput component
            lookInput = PlayerInput.actions["Look"].ReadValue<Vector2>();

            // Calculate rotation based on input
            float horizontalLook = lookInput.x * Sensitivity * Time.deltaTime;
            float verticalLook = lookInput.y * Sensitivity * Time.deltaTime;

            // Apply rotation to the camera around the target
            Quaternion rotation = Quaternion.Euler(verticalLook, horizontalLook, 0f);
            desiredPosition = rotation * desiredPosition;
            Vector3 position = Target.position + desiredPosition.normalized * Distance;

            // Handle collision detection
            RaycastHit hit;
            if (Physics.Linecast(Target.position, position, out hit, CollisionLayers))
            {
                position = hit.point;
            }

            // Smoothly move the camera towards the desired position
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * Sensitivity);

            // Make the camera always look at the target
            transform.LookAt(Target);
        }
    }

}
