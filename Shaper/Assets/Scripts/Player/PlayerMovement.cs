using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        // Mechanics and components
        [Header("Components & IsGrounded")]
        private Rigidbody2D rb; // Rigidbody
        private PlayerInteraction interactionScript; // Script of interaction
        private PlayerWallJump wallJumpScript; // Script of wallJump
        [SerializeField] private LayerMask groundLayer; // Ground layer
        [SerializeField] private Transform groundCheckPosition; // Position where the circle of checking ground collision is
        [SerializeField] private float groundCircleColliderRadius; // ground collision radius
        

        // Responsive Movement
        [Header("Responsive Movement")]
        [SerializeField] private float spdX; // Base Speed
        [SerializeField] private float accel; // Player acceleration
        [SerializeField] private float deccel; // Player decceleration
        [SerializeField] private float velPower; // Velocity that will serve as a powering
        [SerializeField] private float frictionAmount; // Artifical friction value
        private float movement; // Movement input
        private bool facingLeft = true; // Bool condition to check when the player needs to flip
        private float maxHorizontalSpeed = 15f; // Max speed cap

        // Jump
        [Header("Jump")]
        [SerializeField] private float spdY = 17f; // Jump speed
        private float maxJumpSpeed = 18f; // Max jump velocity cap
        private float maxFallSpeed = -15f; // Max fall velocity cap

        // Coyote time (Mechanic to make the player able to jump for a little interval after leaving the ground
        [SerializeField] private float coyoteTime = 0.2f; // Value in seconds when the player last touched the ground
        private float coyoteTimerCounter; // Counter of coyote time

        // Jump cut (Mechanic to make the player precisely jump when "mashing" the space)
        [SerializeField] private float jumpBufferTime = 0.15f; // Jump buffer time value (Jump cut)
        private float jumpBufferCounter; // Counter of the jump buffer
    
        public bool OnGround
        {
            get { return IsGrounded(); }
        }
        

        public float CurrentMovement
        {
            get { return movement; }
            set { movement = value; }
        }

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            interactionScript = GetComponent<PlayerInteraction>();
            wallJumpScript = GetComponent<PlayerWallJump>();
        }


        private void Update()
        {
            // Assigning the "movement" to the horizontal inputs
            movement = Input.GetAxis("Horizontal");
            // Player can only move when he is alive
            if (interactionScript.Dead == false) { PlatformMechanics(); }
            else { movement = 0f; }

        }

        private void FixedUpdate()
        {
            if (!wallJumpScript.IsWallJumpingNow) ResponsiveMovement();

        }

        // Method to make the player move horizontally with responsive and detailed movement
        private void ResponsiveMovement()
        {
            // Calculates the direction of the movement
            float playerSpeed = movement * spdX;

            // Calculates difference between current velocity and desired velocity
            float speedDif = playerSpeed - rb.velocity.x;

            // Changes the acceleration rate when player moves and stops
            float accelRate = Mathf.Abs(playerSpeed) > 0.01f ? accel : deccel;

            // Applying acceleration to the speed difference, which with pow, increases acceleration constantly
            // Mathf.sign serves for redirection
            float finalMovement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

            // Applying this final movement to a right force with a maxValue, only affecting X axis
            rb.AddForce(finalMovement * Vector2.right);

            // Limiting the minimun and maximum velocity of horizontal & vertical movement
            rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -maxHorizontalSpeed, maxHorizontalSpeed), Mathf.Clamp(rb.velocity.y, maxFallSpeed, maxJumpSpeed));

            
            if (movement < 0 && facingLeft) { Flip(); }
            else if (movement > 0 && !facingLeft) { Flip(); }

        }

        // Method to agroup methods
        private void PlatformMechanics()
        {          
            if (wallJumpScript.IsWallJumpingNow == false && wallJumpScript.IsSliding == false) { Jump(); CoyoteJump(); }
            Friction();
           
        }

        // Method that will execute the coyote jump mechanic
        private void CoyoteJump()
        {
            bool jumpKey = Input.GetKeyDown(KeyCode.Space); // Space

            // Activating coyote timer when touching the ground
            if (IsGrounded() == true) { coyoteTimerCounter = coyoteTime; }
            else { coyoteTimerCounter -= Time.deltaTime; } // When not, reduces the cooldown

            // Detecting jump buffer
            if (jumpKey)
            {
                // Resetting buffer counter
                jumpBufferCounter = jumpBufferTime;
            }
            else
            {
                // Reducing buffer counter
                jumpBufferCounter -= Time.deltaTime;
            }

        }

        // Method to implement artifical friction, preventing sliding on surfaces
        private void Friction()
        {
            // If player is in the ground, and his rb.velocity is smaller than 0.01f
            if (IsGrounded() && Mathf.Abs(movement) < 0.01f)
            {
                // Calculates the minimum value between player speed and friction amount
                float amount = Mathf.Min(Mathf.Abs(rb.velocity.x), Mathf.Abs(frictionAmount));

                // This amount will be redirected with the velocity
                amount *= Mathf.Sign(rb.velocity.x);

                // A force will be added to the player, which will be the friction
                rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
            }
        }

        // Method to implement jump and it´s features
        private void Jump()
        {
            // Key for when releasing the jump key
            bool releaseJumpKey = Input.GetKeyUp(KeyCode.Space);

            // Player will only jump when his coyote is enabled and jump buffer enabled          
            if (coyoteTimerCounter > 0 && jumpBufferCounter > 0 && IsGrounded() || IsGrounded() == false && coyoteTimerCounter > 0 && jumpBufferCounter > 0)
            {
                jumpBufferCounter = 0f;
                rb.AddForce(Vector2.up * spdY, ForceMode2D.Impulse);
            }

            // This code will make the player jump higher when holding, and lower when just tapping
            if (releaseJumpKey && rb.velocity.y > 0f)
            {
                // Adding a opposite force to the player
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
                coyoteTimerCounter = 0f; // Resetting coyote timer
            }
         

        }

        // Method to make the player flip to the direction his facing
        private void Flip()
        {
            // When he is facing left, he faces right, and vice-versa
            facingLeft = !facingLeft;
            transform.localScale = new Vector2(transform.localScale.x * -1f, transform.localScale.y); // Changes X local scale
           
        }

        // Method to check if the player is touching the ground
        private bool IsGrounded()
        {
            // Returning to default gravity when player touches the ground
            if (rb.gravityScale > 1) { rb.gravityScale = 1f; }

            // overlapCircle to detect collision precisely
            bool ground = Physics2D.OverlapCircle(transform.position, groundCircleColliderRadius, groundLayer);

            return ground;
        }

    }
}