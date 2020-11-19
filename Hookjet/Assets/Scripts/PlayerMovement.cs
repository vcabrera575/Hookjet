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

    // Pendulum management
    float angularVelocity = 0f;
    float angularAcceleration = 0f;

    // Time Management
    float dashTimer = 0f;

    Collider2D[] groundColliders;
    public bool grounded;
    bool jump;
    bool dash;
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

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundDistance, groundLayer);

        // Fix deadzone for controllers and add normalization
        if (inputs.magnitude < deadZone)
            inputs = Vector2.zero;
        else
            inputs = inputs.normalized * ((inputs.magnitude - deadZone) / (1 - deadZone));

        if (grounded && Input.GetButtonDown("Jump"))
        {
            playerController.AddForce(new Vector2(0f, gameController.jumpHeight * acceleration), ForceMode2D.Impulse);
            grounded = false;
        }
        else
        {
            inputs.y = 0;
        }
    }

    void FixedUpdate()
    {
        float movementSpeed = gameController.playerSpeed * acceleration * Time.fixedDeltaTime;

        Vector3 targetVelocity = new Vector2( inputs.x * movementSpeed, playerController.velocity.y);
        playerController.velocity = targetVelocity;
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
