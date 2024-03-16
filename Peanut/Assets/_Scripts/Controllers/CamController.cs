// Credits to: Mike Oomen with the base of the camera controller.
// Further adjustments like PlayerInput and player actions by me, therefore credit me.
// Copyright Niek Melet

using UnityEngine;
using UnityEngine.InputSystem;

namespace _Scripts.Controllers
{
    public class CamController : MonoBehaviour
    {
        [Tooltip("Make sure to insert the Player component of the player.")]
        public Player PlayerInput;

        [Header("Collision Detection")]
        [SerializeField] private float boxSize = 0;
        [SerializeField] private Vector3 originalPosition;

        [Space(10)]
        [Range(1f, 20f)]
        [SerializeField] private float adjustSpeed = 25;

        [Range(1f, 10f)] 
        [SerializeField] private float slowAdjustmentSpeed;

        [Range(1f, 10f)]
        [SerializeField] private float slowRange;

        [Header("Camera Properties")]
        [SerializeField] private Transform child;
        [SerializeField] private Transform Target;

        [Space(10)]
        [SerializeField] private LayerMask collisionLayer;

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
            HandleInput();
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

        /// <summary>
        /// Handles player input for camera movement.
        /// </summary>
        private void HandleInput()
        {
            input = PlayerInput.LookInput;

            // Check if the input is coming from a gamepad
            if (IsGamepadControlScheme(PlayerInput.PlayerInputSystem.currentControlScheme))
                // Apply sensitivity multiplier for gamepad input
                input *= 6.0f;
            else
                // Apply sensitivity multiplier for mouse input
                input *= 1.0f;
        }

        /// <summary>
        /// Rotates the camera based on player input.
        /// </summary>
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

        /// <summary>
        /// Handles camera collision detection and adjustment.
        /// </summary>
        private void HandleCameraCollision()
        {
            float speed = isSlow ? slowAdjustmentSpeed : adjustSpeed;

            if (IsColliding())
            {
                // Adjust the child's position to avoid colliding with the ground
                var localPosition = child.localPosition;
                
                localPosition = Vector3.MoveTowards(localPosition, new Vector3(localPosition.x, 
                            localPosition.y, transform.localPosition.z), speed * Time.deltaTime);
                
                child.localPosition = localPosition;
            }
            else if (!IsColliding() && !shouldStop)
            {
                // Move the child back to its original position if not colliding with the ground and not stopped
                child.localPosition = Vector3.MoveTowards(child.localPosition,
                    originalPosition, speed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Checks for camera collision with objects in its path.
        /// </summary>
        private void CheckCameraCollision()
        {
            shouldStop = CheckCollidersOverlap(Physics.OverlapBox(child.position,
                new Vector3(boxSize, boxSize, boxSize), Quaternion.identity, collisionLayer));
            isSlow = Physics.Raycast(child.position, -child.forward, slowRange, collisionLayer);
        }

        /// <summary>
        /// Checks if something is obstructing the camera's view.
        /// </summary>
        /// <returns>Returns true if obstructed, false otherwise.</returns>
        private bool IsColliding()
        {
            return Physics.Raycast(child.position, (transform.position - child.position).normalized,
                       Vector3.Distance(child.position, transform.position), collisionLayer) ||
                   Physics.Raycast(transform.position, (child.position - transform.position).normalized,
                       Vector3.Distance(child.position, transform.position), collisionLayer) ||
                   CheckCollidersOverlap(Physics.OverlapBox(child.position, transform.localScale / 2, Quaternion.identity, collisionLayer));
        }

        /// <summary>
        /// Checks if any colliders overlap with the camera.
        /// </summary>
        /// <param name="colliders">An array of colliders to check for overlap.</param>
        /// <returns>Returns true if colliders overlap, false otherwise.</returns>
        private bool CheckCollidersOverlap(Collider[] colliders)
        {
            foreach (var col in colliders)
            {
                if (col != null)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if the current control scheme is a gamepad.
        /// </summary>
        /// <param name="controlScheme">The current control scheme to check.</param>
        /// <returns>Returns true if the control scheme is a gamepad, false otherwise.</returns>
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