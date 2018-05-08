using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperGhostClone : MonoBehaviour 
{
	public Transform[] waypoints;
	int cur = 0;
	bool started = false;
	enum Direction {up, down, left, right, error};
	Direction cdirection;
	public AudioClip[] death;
	public AudioSource deathSource;

	public const float baseSpeed = 0.1f;
	public float speed = 0.005f;
	public float aggression = 0;

	private int playerAttention = 0;
	private int playerMeditation = 0;

	Vector2 dest = Vector2.zero;
	GameObject camera;

	void Start()
	{
		camera = GameObject.Find ("Camera");
	}

	void FixedUpdate () 
	{	
		// start with basic waypoint marker walk
		playerAttention = camera.GetComponent<DisplayData>().getattention1();
		playerMeditation = camera.GetComponent<DisplayData> ().getmeditation1 ();

		if (playerAttention>70) 
		{
			speed = 0.1f;
			if (!started) {
				waypointWalk ();
			}
		}
		// ghosts have left start, should check if the connection is made
		if (started) {
			int connection = camera.GetComponent<DisplayData> ().getpoorSignal1 (); 
			if (connection == 200 || connection == -1) {
				//No connection or not engaged
				randomWalk ();
			}
			// if connection is made, start game with movement of player
			else 
			{
				if (playerMeditation > 70)
					speed = 0.5f;
				else if (playerMeditation == 100)
					speed = 0.0f;
				honeWalk ();
			}
		}
	}


	void waypointWalk()
	{
		// Waypoint not reached yet? then move closer
		if (transform.position != waypoints [cur].position) 
		{
			Vector2 p = Vector2.MoveTowards (transform.position,waypoints [cur].position, speed);
			GetComponent<Rigidbody2D> ().MovePosition (p);
		}
		// Waypoint reached, select next one, or change to random movement after waypoints
		else 
		{
			cur = (cur + 1) % waypoints.Length;
			if (cur == 0) 
			{
				started = true;
				dest = transform.position;
				return;
			}
		}

		// Animation
		Vector2 dir = waypoints[cur].position - transform.position;
		setDirection (dir.x, dir.y);
	}

	void randomWalk()
	{
		Direction curr = cdirection;
		Direction n = Direction.error;

		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);


		// Check for new direction if not moving
		if ((Vector2)transform.position == dest) 
		{
			bool uvalid = valid (Vector2.up);
			bool dvalid = valid (-Vector2.up) && valid (-Vector2.up * 2);
			bool rvalid = valid(Vector2.right) && valid(Vector2.right - Vector2.up * 0.5f);
			bool lvalid = valid(-Vector2.right) && valid(-Vector2.right - Vector2.up * 0.5f);

			switch (curr) 
			{
			case Direction.down:
				n = randNew (Direction.down, dvalid, Direction.right, rvalid, Direction.left, lvalid, Direction.up, uvalid);
				break;
			case Direction.up:
				n = randNew (Direction.up, uvalid, Direction.right, rvalid, Direction.left, lvalid, Direction.down, dvalid);
				break;
			case Direction.right:
				n = randNew (Direction.right, rvalid, Direction.up, uvalid, Direction.down, dvalid, Direction.left, lvalid);
				break;
			case Direction.left:
				n = randNew (Direction.left, lvalid, Direction.up, uvalid, Direction.down, dvalid, Direction.right, rvalid);
				break;
			}

			move (n);

		}
	}

	void move(Direction direction)
	{
		switch (direction) 
		{
		case Direction.down:
			dest = (Vector2)transform.position - Vector2.up;
			break;
		case Direction.up:
			dest = (Vector2)transform.position + Vector2.up;
			break;
		case Direction.right:
			dest = (Vector2)transform.position + Vector2.right;
			break;
		case Direction.left:
			dest = (Vector2)transform.position - Vector2.right;
			break;
		}
		// Animation Parameters
		Vector2 dir = dest - (Vector2)transform.position;
		setDirection (dir.x, dir.y);
	}

	void controlWalk()
	{
		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);

		// Check for Input if not moving
		if ((Vector2)transform.position == dest) 
		{
			if (Input.GetKey(KeyCode.UpArrow) && valid(Vector2.up))
				dest = (Vector2)transform.position + Vector2.up;
			if (Input.GetKey(KeyCode.RightArrow) && valid(Vector2.right) && valid(Vector2.right - Vector2.up * 0.5f))
				dest = (Vector2)transform.position + Vector2.right;
			if (Input.GetKey(KeyCode.DownArrow) && valid (-Vector2.up) && valid (-Vector2.up * 2))
				dest = (Vector2)transform.position - Vector2.up;
			if (Input.GetKey(KeyCode.LeftArrow) && valid(-Vector2.right) && valid(-Vector2.right - Vector2.up * 0.5f))
				dest = (Vector2)transform.position - Vector2.right;
		}

		// Animation Parameters
		Vector2 dir = dest - (Vector2)transform.position;
		setDirection (dir.x, dir.y);
	}

	void honeWalk()
	{
		GameObject pac = GameObject.Find ("pacman");
		if (pac == null) 
		{
			randomWalk ();
			return;
		}

		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);

		// Check for new direction if not moving
		if ((Vector2)transform.position == dest) 
		{
			bool uvalid = valid (Vector2.up);
			bool dvalid = valid (-Vector2.up) && valid (-Vector2.up * 2);
			bool rvalid = valid(Vector2.right) && valid(Vector2.right - Vector2.up * 0.5f);
			bool lvalid = valid(-Vector2.right) && valid(-Vector2.right - Vector2.up * 0.5f);

			Vector3 pacpos = pac.transform.position;
			Direction dir1 = Direction.error;
			Direction dir2 = Direction.error;
			if (transform.position.x < pacpos.x && rvalid && cdirection != Direction.left)
				dir1 = Direction.right;
			else if (transform.position.x > pacpos.x && lvalid  && cdirection != Direction.right)
				dir1 = Direction.left;
			if (transform.position.y < pacpos.y && uvalid && cdirection != Direction.down)
				dir2 = Direction.up;
			else if (transform.position.y > pacpos.y && dvalid && cdirection != Direction.up)
				dir2 = Direction.down;

			if (dir1 == Direction.error && dir2 == Direction.error) 
			{
				randomWalk ();
				return;
			} 
			else 
			{
				if (dir1 != Direction.error) 
				{
					move (dir1);
				} 
				else 
				{
					move (dir2);
				}
			}
		}
	}


	void avoidWalk()
	{
		GameObject pac = GameObject.Find ("pacman");
		if (pac == null) 
		{
			randomWalk ();
			return;
		}

		// Move closer to Destination
		Vector2 p = Vector2.MoveTowards(transform.position, dest, speed);
		GetComponent<Rigidbody2D>().MovePosition(p);

		// Check for new direction if not moving
		if ((Vector2)transform.position == dest) 
		{
			bool uvalid = valid (Vector2.up);
			bool dvalid = valid (-Vector2.up) && valid (-Vector2.up * 2);
			bool rvalid = valid(Vector2.right) && valid(Vector2.right - Vector2.up * 0.5f);
			bool lvalid = valid(-Vector2.right) && valid(-Vector2.right - Vector2.up * 0.5f);

			Vector3 pacpos = pac.transform.position;
			Direction dir1 = Direction.error;
			Direction dir2 = Direction.error;
			if (transform.position.x < pacpos.x && lvalid && cdirection != Direction.right)
				dir1 = Direction.left;
			else if (transform.position.x > pacpos.x && rvalid && cdirection != Direction.left)
				dir1 = Direction.right;
			if (transform.position.y < pacpos.y && dvalid && cdirection != Direction.up)
				dir2 = Direction.down;
			else if (transform.position.y > pacpos.y && uvalid && cdirection != Direction.down)
				dir2 = Direction.up;

			if (dir1 == Direction.error && dir2 == Direction.error) 
			{
				randomWalk ();
				return;
			} 
			else 
			{
				if (dir1 != Direction.error) 
				{
					move (dir1);
				} 
				else 
				{
					move (dir2);
				}
			}
		}
	}

	//Store current direction
	void setDirection(float x, float y)
	{
		GetComponent<Animator>().SetFloat("DirX", x);
		GetComponent<Animator>().SetFloat("DirY", y);
		if (x > 0.1)
			cdirection = Direction.right;
		else if (x < -0.1)
			cdirection = Direction.left;
		else if (y > 0.1)
			cdirection = Direction.up;
		else if (y < -0.1)
			cdirection = Direction.down;
	}

	void OnTriggerEnter2D(Collider2D co) 
	{
		if (co.name == "pacman" || co.name == "pacman(Clone)") 
		{
			Destroy (co.gameObject);
			GameManager.instance.LoseLife ();
			playDeathAudio ();
		}
	}

	void playDeathAudio()
	{
		deathSource.clip = death[0];
		deathSource.Play();
	}

	bool valid(Vector2 dir) 
	{
		// Cast Line from 'next to Pac-Man' to 'Pac-Man'
		Vector2 pos = transform.position;
		RaycastHit2D hit = Physics2D.Linecast(pos + dir, pos);
		return (hit.collider.name != "maze");
	}


	bool randomBool()
	{
		return (Random.value > 0.5f);
	}

	bool randomBool(float tprob)
	{
		return (Random.value < tprob);
	}

	Direction randNew(Direction same, bool validsame, Direction dir1, bool valid1, Direction dir2, bool valid2, Direction opp, bool validopp)
	{
		bool r = randomBool ();
		//		Debug.Log (validsame);
		//		Debug.Log (same.ToString ());
		if ((r || (!valid1 && !valid2)) && validsame) 
		{
			return same;
		} 
		else 
		{
			r = randomBool ();
			if (r) 
			{
				Direction temp = dir1;
				bool tempb = valid1;
				dir1 = dir2;
				valid1 = valid2;
				dir2 = temp;
				valid2 = tempb;
			}
			if (valid1) 
			{
				return dir1;
			} 
			else if (valid2) 
			{
				return dir2;
			} 
			else if (validopp) 
			{
				return opp;
			} 
			else if (validsame) 
			{
				return same;
			} 
			else 
			{
				Debug.Log ("no valid dir");
				return Direction.error;
			}
		}
		Debug.Log ("nonono");
		return Direction.error;
	}
}
