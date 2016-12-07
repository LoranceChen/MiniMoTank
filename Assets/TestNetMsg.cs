using UnityEngine;
using System.Collections;

public class TestNetMsg : NetEntrance {

	// Use this for initialization
	void Start () {
		base.Connect ("localhost", 12652);
	}

}
