using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public GameObject rope;

    float distance = 2f;
    public float speed = 20f;

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
            Destroy(newRope);
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
                newRope = Instantiate(rope, transform.position, transform.rotation);

                //newRope.transform.rotation = direction;
            }
        }

    }
}
