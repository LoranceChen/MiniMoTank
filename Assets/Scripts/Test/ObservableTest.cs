using UnityEngine;
using System.Collections;
using UniRx;
public class ObservableTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Debug.Log ("info - " + default(Package.LogInfo));
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

}
