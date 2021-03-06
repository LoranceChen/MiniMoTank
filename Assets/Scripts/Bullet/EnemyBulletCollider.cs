﻿using UnityEngine;
using System.Collections;

public class EnemyBulletCollider : MonoBehaviour 
{
	public GameObject explosion;
	private GameController gameController;
	void Start()
	{
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null)
		{
			gameController = gameControllerObject.GetComponent <GameController>();
		}
		if (gameController == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}

	}
	void OnTriggerEnter2D (Collider2D col) 
	{
		if (col.tag == "brick")
		{
			Destroy (col.gameObject);
			OnExplore ();
			Destroy (gameObject);
		} 
		else if (col.tag == "stone") 
		{
			OnExplore ();
			Destroy (gameObject);
		} 
		else if (col.tag == "Player") 
		{
			Destroy (col.gameObject);
			gameController.GameOver ();
			OnExplore ();
			Destroy (gameObject);
		} 
		else if (col.tag == "home") 
		{
			Destroy(col.gameObject);
			gameController.GameOver();
			OnExplore();
			Destroy(gameObject);
		}

	}
	void OnExplore()
	{
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		Instantiate(explosion, transform.position, randomRotation);
	}
}
