using UnityEngine;
using System.Collections;
using RSG;
using UniRx;
public class UnityVar : MonoBehaviour {
	public static Promise<UnityVar> inst = new Promise<UnityVar> ();

	public UnityVar() {
//		inst = new Promise<UnityVar> ();
	}
	public string dataPath;
	void Awake() {
		this.dataPath = Application.dataPath;


		//resolve after variable instanced
		inst.Resolve(this);
		inst.SchedulerOn (Scheduler.MainThread).Done (x => Package.Log("init MainThread scheduler"));
		DontDestroyOnLoad(gameObject);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

}
