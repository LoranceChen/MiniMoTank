using UnityEngine;
using System.Collections;

public class NetWorld : NetEntrance {
	public static NetWorld s_notWorld;

	void Awake() {
		s_notWorld = this;
		base.Connect ("127.0.0.1", 12553);
		//enter world scene
	}
}
