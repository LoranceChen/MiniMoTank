using UnityEngine;
using System.Collections;
using RSG;
using UniRx;

/// <summary>
/// a promise able to schedule action on pool , current thread or Unity MainThread
/// only support on Unity MainThread yet.
/// </summary>
public class ScheduleredPromise<T> : IPromise<T> {
	readonly IPromise<T> source;
	readonly IScheduler scheduler;

	public ScheduleredPromise(IPromise<T> source, IScheduler scheduler) {
		this.source = source;
		this.scheduler = scheduler;
	}

	public ScheduleredPromise(IPromise<T> source) {
		this.source = source;
		this.scheduler = Scheduler.DefaultSchedulers.AsyncConversions;
	}

	#region IPromise implementation
	public IPromise<T> WithName (string name)
	{
		return source.WithName (name);
	}
	public void Done (System.Action<T> onResolved, System.Action<System.Exception> onRejected)
	{
		scheduler.Schedule (() => {
			source.Done(onResolved, onRejected);
		});
	}
	public void Done (System.Action<T> onResolved)
	{
		scheduler.Schedule (() => {
			source.Done(onResolved);
		});
	}
	public void Done ()
	{
		scheduler.Schedule (() => {
			source.Done();
		});
	}
	public IPromise<T> Catch (System.Action<System.Exception> onRejected)
	{
		return warpOnSchedule (() => source.Catch (onRejected));
	}
	public IPromise<ConvertedT> Then<ConvertedT> (System.Func<T, IPromise<ConvertedT>> onResolved)
	{
		return warpOnSchedule<ConvertedT> (() => source.Then<ConvertedT> (onResolved));
	}
	public IPromise Then (System.Func<T, IPromise> onResolved)
	{
		return warpOnSchedule_NonGeneric (() => source.Then (onResolved));
	}
	public IPromise<T> Then (System.Action<T> onResolved)
	{
		return warpOnSchedule (() => source.Then (onResolved));
	}
	public IPromise<ConvertedT> Then<ConvertedT> (System.Func<T, IPromise<ConvertedT>> onResolved, System.Action<System.Exception> onRejected)
	{
		return warpOnSchedule<ConvertedT> (() => {
			return source.Then<ConvertedT>(onResolved, onRejected);
		});
	}
	public IPromise Then (System.Func<T, IPromise> onResolved, System.Action<System.Exception> onRejected)
	{
		return warpOnSchedule_NonGeneric (() => {
			return source.Then(onResolved, onRejected);
		});
	}
	public IPromise<T> Then (System.Action<T> onResolved, System.Action<System.Exception> onRejected)
	{
		return warpOnSchedule (() => {
			return source.Then(onResolved, onRejected);
		});
	}
	public IPromise<ConvertedT> Then<ConvertedT> (System.Func<T, ConvertedT> transform)
	{
		return warpOnSchedule<ConvertedT> (() => {
			return source.Then<ConvertedT>(transform);
		});
	}
	public IPromise<ConvertedT> Transform<ConvertedT> (System.Func<T, ConvertedT> transform)
	{
		return warpOnSchedule<ConvertedT> (() => {
			return source.Then<ConvertedT>(transform);
		});
	}
	public IPromise<System.Collections.Generic.IEnumerable<ConvertedT>> ThenAll<ConvertedT> (System.Func<T, System.Collections.Generic.IEnumerable<IPromise<ConvertedT>>> chain)
	{
		return warpCollectionOnSchedule<ConvertedT> (() => {
			return source.ThenAll<ConvertedT>(chain);
		});
	}
	public IPromise ThenAll (System.Func<T, System.Collections.Generic.IEnumerable<IPromise>> chain)
	{
		return warpOnSchedule_NonGeneric (() => {
			return source.ThenAll(chain);
		});
	}
	public IPromise<ConvertedT> ThenRace<ConvertedT> (System.Func<T, System.Collections.Generic.IEnumerable<IPromise<ConvertedT>>> chain)
	{
		return warpOnSchedule<ConvertedT> (() => {
			return source.ThenRace<ConvertedT>(chain);
		});
	}
	public IPromise ThenRace (System.Func<T, System.Collections.Generic.IEnumerable<IPromise>> chain)
	{
		return warpOnSchedule_NonGeneric (() => {
			return source.ThenRace(chain);
		});
	}
	#endregion

	private IPromise warpOnSchedule_NonGeneric (System.Func<IPromise> func) {
		Promise promise = new Promise();
		scheduler.Schedule (() => {
			//func().Done(x => promise.Resolve(x), ex => promise.Reject(ex)); //why can't compiler
			func().Done(promise.Resolve, promise.Reject);
		});

		return promise;
	}

	private IPromise<T> warpOnSchedule (System.Func<IPromise<T>> func) {
		Promise<T> promise = new Promise<T>();
		scheduler.Schedule (() => {
			func().Done(x => promise.Resolve(x), ex => promise.Reject(ex));
		});

		return promise;
	}

	private IPromise<ConvertedT> warpOnSchedule<ConvertedT> (System.Func<IPromise<ConvertedT>> func) {
		Promise<ConvertedT> promise = new Promise<ConvertedT>();
		scheduler.Schedule (() => {
			func().Done(x => {promise.Resolve(x);}, ex => {promise.Reject(ex);});
		});

		return promise;
	}

	private IPromise<System.Collections.Generic.IEnumerable<ConvertedT>> warpCollectionOnSchedule<ConvertedT> (System.Func<IPromise<System.Collections.Generic.IEnumerable<ConvertedT>>> func) {
		Promise<System.Collections.Generic.IEnumerable<ConvertedT>> promise = new Promise<System.Collections.Generic.IEnumerable<ConvertedT>>();
		scheduler.Schedule (() => {
			func().Done(x => promise.Resolve(x), ex => promise.Reject(ex));
		});

		return promise;
	}
}

public static class RSGPromiseEx {
	public static IPromise<T> ShedulerOn<T>(this IPromise<T> source, IScheduler scheduler) {
		return new ScheduleredPromise<T> (source, scheduler);
	}
}