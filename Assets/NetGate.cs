using UnityEngine;
using System.Collections;

public class NetGate : NetEntrance {
	public static NetGate s_netGate;
	// Use this for initialization
	void Awake () {
		s_netGate = this;
		base.Connect ("localhost", 12652);
	}
}
