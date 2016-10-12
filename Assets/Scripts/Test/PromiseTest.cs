using UnityEngine;
using System.Collections;
using RSG;
using UniRx;
using System.Threading;
using System;

public class PromiseTest : MonoBehaviour
{
	Promise<int> promise = new Promise<int> ();

	// Use this for initialization
	void Start ()
	{
		promise.Done (a => Debug.Log(a));

		//enforce use thread pool
		for (int i = 0; i < 10000; i++) {
			promise.SchedulerOn (Scheduler.DefaultSchedulers.AsyncConversions).Then<string> (value => {
				Debug.Log ("111tid: - " + Thread.CurrentThread.ManagedThreadId);
				return "ThreadId: " + Thread.CurrentThread.ManagedThreadId + " resolved value is - " + value;
			});
		}

		var newP = promise.SchedulerOn (Scheduler.ThreadPool).Then<string> (value => {
			Debug.Log ("222tid: - " + Thread.CurrentThread.ManagedThreadId);
			return String.Format("ThreadId - {0} - should on ThreadPool, Resolved value is - {1}", Thread.CurrentThread.ManagedThreadId, value);
		});

		newP.Done(x => Debug.Log(x));

		var mainTP = newP.SchedulerOn (Scheduler.MainThread).Then<string> (value => {
			return String.Format("ThreadId - {0} - should on Main Thread, Resolved value is - {1}", Thread.CurrentThread.ManagedThreadId, value);
		});

		mainTP.Done(x => Debug.Log(x));
		mainTP.Done(x => Debug.Log(x));

		promise.Resolve (10);
	}
}
