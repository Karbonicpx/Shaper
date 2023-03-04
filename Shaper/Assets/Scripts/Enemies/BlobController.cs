using System.Collections;
using UnityEngine;

public class BlobController : MonoBehaviour
{
    // Components
    private Rigidbody2D rb; // Rigidbody
    private Collider2D col; // Collider
    private Animator anim; // Animator Controller
    [SerializeField] private LayerMask tilemap; // Tilemap layer that the blob is on

    [Header("Movement Behaviour")]
    [SerializeField] private float spdX; // Horizontal speed
    [SerializeField] private float turnAwayCooldownMin; // Minimum cooldown value that will trigger the turn away method
    [SerializeField] private float turnAwayCooldownMax; // Maximum cooldown value that will trigger the turn away method
    private float turnAwayCounter; // Timer of the turn away behaviour


    [Header("Aggro")]

    // Value that will be checked by the distance between player and blob, and determine when blob activates or not
    [SerializeField] private float aggroRange; 
    private Player.SquareMovement target; // Player target
    [SerializeField] private bool isActive = false; // Active state boolean
    [SerializeField] private float unactivateCooldown; // Value that will determine the minimum time the blob will deactivate
    private float unactivateCounter;



    // Property to check when the enemy is near the player, and if yes, he will be activated
    private bool InRange
    {
        get
        {
            // Returns true when the distance between blob and player is smaller or equal than the aggro range
            return Vector2.Distance(transform.position, target.transform.position) <= aggroRange;
        }
    }
    

    void Awake()
    {
        // Getting rigidbody and collider components
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

        // Finding the player in scene
        target = FindObjectOfType<Player.SquareMovement>();
        turnAwayCounter = turnAwayCooldownMax;
        unactivateCounter = unactivateCooldown;

        // Freezing the blob at the start
        rb.bodyType = RigidbodyType2D.Static;

        // Deactivating collider
        col.enabled = false;
    }

    
    void Update()
    {
        // When he is not underground
        if (isActive == true)
        {
            // Moves by the speed X and horizontal axis
            rb.velocity = Vector2.left * spdX;
            TurnAway(); // Turn away function
        }
        
        AggroBehaviour();
    }


    // Method that makes the blob invert direction once in a while
    private void TurnAway()
    {
        // When counter reaches 0, he will turn left OR right
        if (turnAwayCounter > 0) turnAwayCounter -= Time.deltaTime;
        else
        { 
            spdX *= -1;
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            turnAwayCounter = Random.Range(turnAwayCooldownMin, turnAwayCooldownMax); 

        }     
    }

    // Method that makes the blob inactive OR active depending on his distance between him and the player
    private void AggroBehaviour()
    {
        
        // When blob is near the player:
        if (InRange == true) 
        {
            // He will be activated
            isActive = true;
            anim.SetBool("CanDig", false);
            anim.SetBool("Unactive", false); // Dig --> Undig        
            anim.SetBool("IsActivated", true); // Undig --> Walk
           

            // Collider will be activated & rigidbody will begin to move
            col.enabled = true;
            rb.bodyType = RigidbodyType2D.Dynamic;          
        }

        // If blob is not near the player, he continues inactive
        else 
        { 
            isActive = false; 
            StartCoroutine(Dig()); 
            anim.SetBool("IsActivated", false);
            anim.SetBool("Unactive", true);
        }
    }

    // Method that will make the blob deactivate after a period of time, and not instantly after losing aggro
    private IEnumerator Dig()
    {
        unactivateCounter -= Time.deltaTime;
        yield return new WaitUntil(() => unactivateCounter < 0);
        anim.SetBool("CanDig", true);
        unactivateCounter = unactivateCooldown;
        
    }
}
