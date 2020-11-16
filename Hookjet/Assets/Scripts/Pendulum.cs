using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

// Author: Eric Eastwood (ericeastwood.com)
//
// Description:
//		Written for this gd.se question: http://gamedev.stackexchange.com/a/75748/16587
//		Simulates/Emulates pendulum motion in code
// 		Works in any 3D direction and with any force/direciton of gravity
//
// Demonstration: https://i.imgur.com/vOQgFMe.gif
//
// Usage: https://i.imgur.com/BM52dbT.png



public class Pendulum : MonoBehaviour
{
	public GameObject Pivot;
	public GameObject Bob;

	Vector3 bob;			// position of pendulum ball
	Vector3 origin;         // position of arm origin
	float length;			// Length of arm
	float angle;			// Pendulum arm angle
	float aVelocity;		// Angle velocity
	float aAcceleration;    // Angle acceleration

	float gravity = 9.81f;

	public float dampener = 0.995f; // How much slower should the swing get?

	void Start()
	{
		bob = Bob.transform.position;
		origin = Pivot.transform.position;
		length = Vector3.Distance(bob, origin);

		angle = Vector3.Angle(Vector3.down, bob - origin) ;

		aVelocity = 0.0f;
		aAcceleration = 0.0f;
	}

	// Function to update position
	void Update()
	{
		// Thanks to The Coding Train
		// https://www.youtube.com/watch?v=9iaEqGOh5WM

		// Also thanks to Good Vibrations with Freeball
		// https://www.youtube.com/watch?v=Qo0IW91tniw
		bob.x = origin.x + length * Mathf.Sin(angle);
		bob.y = origin.y + length * Mathf.Cos(angle);
		bob.z = origin.z + length * Mathf.Sin(angle); 

		aAcceleration = (gravity) * Mathf.Sin(angle);
		angle += aVelocity * dampener * Time.deltaTime * Time.deltaTime;
		aVelocity += aAcceleration * dampener;

		Bob.transform.position = bob;
	}
}
