using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    public CharacterController playerController;
    public GameController gameController;

    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    bool grounded;

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        float deadZone = 0.25f; // Controller deadzone.

        Vector3 move = transform.right * x + transform.forward * z;

        // This will fix deadzone and normalization on controllers as well as keyboard.
        if (move.magnitude < deadZone)
            move = Vector3.zero;
        else
            move = move.normalized * ((move.magnitude - deadZone) / (1 - deadZone));

        // Check if the player is on the ground
        if (playerController.isGrounded && velocity.y < 0)
            grounded = true;
        else
            grounded = false;

        // Gravity
        if (grounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Detects if the player is pressing jump key
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            grounded = false;
        }

        velocity.y += gravity * Time.deltaTime;

        playerController.Move((move * gameController.playerSpeed * Time.deltaTime) + (velocity * Time.deltaTime));
    }
}
