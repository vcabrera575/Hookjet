using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Timer management
    float gameStartTime = 0f;

    // Player management
    public float playerSpeed = 12f;
    public float playerBaseSpeed = 12f;

    public float playerFullness = 0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {


        // Increment timer
        gameStartTime += Time.deltaTime;
    }
}
