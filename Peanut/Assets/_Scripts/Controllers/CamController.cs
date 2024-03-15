using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Assets._Scripts.Controllers
{
    public class CamController : MonoBehaviour
    {
        [Tooltip("Make sure to insert the PlayerInput component of the player.")]
        public PlayerInput PlayerInput;

        [Header("Collision Detection")]
        [SerializeField] private float boxSize;
        [SerializeField] private Vector3 originalPosition;

        [Space(10)]
        [Range(1f, 20f)]
        [SerializeField] private float adjustSpeed = 25;

        [Range(1f, 10f)] 
        [SerializeField] private float slowAdjustmentSpeed;

        [Range(1f, 5f)]
        [SerializeField] private float slowRange;

        [Header("Camera Properties")]
        [SerializeField] private Transform child;
        [SerializeField] private Transform Target;

        [Space(10)]
        [SerializeField] private LayerMask collisionLayer;

        private bool isTouchingGround;
        private bool isSlow;
        private bool shouldStop;
        private Vector2 input;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            input = PlayerInput.actions["Look"].ReadValue<Vector2>();

            // Check if the input is coming from a gamepad
            if (IsGamepadControlScheme(PlayerInput.currentControlScheme))
                // Apply sensitivity multiplier for gamepad input
                input *= 6.0f;
            else
                // Apply sensitivity multiplier for mouse input
                input *= 1.0f;

            HandleCameraCollision();
        }

        void LateUpdate()
        {
            transform.position = Target.position;
            RotateCamera();
        }

        void FixedUpdate()
        {
            CheckCameraCollision();
        }

        private void RotateCamera()
        {
            float yInput = -input.y * adjustSpeed * Time.deltaTime;
            float xInput = input.x * adjustSpeed * Time.deltaTime;

            float angleX = transform.eulerAngles.x > 256 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
            float currentRotationX = Mathf.Clamp(angleX + yInput, -60, 60);

            float yRotation = transform.eulerAngles.y + xInput;

            transform.rotation = Quaternion.Euler(currentRotationX, yRotation, transform.rotation.z);

            // look at the player
            transform.LookAt(Target);
        }

        private void HandleCameraCollision()
        {
            float speed = isSlow ? slowAdjustmentSpeed : adjustSpeed;

            if (IsColliding())
            {
                // Adjust the child's position to avoid colliding with the ground
                child.localPosition = 
                    Vector3.MoveTowards(child.localPosition, new Vector3(child.localPosition.x, child.localPosition.y, transform.localPosition.z),
                        speed * Time.deltaTime);
            }
            else if (!IsColliding() && !shouldStop)
            {
                // Move the child back to its original position if not colliding with the ground and not stopped
                child.localPosition = Vector3.MoveTowards(child.localPosition, originalPosition, speed * Time.deltaTime);
            }
        }

        private void CheckCameraCollision()
        {
            shouldStop = CheckCollidersOverlap(Physics.OverlapBox(child.position,
                new Vector3(boxSize, boxSize, boxSize), Quaternion.identity, collisionLayer));
            isSlow = Physics.Raycast(child.position, -child.forward, slowRange, collisionLayer);
        }

        /// <summary>
        /// Checks if something is in between the player and the camera.
        /// </summary>
        /// <returns>Returns true if something is in between the player and the camera else false.</returns>
        bool IsColliding()
        {
            return Physics.Raycast(child.position, transform.position, Vector3.Distance(child.position, transform.position), collisionLayer) ||
                   Physics.Raycast(transform.position, child.position, Vector3.Distance(child.position, transform.position), collisionLayer) ||
                   CheckCollidersOverlap(Physics.OverlapBox(child.position, transform.localScale / 2, Quaternion.identity, collisionLayer));
        }

        private bool CheckCollidersOverlap(Collider[] colliders)
        {
            foreach (var col in colliders)
            {
                if (col != null)
                    return true;
            }
            return false;
        }

        private bool IsGamepadControlScheme(string controlScheme)
        {
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