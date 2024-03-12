using Assets._Scripts.Entities;
using UnityEngine;

namespace Assets._Scripts.Controllers
{
    /// <summary>
    /// The controller of the player. This class is responsible to handle everything the player wants to do.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerController : MonoBehaviour
    {
        [Tooltip("The playerEntityData is basically the data of a scriptable object that has the stats of the player.")] 
        public Player playerEntityData;

        [Space]
        [SerializeField] private Rigidbody _playerBod;

        [Header("Jumping Properties")] 
        [SerializeField] private float _jumpTimer = .5f;
        private float _jumpTimerCountdown;

        [Header("Gravity Manipulation")]
        [SerializeField] private float _gravityScale = 0f;
        [SerializeField] private float _fallingSpeed = 0f;

        [Header("Ground Check")] 
        public LayerMask groundLayer;
        public Transform groundChecker;
        [Range(0f, 2f)] public float radius = .5f;

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

        void Update()
        {
            // keep track of the data
            KeepTrackOfData();

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
            _playerBod = GetComponent<Rigidbody>();
            GetComponent<CapsuleCollider>();

            _speed = playerEntityData.Speed;
            _drag = playerEntityData.DragAmount;
            _jumpForce = playerEntityData.JumpForce;
        }

        /// <summary>
        /// This function will make sure the data of the SO would get updated accordingly within the <c>Update()</c> method.
        /// </summary>
        private void KeepTrackOfData()
        {
            _speed = playerEntityData.Speed;
            _drag = playerEntityData.DragAmount;
            _jumpForce = playerEntityData.JumpForce;
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
            if (_xMovement != 0 || _zMovement != 0)
                _playerBod.velocity = new Vector3(_xMovement * _speed, _playerBod.velocity.y, _zMovement * _speed);

            _playerBod.drag = Drag();
        }

        /// <summary>
        /// Makes the player Jump upwards.
        /// </summary>
        private void Jump()
        {
            // check if the player is jumping whilst grounded
            if (_pressedJump && IsGrounded())
            {
                // apply a force upwards.
                _playerBod.velocity = Vector3.up * _jumpForce;

                // set a countdown
                _isJumping = true;
                _jumpTimerCountdown = _jumpTimer;
            }

            // jump higher upon holding
            if (_holdingJump && _isJumping)
            {
                if (_jumpTimerCountdown > 0)
                {
                    _playerBod.velocity = new Vector3(_xMovement * _speed, _jumpForce, _zMovement * _speed);
                    _jumpTimerCountdown -= Time.deltaTime;
                }
                else
                    _isJumping = false;
            }

            // check if the player isn't jumping anymore
            if (_releasedJump)
                _isJumping = false;

            // ensure the player jump at least higher
            if (_jumpTimerCountdown > 0)
                _jumpTimerCountdown -= Time.deltaTime;

            // reset the mass back to 1f
            if (IsGrounded())
                _playerBod.mass = 1f;
            else // set a new mass of the player
            {
                _playerBod.mass = _gravityScale;

                // clamp the velocity of the player once they fall
                var velocity = _playerBod.velocity;
                _playerBod.velocity = 
                    new Vector3(velocity.x, Mathf.Clamp(velocity.y, -_fallingSpeed, Mathf.Infinity), velocity.z);
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
            var pos = groundChecker.position;

            Collider[] colliders = Physics.OverlapSphere(pos, radius, groundLayer);

            return colliders.Length > 0;
        }

        void OnDrawGizmos()
        {
            Helper.DrawWireframeSphere(groundChecker.position, radius, Color.red);
        }
    }
}
