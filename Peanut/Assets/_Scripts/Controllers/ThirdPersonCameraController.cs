// Credits to: Mike Oomen with the base of the camera controller.
// Further adjustments like PlayerInput and player actions by me, therefore credit me.
// Copyright Niek Melet

using UnityEngine;

namespace _Scripts.Controllers
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [Tooltip("Make sure to insert the target component of the target.")]
        public Player.Player PlayerInput;
        
        [Header("Collision Detection")]
        [Tooltip("The size of the sphere cast to check the collision.")]
        [SerializeField] private float sphereSize;
        
        [Tooltip("THe original position of the camera. The camera would automatically go towards this position.")]
        [SerializeField] private Vector3 originalPosition;

        [Space(10)]
        
        [Range(1f, 50f)]
        [Tooltip("The adjustment speed of the camera.")]
        [SerializeField] private float adjustSpeed = 25;

        [Range(1f, 10f)] 
        [Tooltip("The slow adjustment speed once the camera is slow.")]
        [SerializeField] private float slowAdjustmentSpeed;

        [Range(1f, 10f)]
        [Tooltip("The distance at which the camera should slow down.")]
        [SerializeField] private float slowRange;

        [Header("Camera Properties")]
        
        [Range(1, 10)]
        [Tooltip("The sensitivity of the camera. This is both applicable for Mouse & Gamepad input.")]
        public int Sensitivity = 5;
        
        [Space(5)]
        
        [Tooltip("Apply the child(Camera) to this variable.")]
        [SerializeField] private Transform child;
        
        [Tooltip("Target is the target the camera should follow, most cases this is player.")]
        [SerializeField] private Transform target;
        
        [Space(10)]
        [Tooltip("The layer the camera is colliding with.")]
        [SerializeField] private LayerMask collLayer;
        
        private bool isSlow; // flag indicates if the camera should be slow
        private bool stop; // flag indicates if the camera should stop
        
        private Vector2 input; // the input of the Player Actions
        private GameObject obj; // the last object the camera collided with.

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            transform.position = 
                new Vector3(target.position.x, target.position.y + originalPosition.y, target.position.z);

            HandleInput();
        }

        void LateUpdate()
        {
            RotateCamera();
            HandleCameraCollision();
        }

        void FixedUpdate()
        {
            CheckCameraCollision();
        }

        /// <summary>
        /// Handles target input for camera movement.
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
        /// Rotates the camera based on target input.
        /// </summary>
        private void RotateCamera()
        {
            // Calculate the vertical and horizontal input for camera rotation
            float yInput = -input.y * Sensitivity * Time.deltaTime;
            float xInput = input.x * Sensitivity * Time.deltaTime;

            // Calculate the current rotation angle around the X-axis, ensuring it stays within the desired range
            float angleX = transform.eulerAngles.x > 256 ? transform.eulerAngles.x - 360 : transform.eulerAngles.x;
            float currentRotationX = Mathf.Clamp(angleX + yInput, -60, 60);

            // Calculate the new rotation around the Y-axis
            float yRotation = transform.eulerAngles.y + xInput;

            // Apply the new rotation to the camera transform
            // Note: Using Quaternion.Euler to construct a rotation from Euler angles
            // while preserving the original Z-axis rotation
            transform.rotation = Quaternion.Euler(currentRotationX, yRotation, transform.rotation.z);
        }

        /// <summary>
        /// Handles camera collision detection and adjustment to prevent clipping through objects.
        /// </summary>
        private void HandleCameraCollision()
        {
            // Determine the adjustment speed based on whether the camera is in slow mode or regular mode
            float speed = isSlow ? slowAdjustmentSpeed : adjustSpeed;

            // Check if the camera is currently colliding with any objects
            if (IsColliding())
            {
                // Adjust the camera position to move away from the colliding object
                child.localPosition = Vector3.MoveTowards(child.localPosition,
                    new Vector3(child.localPosition.x, child.localPosition.y, transform.position.z),
                    speed * Time.deltaTime);
            }
            else if (!IsColliding() && !stop)
            {
                // Move the child back to its original position if not colliding with the ground and not stopped
                child.localPosition = Vector3.MoveTowards(child.localPosition,
                    new Vector3(0, 0, originalPosition.z), slowAdjustmentSpeed * Time.deltaTime);
            }
        }

        /// <summary>
        /// Checks for camera collision with objects in its path.
        /// </summary>
        private void CheckCameraCollision()
        {
            // Check for collision using a sphere cast from the camera position
            stop = CollCheck(Physics.OverlapSphere(child.position, sphereSize, collLayer), out GameObject tempObj);

            // If a colliding object is found, assign it to 'obj'
            if (tempObj != null)
                obj = tempObj;

            // If 'obj' is still null, return as there is no collision
            if (obj == null)
                return;

            // Determine if the camera should switch to slow mode based on distance to object and angle
            isSlow = Vector3.Distance(obj.transform.position, child.position) < slowRange &&
                     Vector3.Dot(-child.forward, obj.transform.position - child.position) > 0.0f ||
                     Physics.Raycast(child.position, -child.forward, slowRange, collLayer);
        }

        /// <summary>
        /// Checks if something is obstructing the camera's view.
        /// </summary>
        /// <returns>Returns true if obstructed, false otherwise.</returns>
        private bool IsColliding()
        {
            // Check if there are any obstructions between camera and target using ray-casts
            return Physics.Raycast(child.position, (transform.position - child.position).normalized, 
                       Vector3.Distance(child.position, transform.position), collLayer) ||
                   Physics.Raycast(transform.position, (child.position - transform.position).normalized, 
                       Vector3.Distance(child.position, transform.position), collLayer) ||
                   
                   // Additionally, check for collisions within a sphere around the camera position
                   CollCheck(Physics.OverlapSphere(child.position, transform.localScale.x / 2.1f, collLayer));
        }


        /// <summary>
        /// Checks if any col overlap with the camera.
        /// </summary>
        /// <param name="col">An array of col to check for overlap.</param>
        /// <returns>Returns true if col overlap, false otherwise.</returns>
        private bool CollCheck(Collider[] col)
        {
            foreach (var coll in col)
            {
                // if any collider is not null (indicating a collision), return true
                if (coll != null) return true;
            }
            return false;
        }
        
        /// <summary>
        /// Checks an array of colliders for any non-null elements and retrieves the corresponding GameObject.
        /// </summary>
        /// <param name="col">An array of Collider objects to check.</param>
        /// <param name="go">Out parameter to store the first non-null GameObject found.</param>
        /// <returns>True if a non-null Collider is found and its GameObject is assigned to 'go'; otherwise, returns false.</returns>
        private bool CollCheck(Collider[] col, out GameObject go)
        {
            go = null;
            
            foreach (var coll in col)
            {
                if (coll != null) { go = coll.gameObject; return true; }
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
            // the available control schemes
            string[] gamepadControlSchemes = { "Gamepad", "Gamepad&Mouse" };

            // iterate through the schemes
            foreach (string scheme in gamepadControlSchemes)
            {
                // return true if found
                if (controlScheme.Equals(scheme))
                {
                    return true;
                }
            }

            return false;
        }
    }
}