using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    //public CharacterController playerController;
    Rigidbody2D playerController;
    public GameController gameController;
    Vector3 inputs;

    public float gravity = -9.81f;
    public float hookGravity = -6.81f;
    public float acceleration = 0f; // Acceleration while the player is on a hook
    float playerMass;

    // Velocity
    Vector3 currentVelocity = Vector3.zero;

    // Variables for the ground
    public Transform groundCheck;
    public float groundDistance = 0.4f; // The size the sphere should check for
    public LayerMask groundLayer;
    bool firstGroundContact;

    // Pendulum management
    float angularVelocity = 0f;
    float angularAcceleration = 0f;

    // Time Management
    float dashTimer = 0f;
    public bool grounded;
    bool dashCooldown;
    bool wasOnHook;

    void Start()
    {
        dashCooldown = false;
        wasOnHook = false;
        firstGroundContact = false;
        playerController = GetComponent<Rigidbody2D>();
        playerMass = playerController.mass;
    }

    void Update()
    {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundLayer);

        // allow player jumping
        if (grounded)
        {
            firstGroundContact = true;
            if (Input.GetButtonDown("Jump"))
            {
                playerController.AddForce(new Vector2(0f, gameController.jumpHeight * acceleration), ForceMode2D.Impulse);
                grounded = false;
            }
        }


        // If player is on a hook, stop moving out of that distance
        if (gameController.onHook)
        {
            playerController.gravityScale = gameController.defaultGravityScale / 4;
            float currentDistance = Vector3.Distance(transform.position, gameController.hookshotLocation);
            if (currentDistance > gameController.distanceFromHit)
            {
                Vector3 distanceFromPoint = transform.position - gameController.hookshotLocation;
                distanceFromPoint *= gameController.distanceFromHit / currentDistance;
                transform.position = gameController.hookshotLocation + distanceFromPoint;
            }

            wasOnHook = true;
        }
        else if(wasOnHook)
        {
            wasOnHook = false;

            // Reset player's falling velocity
            Vector3 tmpVelocity = playerController.velocity;
            playerController.velocity = new Vector3(tmpVelocity.x, 0, tmpVelocity.z);

            playerController.gravityScale = gameController.defaultGravityScale;
        }

        // Make camera follow player
        Camera.main.transform.position = new Vector3(transform.position.x + 6, 0, Camera.main.transform.position.z);
    }

    void FixedUpdate()
    {
        float movementSpeed = gameController.playerSpeed * acceleration * Time.fixedDeltaTime;

        // TODO: Make this also happen while player is in the air, but make it look like they don't move at the start
        if (!gameController.onHook)
        {
            if (firstGroundContact)
            {
                Vector3 targetVelocity = new Vector2(movementSpeed, playerController.velocity.y);
                playerController.velocity = targetVelocity;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        // If we collide with the jump blue area and the player hasn't already jumped, make them jump
        if (col.gameObject.tag == "JumpTrigger")
        {
            // Don't consider this collision to stop the player from movement
            Physics2D.IgnoreCollision(col.collider, gameObject.GetComponent<Collider2D>());

            if (grounded)
            {
                playerController.AddForce(new Vector2(0f, gameController.jumpHeight * acceleration), ForceMode2D.Impulse);
                grounded = false;
            }
        }
    }


    void PendulumPlayer(Vector3 move)
    {
        Vector3 bob = transform.position; // Player is the bob
        Vector3 origin = gameController.hookshotLocation; // The spot where the player hit
        float length = gameController.distanceFromHit;
        
        // Which way are we facing
        Vector3 currentDirection = (transform.position - gameController.hookshotLocation).normalized;
        float angle = Vector3.Angle(Vector3.down, currentDirection) * Mathf.Deg2Rad;

        bob.x = origin.x + length * Mathf.Sin(angle);
        bob.z = origin.z + length * Mathf.Cos(angle);

        angularAcceleration = (gravity * playerMass) * Mathf.Sin(angle);


        angle += angularVelocity * 0.995f;
        angularVelocity += angularAcceleration;
    }


}
