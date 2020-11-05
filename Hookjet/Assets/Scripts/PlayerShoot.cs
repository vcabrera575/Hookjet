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

    GameObject newRope;
    Vector3 hitLocation;


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
            gameController.hookshotLocation = new RaycastHit();
            gameController.distanceFromHit = 0f;
            gameController.onHook = false;

            Destroy(newRope);
        }

        if (newRope)
        {
            // Move the rope for the player and move 
            newRope.transform.position = transform.position - new Vector3(0, 0.75f, 0);
            newRope.GetComponent<Transform>().LookAt(hitLocation);

            // Make the rope appear to go point that was hit
            distanceToHit = Vector3.Distance(transform.parent.position, hitLocation);
            newRope.transform.localScale = new Vector3(1, 1, distanceToHit / 4);

            // Shrink the rope if the player somehow got closer to feel smoother
            float currentDistance = Vector3.Distance(transform.parent.position, gameController.hookshotLocation.point);
            if (currentDistance < gameController.distanceFromHit)
                gameController.distanceFromHit = currentDistance;

        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.transform.tag == "Environment")
            {
                // This is where we create things when we hit an environment object
                newRope = Instantiate(rope, transform.position - new Vector3(0, -1, 0), transform.rotation);
                hitLocation = hit.point;

                // Save info for player movement
                gameController.hookshotLocation = hit;
                gameController.distanceFromHit = Vector3.Distance(transform.parent.position, gameController.hookshotLocation.point);
                gameController.onHook = true;
            }
        }

    }

}
