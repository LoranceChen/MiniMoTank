using UnityEngine;
using System.Collections;
using Lorance.Util;
using System.Collections.Generic;
using System.Threading;

public class Package : MonoBehaviour {
	public static int s_level = 0;
	public static List<string> s_aimLevels = new List<string> (); 

	public static void Log(
		object msg, 
		int level = 0, 
		Option<string> alias = default(None<string>)) 
	{
		if (level <= s_level || (!alias.IsEmpty() && s_aimLevels.Contains(alias.Get()))) {
			Debug.Log ("Thread Id - " + Thread.CurrentThread.ManagedThreadId + " - " + msg.ToString());
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
		public static LogInfo s_log = new LogInfo();

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
