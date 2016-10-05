using UnityEngine;
using System.Collections;
using UniRx;
using RSG;

//NOTE: extends can't effect a Type
public static class IObservableEx
{
//	public static IObservable<T> FromFuture<T>(this IObservable<T> obv, IPromise<T> T) {
//		
//	}

	public static IObservable<T> Spy<T>(this IObservable<T> source, string opName = null)
	{
		opName = opName ?? "IObservable";
		Package.Log(opName + ": Observable obtained on Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

		return Observable.Create<T>(obs =>
			{
				Debug.Log(opName + ": Subscribed to on Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);

				try
				{
					var subscription = source
						.Do(x => Debug.Log(string.Format("{0}: OnNext({1}) on Thread: {2}",
							opName,
							x,
							System.Threading.Thread.CurrentThread.ManagedThreadId)),
							ex => Debug.Log(string.Format("{0}: OnError({1}) on Thread: {2}",
								opName,
								ex,
								System.Threading.Thread.CurrentThread.ManagedThreadId)),
							() => Debug.Log(string.Format("{0}: OnCompleted() on Thread: {1}",
								opName,
								System.Threading.Thread.CurrentThread.ManagedThreadId))
						)
						.Subscribe(obs);
					return new CompositeDisposable(
						subscription,
						Disposable.Create(() => Debug.Log(string.Format(
							"{0}: Cleaned up on Thread: {1}",
							opName,
							System.Threading.Thread.CurrentThread.ManagedThreadId))));
				}
				finally
				{
					Debug.Log(string.Format("{0}: Subscription completed.", opName));
				}
			});
	}
}

