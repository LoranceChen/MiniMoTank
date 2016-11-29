using UnityEngine;
using System.Collections;
using RSG;

public class NetGate : NetEntrance {
	public static Promise<NetGate> s_netGate = new Promise<NetGate>();
	// Use this for initialization
	void Start () {
		s_netGate.Resolve(this);
		base.Connect ("localhost", 12652);
	}
}
