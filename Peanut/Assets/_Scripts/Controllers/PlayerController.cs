using Assets._Scripts;
using UnityEngine;

namespace _Scripts.Controllers
{
    /// <summary>
    /// The controller of the player. This class is responsible to handle everything the player wants to do.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody playerBod;

        [Header("Jumping Properties")] 
        [SerializeField] private float jumpTimer = .5f;
        private float jumpTimerCountdown;

        [Header("Gravity Manipulation")]
        [SerializeField] private float gravityScale;
        [SerializeField] private float fallingSpeed;

        [Header("Ground Check")] 
        public LayerMask GroundLayer;
        public Transform GroundChecker;
        [Range(0f, 2f)] public float Radius = .5f;

        [Header("Camera Properties")] 
        public Transform TpCamera;

        public float TurnSmoothTime = 0.1f;
        private float turnSmoothVelocity;
        
        // player class
        private Player player;

        #region Movement Properties
        
        private float _speed;
        private float _drag;
        private float _jumpForce;

        #endregion

        #region Input Handling

        private float _xMovement;
        private float _zMovement;

        private bool _pressedJump;
        private bool _holdingJump;
        private bool _releasedJump;
        private bool _isJumping;

        #endregion

        void Start()
        {
            Init();
        }

        /// <summary>
        /// This function gets called from the <see cref="Player"/> class.
        /// </summary>
        public void HandlePlayer()
        {
            // gather input
            GetInput();

            Move();
            Jump();
        }

        /// <summary>
        /// Initializes the player, this includes its components and data.
        /// </summary>
        private void Init()
        {
            // gather the components
            playerBod = GetComponent<Rigidbody>();
            player = GetComponent<Player>();
            GetComponent<CapsuleCollider>();
            
            // initialize the variables
            _speed = player.PlayerEntity.Speed;
            _drag = player.PlayerEntity.Drag;
            _jumpForce = player.PlayerEntity.JumpForce;
        }

        /// <summary>
        /// Receives the Input, this would also make sure everything would be handled accordingly.
        /// </summary>
        private void GetInput()
        {
            _xMovement = Input.GetAxisRaw("Horizontal");
            _zMovement = Input.GetAxisRaw("Vertical");

            _pressedJump = Input.GetButtonDown("Jump");
            _holdingJump = Input.GetButton("Jump");
            _releasedJump = Input.GetButtonUp("Jump");
        }

        /// <summary>
        /// Makes the player move around.
        /// </summary>
        private void Move()
        {
            // check if input has been given, if so move.
            if (_xMovement != 0 || _zMovement != 0)
            {
                // save the move direction
                Vector3 moveDir = new Vector3(_xMovement, playerBod.velocity.y, _zMovement);

                // rotate player based on direction of input
                float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + TpCamera.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
                    ref turnSmoothVelocity, TurnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                // Move player forward based on cam
                Vector3 direction = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                playerBod.velocity = new Vector3(direction.x * _speed, playerBod.velocity.y, direction.z * _speed);
            }

            // apply a drag
            playerBod.drag = Drag();
        }

        /// <summary>
        /// Makes the player Jump upwards.
        /// </summary>
        private void Jump()
        {
            // check if the player is jumping whilst grounded
            if (_pressedJump && IsGrounded())
            {
                Vector3 jumpDirection = Vector3.up * _jumpForce;
                
                var velocity = playerBod.velocity;
                velocity = new Vector3(velocity.x, jumpDirection.y, velocity.z);
                playerBod.velocity = velocity;

                // set a countdown
                _isJumping = true;
                jumpTimerCountdown = jumpTimer;
            }

            // jump higher upon holding
            if (_holdingJump && _isJumping)
            {
                if (jumpTimerCountdown > 0)
                {
                    Vector3 jumpDirection = Vector3.up * _jumpForce;

                    var velocity = playerBod.velocity;
                    velocity = new Vector3(velocity.x, jumpDirection.y, velocity.z);
                    playerBod.velocity = velocity;

                    jumpTimerCountdown -= Time.deltaTime;
                }
                else
                    _isJumping = false;
            }

            // check if the player isn't jumping anymore
            if (_releasedJump)
                _isJumping = false;

            // ensure the player jump at least higher
            if (jumpTimerCountdown > 0)
                jumpTimerCountdown -= Time.deltaTime;

            // reset the mass back to 1f
            if (IsGrounded())
                playerBod.mass = 1f;
            else // set a new mass of the player
            {
                playerBod.mass = gravityScale;

                // clamp the velocity of the player once they fall
                var velocity = playerBod.velocity;
                playerBod.velocity = new Vector3(velocity.x,
                    Mathf.Clamp(velocity.y, -fallingSpeed, Mathf.Infinity), velocity.z);
            }
        }

        /// <summary>
        /// Makes sure a value of drag will be applied based on if the player is moving and is grounded.
        /// </summary>
        /// <returns>The drag amount for the player.</returns>
        private float Drag() => _xMovement == 0 && _zMovement == 0 && IsGrounded() ? _drag : 0;

        /// <summary>
        /// Checks if the player is grounded by sending a ray downwards to the ground and checks it layer.
        /// </summary>
        /// <returns>Either returns true or false based on if the ray has hit the correct layer or not.</returns>
        private bool IsGrounded()
        {
            // check with a sphere is the player is grounded or not
            var position = GroundChecker.position;

            Collider[] colliders = Physics.OverlapSphere(position, Radius, GroundLayer);

            return colliders.Length > 0;
        }

        void OnDrawGizmos()
        {
            DebugHelper.DrawWireframeSphere(GroundChecker.position, Radius, Color.red);
        }
    }
}