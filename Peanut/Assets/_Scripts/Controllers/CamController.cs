using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets._Scripts.Controllers
{
    public class CamController : MonoBehaviour
    {
        public PlayerInput PlayerInput;

        public Quaternion SafeRotation { get; set; } = Quaternion.identity;

        public Vector3 BoxSize;
        [SerializeField] private Vector3 originalPosition;
        [Range(1f, 20f)]
        [SerializeField] private float adjustSpeed = 25;
        [SerializeField] private Transform child;
        public Transform Target;
        [SerializeField] private LayerMask collisionLayer;

        private bool isTouchingGround;
        private bool shouldStop;
        private Vector2 input;


        private void Start()
        {
            // Initialize camera rotation to the safe quaternion
            transform.rotation = SafeRotation;
        }

        private void Update()
        {
            input = PlayerInput.actions["Look"].ReadValue<Vector2>();
            // Check if the input is coming from a gamepad
            if (IsGamepadControlScheme(PlayerInput.currentControlScheme))
                // Apply sensitivity multiplier for gamepad input
                input *= 6.0f;
            else
                // Apply sensitivity multiplier for mouse input
                input *= 1.0f;

            transform.position = Target.position;

            HandleCameraCollision();
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
            if (isTouchingGround)
            {
                // Adjust the child's position to avoid colliding with the ground
                child.localPosition = Vector3.MoveTowards(child.localPosition,
                    new Vector3(child.localPosition.x, child.localPosition.y, transform.position.z), adjustSpeed * Time.deltaTime);
            }
            else if (!isTouchingGround && !shouldStop)
            {
                // Move the child back to its original position if not colliding with the ground and not stopped
                child.localPosition = Vector3.MoveTowards(child.localPosition, originalPosition, adjustSpeed * Time.deltaTime);
            }
        }

        private void CheckCameraCollision()
        {
            // Check if the camera or its child is colliding with objects
            isTouchingGround = Physics.Linecast(child.position, transform.position, collisionLayer) || Physics.Linecast(transform.position, child.position, collisionLayer) 
                || CheckCollidersOverlap(Physics.OverlapBox(child.position, transform.localScale / 2));

            // Check if any colliders are overlapping with the child's bounding box
            shouldStop = CheckCollidersOverlap(Physics.OverlapBox(child.position, BoxSize, Quaternion.identity, collisionLayer));
        }

        private bool CheckCollidersOverlap(Collider[] colliders)
        {
            foreach (var collider in colliders)
            {
                if (collider != null)
                    return true;
            }
            return false;
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