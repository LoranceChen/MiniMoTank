using UnityEngine;
using System.Collections;
using Lorance.Util;
using System.Collections.Generic;
using System.Threading;
using System;

public class Package : MonoBehaviour {
	public static int s_level = 0;
	public static List<string> s_aimLevels = new List<string> (); 

	public static object mylock = new object();
	public static void Log(
		object msg, 
		int level = 0, 
		Option<string> alias = default(None<string>)) 
	{
		Debug.Log ("aaaa");
		lock (mylock) {
			if (level <= s_level || (!alias.IsEmpty () && s_aimLevels.Contains (alias.Get ()))) {

//			string path = UnityVar.inst.dataPath + @"/package_log.txt";
			string line = "Thread Id - " + Thread.CurrentThread.ManagedThreadId + " - " + msg.ToString ();
//			using (System.IO.StreamWriter file = 
//				new System.IO.StreamWriter(path, true))
//			{
//				file.WriteLine(line);
//			}
				Debug.Log (line);
			}
		}
	}

	public static void Log(
		object msg, 
		ILog logger,
		int level = 0, 
		Option<string> alias = default(None<string>)) 
	{
		if (level <= s_level || (!alias.IsEmpty() && s_aimLevels.Contains(alias.Get()))) {
			logger.Log ("Thread Id - " + Thread.CurrentThread.ManagedThreadId + " - " + msg.ToString());
		}
	}


	public interface ILog {
		void Log (object msg);
	}

	public class LogInfo : ILog {
		public void Log(object msg) {
			Debug.Log (msg);
		}

	}

	public class LogError : ILog {
//		public static LogError s_log = new LogError();

		public void Log(object msg) {
			Debug.LogError (msg);
		}

	}
}
