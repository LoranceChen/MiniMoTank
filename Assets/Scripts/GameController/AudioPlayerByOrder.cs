using UnityEngine;
using System.Collections;

public class AudioPlayerByOrder : MonoBehaviour {
	public AudioSource[] audioOrder=new AudioSource[2]; 

	private bool complete;
	// Use this for initialization
	void Start () {
		complete = false;
		audioOrder [0].Play ();
	}
	
	// Update is called once per frame
	void Update () {
		if (!complete) {
			if (!audioOrder [0].isPlaying) {
				audioOrder [1].Play ();
				complete=true;
			}
		}
	}
}
