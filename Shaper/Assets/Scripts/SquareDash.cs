using System.Collections;
using UnityEngine;

namespace Player
{
    public class SquareDash : MonoBehaviour
    {
        private Rigidbody2D rb; // Rigidbody of the player
        private SquareMovement movementScript; // Script of the movement of the player
        private TrailRenderer dashTrail; // Trail that will show when dashing

        // Dash
        private float dashCounter = 1; // Quantity of dashes
        private float dashForce = 30f; // Dash power
        private float dashDuration = 0.2f; // Duration of the dash
        public bool isDashing = false; // Condition to check when player is dashing
        

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            movementScript= GetComponent<SquareMovement>();
            dashTrail = GetComponent<TrailRenderer>();
        }

        private void Update()
        {
            // Player will have a dash to spend when he touches the ground
            if (movementScript.IsGrounded() == true) { dashCounter = 1; }

            // Player will enter dash state when he presses E, has 1 dash to spend, and he is moving
            if (Input.GetKeyDown(KeyCode.E) && dashCounter == 1 && rb.velocity.x != 0)
            { DashState(); }
            
            // When player is in dash state and is not on ground, he will execute the dash
            if (isDashing == true && movementScript.IsGrounded() == false) StartCoroutine(DashCoroutine());
            else { isDashing = false; }
        }

        private void DashState()
        {
            isDashing = true; // Dash state activated
            dashCounter = 0; // Removing dash 
            rb.gravityScale = 0f; // Making player float
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Resetting Y velocity
        }

        private IEnumerator DashCoroutine()
        {
            // Resetting velocity
            rb.velocity = Vector2.zero;

            // Increasing velocity and redirecting it by the direction the player is looking
            rb.velocity = Vector2.right * transform.localScale.x * dashForce;

            // Activating trail
            dashTrail.emitting = true;
            yield return new WaitForSeconds(dashDuration); // Dash duration

            // Player leaves the dash state
            isDashing = false;

            // Making player fall faster
            rb.gravityScale = 2f;

            // Resetting X velocity
            rb.velocity = new Vector2(0f, rb.velocity.y);

            // Deactivating trail
            dashTrail.emitting = false;
        }

    }
}