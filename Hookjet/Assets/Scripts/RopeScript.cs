using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //gameObject.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
    }

}
