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
    public float jumpHeight = 3f;
    public float acceleration = 0f; // Acceleration while the player is on a hook
    float playerMass;

    // Velocity
    Vector3 currentVelocity = Vector3.zero;

    // Variables for the ground
    public Transform groundCheck;
    public float groundDistance = 0.4f; // The size the sphere should check for
    public LayerMask groundLayer;

    // Pendulum management
    float angularVelocity = 0f;
    float angularAcceleration = 0f;

    // Time Management
    float dashTimer = 0f;

    Collider2D[] groundColliders;
    bool grounded;
    bool dashCooldown;
    bool wasOnHook;

    void Start()
    {
        dashCooldown = false;
        wasOnHook = false;
        playerController = GetComponent<Rigidbody2D>();
        playerMass = playerController.mass;
    }

    void Update()
    {
        inputs = Vector2.zero;
        inputs.x = Input.GetAxis("Horizontal");
        inputs.y = Input.GetAxis("Vertical");

        float deadZone = 0.025f; // Controller deadzone

        // Fix deadzone for controllers and add normalization
        if (inputs.magnitude < deadZone)
            inputs = Vector2.zero;
        else
            inputs = inputs.normalized * ((inputs.magnitude - deadZone) / (1 - deadZone));

        if (!grounded)
            inputs.y = playerController.velocity.y;
        else
        {
            inputs.y = 0;
            Debug.Log(grounded);
        }
    }

    void FixedUpdate()
    {

        bool wasGrounded = grounded;
        grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] groundColliders = Physics2D.OverlapCircleAll(groundCheck.position, groundDistance, groundLayer);

        for (int i = 0; i < groundColliders.Length; i++)
        {
            if (groundColliders[i].gameObject != gameObject)
            {
                grounded = true;
            }
        }

        float movementSpeed = gameController.playerSpeed * acceleration * Time.fixedDeltaTime;

        // Move the character by finding the target velocity
        Vector3 targetVelocity = new Vector2( inputs.x * movementSpeed, playerController.velocity.y);
        // And then smoothing it out and applying it to the character
        playerController.velocity = targetVelocity;
    }

    void OldUpdate()
    {
        //grounded = Physics.CheckSphere(groundCheck.position, GroundDistance, groundCheck, QueryTriggerInteraction.Ignore); 
        inputs = Vector3.zero;
        inputs.x = Input.GetAxis("Horizontal");
        inputs.y = Input.GetAxis("Vertical");
        float deadZone = 0.25f; // Controller deadzone.

        // This will fix deadzone and normalization on controllers as well as keyboard.
        if (inputs.magnitude < deadZone)
            inputs = Vector2.zero;
        else
            inputs = inputs.normalized * ((inputs.magnitude - deadZone) / (1 - deadZone));

        grounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundLayer);

        if (grounded)
            Debug.Log("test");

        // Player jump
        if (Input.GetButtonDown("Jump") && grounded)
        {
            playerController.AddForce(Vector2.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y));
            grounded = false;
        }

        // Player dashing
        if (Input.GetButtonDown("Dash") && !dashCooldown)
        {
            Vector3 dashVelocity = (gameController.dashDistance * transform.right);
            playerController.AddForce(dashVelocity);
            dashCooldown = true;
            dashTimer = gameController.dashResetTime;
        }

        velocity.y += gravity * Time.deltaTime;
        playerController.AddForce(velocity * Time.deltaTime);

        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;

        if (dashTimer <= 0)
            dashCooldown = false;
        //playerController.MovePosition(velocity * Time.deltaTime);


        // If player is on a hook, and at the furthest distance from the hook, stop moving out of that distance
        if (gameController.onHook)
        {
            acceleration = gameController.hookAcceleration;
            // If player is on a hook, and at the furthest distance from the hook, stop moving out of that distance
            if (gameController.onHook)
            {
                float currentDistance = Vector3.Distance(transform.position, gameController.hookshotLocation.point);
                if (currentDistance > gameController.distanceFromHit) // If player is getting outside the boundry
                {
                    // Reset location just to the edge of the boundry
                    Vector3 distanceFromPoint = transform.position - gameController.hookshotLocation.point;
                    distanceFromPoint *= gameController.distanceFromHit / currentDistance;
                    transform.position = gameController.hookshotLocation.point + distanceFromPoint;
                }
            }

            wasOnHook = true;
        }
        else if (wasOnHook)
        {
            //Vector3 negVelocity = -velocity;
            wasOnHook = false;
            //playerController.Move(negVelocity * Time.deltaTime);
            Vector3 tmpVelocity = playerController.velocity;
            playerController.velocity = new Vector3(tmpVelocity.x, 0, tmpVelocity.z);
        }
        acceleration = 1f;
    }

    void OldFixedUpdate()
    {
        //playerController.MovePosition(playerController.position + inputs * gameController.playerSpeed * Time.fixedDeltaTime);
        float movementSpeed = gameController.playerSpeed * acceleration * Time.deltaTime;

        float distance = inputs.magnitude * Time.fixedDeltaTime;
        //RaycastHit hit;
        //if (playerController.SweepTest(inputs * acceleration * movementSpeed, out hit, distance))
        //    playerController.velocity = new Vector3(0, playerController.velocity.y, 0);
        
        
        playerController.MovePosition(transform.position + (transform.up * inputs.y * movementSpeed) + (transform.right * inputs.x * movementSpeed));

    }

    void PendulumPlayer(Vector3 move)
    {
        Vector3 bob = transform.position; // Player is the bob
        Vector3 origin = gameController.hookshotLocation.point; // The spot where the player hit
        float length = gameController.distanceFromHit;
        
        // Which way are we facing
        Vector3 currentDirection = (transform.position - gameController.hookshotLocation.point).normalized;
        float angle = Vector3.Angle(Vector3.down, currentDirection) * Mathf.Deg2Rad;

        bob.x = origin.x + length * Mathf.Sin(angle);
        bob.z = origin.z + length * Mathf.Cos(angle);

        angularAcceleration = (gravity * playerMass) * Mathf.Sin(angle);


        angle += angularVelocity * 0.995f;
        angularVelocity += angularAcceleration;
    }
}
