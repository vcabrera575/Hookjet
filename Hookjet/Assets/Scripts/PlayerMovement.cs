using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    //public CharacterController playerController;
    Rigidbody playerController;
    public GameController gameController;
    Vector3 inputs;

    public float gravity = -9.81f;
    public float hookGravity = -6.81f;
    public float jumpHeight = 3f;
    public float acceleration = 0f;
    float playerMass;

    // Variables for the ground
    public Transform groundCheck;
    public float groundDistance = 0.4f; // The size the sphere should check for
    public LayerMask groundLayer;

    // Pendulum management
    float angularVelocity = 0f;
    float angularAcceleration = 0f;

    // Time Management
    float dashTimer = 0f;

    bool grounded;
    bool dashCooldown;
    bool wasOnHook;

    void Start()
    {
        dashCooldown = false;
        wasOnHook = false;
        playerController = GetComponent<Rigidbody>();
        playerMass = playerController.mass;
    }

    void Update()
    {
        //grounded = Physics.CheckSphere(groundCheck.position, GroundDistance, groundCheck, QueryTriggerInteraction.Ignore); 
        inputs = Vector3.zero;
        inputs.x = Input.GetAxis("Horizontal");
        inputs.z = Input.GetAxis("Vertical");
        float deadZone = 0.25f; // Controller deadzone.

        // This will fix deadzone and normalization on controllers as well as keyboard.
        if (inputs.magnitude < deadZone)
            inputs = Vector3.zero;
        else
            inputs = inputs.normalized * ((inputs.magnitude - deadZone) / (1 - deadZone));

        grounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundLayer);

        // Player jump
        if (Input.GetButtonDown("Jump") && grounded)
        {
            playerController.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            grounded = false;
        }

        // Player dashing
        if (Input.GetButtonDown("Dash") && !dashCooldown)
        {
            Vector3 dashVelocity = (gameController.dashDistance * transform.forward);
            playerController.AddForce(dashVelocity, ForceMode.VelocityChange);
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

    }

    void FixedUpdate()
    {
        //playerController.MovePosition(playerController.position + inputs * gameController.playerSpeed * Time.fixedDeltaTime);
        float movementSpeed = gameController.playerSpeed * Time.deltaTime;
        playerController.MovePosition(transform.position + (transform.forward * inputs.z * movementSpeed) + (transform.right * inputs.x * movementSpeed));
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
