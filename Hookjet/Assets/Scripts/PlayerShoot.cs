using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject rope;
    public GameController gameController;

    //float distance = 2f;
    public float speed = 20f;
    public float movementToHit = 0f;
    float distanceToHit = 0f;

    Vector3 mousePosition;
    GameObject newRope;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        else if(Input.GetButtonUp("Fire1"))
        {
            // Reset so player can move again
            gameController.hookshotLocation = new Vector3();
            gameController.distanceFromHit = 0f;
            gameController.onHook = false;

            if (newRope)
                Destroy(newRope);
        }

        if (newRope)
        {
            // Move the rope for the player and move 
            newRope.transform.position = gameController.hookshotLocation;
            // Rope turns to the player
            newRope.transform.LookAt(transform.position);

            // Make the rope stretch towards the player
            distanceToHit = Vector3.Distance(transform.position, gameController.hookshotLocation);
            newRope.transform.localScale = new Vector3(1, 1, distanceToHit / 2);

            // Shrink the rope if the player somehow got closer to feel smoother
            float currentDistance = Vector3.Distance(transform.position, mousePosition);
            if (currentDistance < gameController.distanceFromHit)
                gameController.distanceFromHit = currentDistance;

        }
    }

    void Shoot()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        newRope = Instantiate(rope, mousePosition, transform.rotation);

        // Rope turns to the player
        newRope.transform.LookAt(transform.position);

        // Save info for player movement
        gameController.hookshotLocation = mousePosition;
        gameController.distanceFromHit = Vector3.Distance(transform.position, gameController.hookshotLocation);
        gameController.onHook = true;
    }

}
