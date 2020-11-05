using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector3 velocity;
    public CharacterController playerController;
    public GameController gameController;

    public float gravity = -9.81f;
    public float hookGravity = -6.81f;
    public float jumpHeight = 3f;
    public float acceleration = 0f;

    bool grounded;
    bool wasOnHook = false;

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

        // Detects if the player is pressing the jump key
        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            grounded = false;
        }

        //Gravity
        if (grounded && velocity.y < 0 && !gameController.onHook)
        {
            velocity.y = -2f;
        }

        // Effect gravity depending on whether or not the player is on a hook
        if (!gameController.onHook)
        {
            velocity.y += gravity * Time.deltaTime;
            acceleration = 1f; // reset acceleration if the player is not on a hook
        }
        else
        {
            acceleration = gameController.hookAcceleration;
            velocity.y += hookGravity * Time.deltaTime;
        }

        playerController.Move((move * (gameController.playerSpeed + acceleration) * Time.deltaTime) + (velocity * Time.deltaTime));

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

            wasOnHook = true;
        }
        else if (wasOnHook)
        {
            Vector3 negVelocity = -velocity;
            wasOnHook = false;
            playerController.Move(negVelocity * Time.deltaTime);
            velocity.y = 0;
        }
    }
}
