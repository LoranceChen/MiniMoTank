using UnityEngine;
using System.Collections;
using UniRx;
public class ObservableTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		TestTimeout ();

		TestMainThreadDispatch ();
	}

	/// <summary>
	/// Editor should under focus
	/// </summary>
	void TestTimeout() {
		var subj = new Subject<int> ();
		new System.Threading.Thread (() => {
			Debug.Log("thread id - " + System.Threading.Thread.CurrentThread.ManagedThreadId);
			var i = 0;
			while(i < 10) {
				i += 1;
				System.Threading.Thread.Sleep(i*1000 );
				subj.OnNext(i);
			}
		}).Start();


		var timeout = subj.Timeout (System.TimeSpan.FromSeconds (4));
		timeout.Spy().Subscribe (x => Debug.Log("x - " + x), ex => Debug.LogError(ex), () => Debug.Log("completed"));

	}

	/// <summary>
	/// throw exception
	/// always ObserveOnMainThread under MainThread (invoke at MonoBehaviour lifecycle)
	/// </summary>
	void TestMainThreadDispatch() {
		var subj = new Subject<int> ();
		new System.Threading.Thread (() => {
			subj.ObserveOnMainThread().Subscribe(x =>
				Debug.Log("thread id - " + System.Threading.Thread.CurrentThread.ManagedThreadId)
			);
		}).Start();


		var timeout = subj.Timeout (System.TimeSpan.FromSeconds (4));
		timeout.Spy().Subscribe (x => Debug.Log("x - " + x), ex => Debug.LogError(ex), () => Debug.Log("completed"));

	}
}
