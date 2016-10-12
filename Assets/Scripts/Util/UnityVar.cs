using UnityEngine;
using System.Collections;

public class UnityVar : MonoBehaviour {
	public static UnityVar inst;

	public string dataPath;
	void Awake() {
		inst = this;
		this.dataPath = Application.dataPath;

		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
