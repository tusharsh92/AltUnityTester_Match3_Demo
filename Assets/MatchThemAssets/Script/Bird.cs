using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour {
	// Value that controls the amount of force applied when clicking
	public float tapForce;
	// A reference to the map class
	//public Map map;
	// Reference to the manager class
	//public GameManager manager;
	// Bool to check if we've died or not
	private bool isDead;
	
	void Update () 
	{
		// Check to see if we're clicking and if we've not already died
		// (don't want to be able to move if we're dead)
		if (Input.GetMouseButtonDown(0) && !isDead && !(Camera.main.WorldToViewportPoint(transform.position).y > 1f))
		{
			// Add our tapForce to our bird's velocity if we do click
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, tapForce);
			// Control the rotation of the bird based on its velocity

			transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(0f,0f,0f), Quaternion.Euler(0f,0f,90f), GetComponent<Rigidbody2D>().velocity.y);
		} else if (GetComponent<Rigidbody2D>().velocity.y < -.05) 
		{
			// Do the same here except only if it is falling
			transform.rotation = Quaternion.RotateTowards(Quaternion.Euler(0f,0f,0f), Quaternion.Euler(0f,0f, -90f), -GetComponent<Rigidbody2D>().velocity.y * 4f);
		}
	}

	// Check to see if we hit ANYTHING that's not a trigger
	/*void OnCollisionEnter2D(Collision2D other)
	{	other.collider.enabled = false;
		// If we have, call the bird's die method
		Die ();
	}*/

	// If we hit a trigger, we know it is a gate trigger
/*	void OnTriggerEnter2D(Collider2D other)
	{
		// In that case, add one to our player's score
		//manager.curScore += 1;
		//map.Generate();
		other.collider2D.enabled = false;

	}*/

	// Method that controls the bird's death
	void Die()
	{
		// Set a boolean of isDead to true so that we can do some checks later
		isDead = true;
		// Make sure we stop the map from moving
		//map.rigidbody2D.velocity = Vector2.zero;
		// Force the Game Over window to pop up and generate our highscore
		//manager.showGameOver = true;
	}
}
