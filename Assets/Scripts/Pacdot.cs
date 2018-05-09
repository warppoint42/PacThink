using UnityEngine;
using System.Collections;

public class Pacdot : MonoBehaviour 
{
	void OnTriggerEnter2D(Collider2D co) 
	{
		if (co.name == "pacman" || co.name == "pacman(Clone)") 
		{
			Destroy (gameObject);
			GameManager.instance.eatFood ();
		}
	}
}