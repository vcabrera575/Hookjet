using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Timer management
    float gameStartTime = 0f;
    public float dashResetTime = 15f;

    // Player management
    public float playerSpeed = 12f;
    public float playerBaseSpeed = 12f;
    public float dashDistance = 5f;
    public float jumpHeight = 5f;

    // Hookshot management
    public Vector3 hookshotLocation; // How far the current hook is from the player
    public float distanceFromHit = 0f;
    public float hookAcceleration = 10f;
    public bool onHook = false;

    // Start is called before the first frame update
    void Start()
    {
        distanceFromHit = 0f;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        // Increment timer
        gameStartTime += Time.deltaTime;
    }
}
