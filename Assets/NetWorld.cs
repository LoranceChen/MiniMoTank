using UnityEngine;
using System.Collections;
using RSG;
public class NetWorld : NetEntrance {
	public static Promise<NetWorld> s_notWorldFur = new Promise<NetWorld>();

	void Awake() {
		s_notWorldFur.Resolve(this);
		base.Connect ("127.0.0.1", 12553);
		//enter world scene
	}
}
