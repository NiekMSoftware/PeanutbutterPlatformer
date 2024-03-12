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

        [Header("DEBUG")] 
        [SerializeField] private float _maxHealth;
        [SerializeField] private float _health;
        [SerializeField] private float _speed;
        [SerializeField] private float _drag;
        [SerializeField] private float _jumpForce;

        [Header("Jumping Properties")] 
        [SerializeField] private float _jumpTimer;
        private float _jumpTimerCountdown;

        [Header("Gravity Manipulation")]
        [SerializeField] private float _gravityScale;
        [SerializeField] private float _fallingSpeed;

        [Header("Ground Check")] 
        public LayerMask groundLayer;
        public Transform groundChecker;
        public float rayDistance = .2f;

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

            // draw a ray every frame
            Helper.DrawRayDown(groundChecker.position, rayDistance, 0.1f);
        }

        /// <summary>
        /// Initializes the player, this includes its components and data.
        /// </summary>
        private void Init()
        {
            // gather the components
            _playerBod = GetComponent<Rigidbody>();
            GetComponent<CapsuleCollider>();

            // init health
            _maxHealth = playerEntityData.MaxHealth;
            _health = _maxHealth;
            _speed = playerEntityData.Speed;
            _drag = playerEntityData.DragAmount;
            _jumpForce = playerEntityData.JumpForce;
        }

        /// <summary>
        /// This function will make sure the data of the SO would get updated accordingly within the <c>Update()</c> method.
        /// </summary>
        private void KeepTrackOfData()
        {
            playerEntityData.Health = _health;
            playerEntityData.Speed = _speed;
            playerEntityData.DragAmount = _drag;
            playerEntityData.JumpForce = _jumpForce;
        }

        /// <summary>
        /// Receives the Input, this would also make sure everything would be handled accordingly.
        /// </summary>
        private void GetInput()
        {
            if (HasDied()) return;

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
        /// Checks if the player has died.
        /// </summary>
        /// <returns>Returns false if the player health is above zero. It will return true once the player's health reaches zero.</returns>
        private bool HasDied()
        {
            // return false if health is above 0
            if (_health > 0) return false;

            // die and return true
            playerEntityData.Die();
            return true;
        }

        /// <summary>
        /// Makes sure a value of drag will be applied based on if the player is moving and is grounded.
        /// </summary>
        /// <returns>The drag amount for the player.</returns>
        private float Drag() => _xMovement == 0 && _zMovement == 0 && IsGrounded() ? _drag : 0;

        private bool IsGrounded()
        {
            return Physics.Raycast(groundChecker.position, Vector3.down, rayDistance, groundLayer);
        }
    }
}
