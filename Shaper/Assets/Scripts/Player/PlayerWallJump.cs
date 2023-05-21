using Player;
using UnityEngine;
using System.Collections;

public class PlayerWallJump : MonoBehaviour
{
    private Rigidbody2D rb; // Rigidbody
    private PlayerMovement movementScr; // PlayerMovement script that this script will be communicating to

    [Header("Wall Jump")]
    [SerializeField] private LayerMask wallJumpLayer; // Layer that the player can wallSlide/wallJump
    [SerializeField] private float overlapCircleRadius; // Size of the overlap circles
    [SerializeField] private float minSlideSpeed; // Minimum value of the slide speed on the clamp
    [SerializeField] private float slideBaseAccel; // Base acceleration of the player slide
    [SerializeField] private float slideAccelRate; // The incrementation to the base slide acceleration
    [SerializeField] private Vector2 wallJumpingPower; // Force values of the wallJump
    [SerializeField] private float wallJumpingDuration; // Wall jump cooldown in seconds

    private GameObject wallOffsetRight; // OverlapCircle left position
    private GameObject wallOffsetLeft; // OverlapCircle right position

    private bool leftWallCheck; // Boolean that checks if the player is colliding with his leftOffset
    private bool rightWallCheck;// Boolean that checks if the player is colliding with his rightOffset

    private float slideAccel; // Value that will be the current slide acceleration
    private bool canSlide; // Boolean that will check if the player is colliding with a wall
    private bool isWallJumping; // Boolean to check if the player is on the process of wall jump

    private bool CollidingWithWalls
    {
        // Checking if the player is either colliding with the left or the right wall and if he is not wallJumping, just sliding
        get { return leftWallCheck && isWallJumping == false || rightWallCheck && isWallJumping == false; }
    }
    public bool IsSliding
    {
        // Player is sliding if he is not on ground, is colliding with the wall and it´s movement input is different than 0
        get { return !movementScr.OnGround && canSlide && movementScr.CurrentMovement != 0; }

    }

    public bool IsWallJumpingNow
    {
        // Returning isWallJumping boolean to be acessed in other script
        get { return isWallJumping; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        movementScr = GetComponent<PlayerMovement>();
        InstantiateWallChecks();
        slideAccel = slideBaseAccel; // Setting the current acceleration to it´s base value at the start of the game
    }

    
    void Update()
    {
        WallSlide();

        // Player can only walljump when he is sliding and is not already wall jumping
        if (IsSliding && isWallJumping == false) { WallJump(); }

        // Updating the position of the wallchecks to be always near the player
        wallOffsetLeft.transform.position = new Vector2(transform.position.x - 0.55f, transform.position.y);
        wallOffsetRight.transform.position = new Vector2(transform.position.x + 0.55f, transform.position.y);

    }


    // Wall slide when colliding with walls
    private void WallSlide()
    {
        // Booleans to check if the player is colliding with another walls
        rightWallCheck = Physics2D.OverlapCircle(wallOffsetRight.transform.position, overlapCircleRadius, wallJumpLayer);
        leftWallCheck = Physics2D.OverlapCircle(wallOffsetLeft.transform.position, overlapCircleRadius, wallJumpLayer);

        // If player is sliding on wall
        if (CollidingWithWalls) { canSlide = true; }

        else { canSlide = false; } // If not, then false

        // If player is sliding, not wall jumping and not on ground
        if (CollidingWithWalls && IsSliding)
        {
            rb.velocity = Vector2.zero; // Resetting velocity
            slideAccel += slideAccelRate; // Adding acceleration to it´s vertical speed

            // Adding acceleration to the player while he is sliding
            float slidingVel = Mathf.Clamp(rb.velocity.y, -minSlideSpeed, -minSlideSpeed - slideAccel);
            rb.velocity = new Vector2(rb.velocity.x, slidingVel); // Adding acceleration to the velocity

        }
        else { slideAccel = slideBaseAccel; } // When players stops sliding, the acceleration returns to it´s base value
    }


    // Wall jump script
    private void WallJump()
    {
        bool wallJumpKey = Input.GetKeyDown(KeyCode.Space);

        if (wallJumpKey) // If player jumps while is sliding
        {
            isWallJumping = true;
            slideAccel = slideBaseAccel; // Resetting acceleration
            rb.velocity = Vector2.zero;
      
            // Adding a X force opposed to the direction he is facing and Y force to be the jump

            rb.velocity = new Vector2(WallJumpDirection() * wallJumpingPower.x, wallJumpingPower.y);
            // To prevent the player mashing the jump button, we have a wall jump cooldown system
            StartCoroutine(ResetWallJump());
        }
    }


    // Method that will define which direction the player will jump based on the wall checks
    private float WallJumpDirection()
    {
        float rightDir = 1f;
        float leftDir = -1f;

        if (leftWallCheck) { return rightDir; }
        else { return leftDir; }
    }

    // Method that will be called to serve as the wallJump cooldown 
    private IEnumerator ResetWallJump()
    {
        // Disabling wallJump at X seconds
        yield return new WaitForSeconds(wallJumpingDuration);
        isWallJumping = false;
    }

    // Method that will create the wall checks, since they change positions if they´re children of the player, and we want them fixed
    private void InstantiateWallChecks()
    {
        wallOffsetLeft = new GameObject();
        wallOffsetRight = new GameObject();

        wallOffsetLeft.name = "RightWall";
        wallOffsetRight.name = "LeftWall";

        wallOffsetLeft.transform.SetParent(transform, false);
        wallOffsetRight.transform.SetParent(transform, false);
    }
}
