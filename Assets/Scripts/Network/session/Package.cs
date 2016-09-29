using UnityEngine;
using System.Collections;
using Lorance.RxScoket.Util;
using System.Collections.Generic;
using System.Threading;

public class Package : MonoBehaviour {
	public static int s_level = 0;
	public static List<string> s_aimLevels = new List<string> (); 
//	private const Option<string> NoneString = new None<string>();
	public static void Log(object msg, int level = 0, Option<string> alias = default(None<string>)) {
		if (level <= s_level || (!alias.IsEmpty() && s_aimLevels.Contains(alias.Get()))) {
			print ("Thread Id - " + Thread.CurrentThread.ManagedThreadId + " - " + msg.ToString());
		}
	}
}
