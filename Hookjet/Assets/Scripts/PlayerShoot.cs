using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject rope;
    public GameController gameController;

    Vector3 mousePosition;
    GameObject newRope;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            // Reset so that the player can continue moving
            gameController.onHook = false;
            gameController.distanceFromHit = 0f;
            gameController.hookshotLocation = new Vector3();

            // Destroy the current rope object
            if (newRope)
            {
                Destroy(newRope);
                gameController.ropeObject = new GameObject();
            }
        }

        if (newRope)
        {
            // Turn the rope to the player
            //newRope.transform.LookAt(transform.position);

            // Make the rope stretch to the player
            float currentDistance = Vector3.Distance(transform.position, gameController.hookshotLocation);
            newRope.transform.localScale = new Vector3(1, 1, currentDistance / 2);

            // Shrink the rope if the player got closer to the hookshot location
            if (currentDistance < gameController.distanceFromHit)
                gameController.distanceFromHit = currentDistance;
        }
    }

    void Shoot()
    {
        // Get where the mouse is at and then create our object there
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        newRope = Instantiate(rope, mousePosition, transform.rotation);

        // Turn rope to the player
        newRope.transform.LookAt(transform.position);

        // Save the information for player movement
        gameController.hookshotLocation = mousePosition;
        gameController.distanceFromHit = Vector3.Distance(transform.position, gameController.hookshotLocation);
        gameController.onHook = true;

        gameController.ropeObject = newRope;
    }

}
