using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public int lives = 3;
	public int foodPoints = 400;
	private int points = 0;
	public Text livesText;
	public Text pointsText;
	public GameObject winner;
	public GameObject loser;
	public GameObject pacmanPrefab;
	public GameObject blinkyPrefab;
	public GameObject blinkyClonePrefab;
	private float resetDelay = 1.0f;

	public static GameManager instance = null;

	private GameObject pacmanClone;
	private GameObject blinky;
	private GameObject blinkyClone;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	void CheckGameOver()
	{
		if (foodPoints < 1) {
			winner.SetActive (true);
			Time.timeScale = .25f;
			Invoke ("Reset", resetDelay);
		}

		if (lives < 1) {
			loser.SetActive (true);
			Time.timeScale = .25f;
			Invoke ("Reset", resetDelay);
		}
	}

	public void LoseLife()
	{
		lives--;
		livesText.text = lives.ToString() + " : Lives";
		Destroy (pacmanClone);
		Invoke ("ResetPacman", resetDelay);
		CheckGameOver();
	}

	void ResetPacman()
	{
		Vector2 newPos = new Vector2 (14, 14);
		pacmanClone = Instantiate (pacmanPrefab, newPos, Quaternion.identity) as GameObject;
	}

	public void eatFood()
	{
		foodPoints--;
		points++;
		pointsText.text = points + " : Points";
		CheckGameOver();
	}
}