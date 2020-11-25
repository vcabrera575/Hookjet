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
    Vector3 startingPos;    // Starting position before begining to pendulum
    Vector3 bob;            // position of pendulum ball
    Vector3 origin;         // position of arm origin
    float length;           // Length of arm
    float angle;            // Pendulum arm angle
    float aVelocity;        // Angle velocity
    float aAcceleration;    // Angle acceleration
    public float dampener = 0.995f; // How much slower should the swing get?
    bool firstSwing = false;        // First time pendulum has started swinging

    public enum hookshotSide { right, left }; // What side of the hookshot are we?
    public hookshotSide side = hookshotSide.left; 

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
        Vector3 tmpPos = transform.position;
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
            // Which side of the hookshot are we on?
            if (transform.position.x < gameController.hookshotLocation.x)
                side = hookshotSide.left;
            else
                side = hookshotSide.right;


            if (!wasOnHook) // Is now on hook, but was not before
            {
                startingPos = transform.position;
                CalculateAngle();
            }

            //playerController.gravityScale = gameController.defaultGravityScale / 4;
            wasOnHook = true;
        }
        else if(wasOnHook) // is NOT on hook but WAS on hook last frame
        {
            wasOnHook = false;

            // Reset player's falling velocity
            Vector3 tmpVelocity = playerController.velocity;
            playerController.velocity = new Vector3(tmpVelocity.x, 0, tmpVelocity.z);
            //playerController.gravityScale = gameController.defaultGravityScale;

            // Reset values relating to the angle
            ResetAngle();
        }


        // Make camera follow player smoothly
        int cameraOffset = 6;
        Vector3 newCameraPos;
        if (gameController.onHook && (playerController.transform.position.x > gameController.hookshotLocation.x))
            newCameraPos = new Vector3(gameController.hookshotLocation.x + cameraOffset, 0, Camera.main.transform.position.z);
        else
            newCameraPos = new Vector3(transform.position.x + cameraOffset, 0, Camera.main.transform.position.z);

        tmpPos.x = tmpPos.x + cameraOffset;
        tmpPos.z = Camera.main.transform.position.z;
        tmpPos.y = 0;
        Camera.main.transform.position = Vector3.Slerp(tmpPos, newCameraPos, 2f * Time.deltaTime);
    }

    void CalculateAngle()
    {
		// OLD EQUATION
        //angle = Mathf.Deg2Rad * Vector3.Angle(Vector3.up, (gameController.hookshotLocation - transform.position) );

        Vector3 vectorFromHookshot = gameController.hookshotLocation - transform.position;
        float x = vectorFromHookshot.x;
        float y = vectorFromHookshot.y;
		
        angle = Mathf.Atan2(y, x);

    }
    void ResetAngle()
    {
        angle = 0f;
        aAcceleration = 1f;
        aVelocity = 0f;
        firstSwing = true;
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
        else // If the player IS on a hook
        {
            Vector3 targetVelocity = new Vector2(0, playerController.velocity.y);
            targetVelocity = new Vector2(movementSpeed, playerController.velocity.y);
            playerController.velocity = Vector3.zero;

            // Calculate where the player will be next
            //transform.position = PendulumPlayer();
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


    Vector3 PendulumPlayer()
    {
        Vector2 bob = transform.position; // Player is the bob
        Vector2 origin = gameController.hookshotLocation; // The spot where the player clicked
        float length = gameController.distanceFromHit;


        //Debug.Log("Starting pos: " + startingPos + "\tNewPos: " + bob + "\tAngle: " + angle + "\tlength: " + length);

        bob.x = origin.x + (length * Mathf.Sin(angle));
        bob.y = origin.y + (-length * Mathf.Cos(angle));

        if (firstSwing)
            bob.x = bob.x;

        aAcceleration = (gravity) * Mathf.Sin(angle);
        angle += aVelocity * dampener * Time.deltaTime * Time.deltaTime;
        aVelocity += aAcceleration * dampener;

        //Debug.Log("Actallly the where we moved: " + bob);
        firstSwing = false;
        return bob;
    }


}
