using UnityEngine;
using System.Collections;

public class BulletMove : MonoBehaviour {
	public float speed;
	void Update()
	{
		transform.Translate(Vector3.up*speed*Time.deltaTime);
	}
}
