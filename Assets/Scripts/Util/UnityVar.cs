﻿using UnityEngine;
using System.Collections;
using RSG;
using UniRx;
using Lorance.Util;

public class UnityVar : MonoBehaviour {
	public static Promise<UnityVar> inst = new Promise<UnityVar> ();
	public string dataPath;
	void Awake() {
		this.dataPath = "./";

		//resolve after variable instanced
		inst.Resolve(this);
		DontDestroyOnLoad(gameObject);
	}

}
