using UnityEngine;
using System.Collections;
using RSG;
using Lorance.RxSocket;
using UniRx;

namespace Lorance.Util.Helper {
    public static class Helper {
		public static IPromise<T> Futr2IPromise<T>(Future<T> f) {
			var iProm = new Promise<T> ();
			f.onComplete (
				x => iProm.Resolve(x),
				e => iProm.Reject(e)
			);

			return iProm;
		}

//		public static void Futr2IPromise<T>(Future<T> f, IPendingPromise<T> dst) {
//			f.onComplete (x => {
//				dst.Resolve(x);
//			});
//		}

		public static IObservable<T> IPomise2Observable<T>(IPromise<T> p, bool isCold = true) {
			ISubject<T> obv;
			if (isCold)
				obv = new ReplaySubject<T> ();
			else
				obv = new Subject<T> ();
			
			p.Done (x => {
				obv.OnNext(x);
				obv.OnCompleted();
			});

			return obv.AsObservable();
		}
    }
}

