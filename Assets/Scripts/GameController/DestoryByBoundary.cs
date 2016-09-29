using UnityEngine;
using System.Collections;

public class DestoryByBoundary : MonoBehaviour {
	void OnTriggerExit2D(Collider2D col) {
		Destroy(col.gameObject);
	}
}

