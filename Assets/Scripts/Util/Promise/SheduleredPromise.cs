using UnityEngine;
using System.Collections;
using RSG;
using UniRx;
using System.Collections.Generic;
using System;

/// <summary>
/// a promise able to schedule action on pool , current thread or Unity MainThread
/// only support on Unity MainThread yet.
/// </summary>
public class ScheduleredPromise<T> : IPromise<T>{//TODOResolve add resolve thread dispatch
	readonly IPromise<T> source;
	readonly IScheduler scheduler;

//	private Dictionary<IScheduler, Action<T>> holderActions = new Dictionary<IScheduler, Action<T>> ();

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
		source.Done((a) => scheduler.Schedule(() => onResolved(a)), (e) => scheduler.Schedule(() => onRejected(e)));
	}
	public void Done (System.Action<T> onResolved)
	{
		scheduler.Schedule (() => {
//			Package.Log("ScheduleredPromise done - ");
			source.Done((a) => scheduler.Schedule(() => onResolved(a)));
		});
	}
	public void Done ()
	{
		source.Done();
	}
	public IPromise<T> Catch (System.Action<System.Exception> onRejected)
	{
		return source.Catch ((e) => scheduler.Schedule(() => onRejected(e)));
	}
	public IPromise<ConvertedT> Then<ConvertedT> (System.Func<T, IPromise<ConvertedT>> onResolved)
	{
		return source.Then<ConvertedT> ((a) => warpOnSchedule<ConvertedT>(() => onResolved(a)));
	}
	public IPromise Then (System.Func<T, IPromise> onResolved)
	{
		return source.Then ((a) => warpOnSchedule_NonGeneric(() => onResolved(a)));
	}
	public IPromise<T> Then (System.Action<T> onResolved)
	{
		return source.Then ((a) => {scheduler.Schedule(() => onResolved(a)); return;});
	}
	public IPromise<ConvertedT> Then<ConvertedT> (System.Func<T, IPromise<ConvertedT>> onResolved, System.Action<System.Exception> onRejected)
	{
		return source.Then<ConvertedT>((a) => warpOnSchedule<ConvertedT>(() => onResolved(a)),
			(e) => scheduler.Schedule(() => onRejected(e)));
	}
	public IPromise Then (System.Func<T, IPromise> onResolved, System.Action<System.Exception> onRejected)
	{
		return source.Then((a) => warpOnSchedule_NonGeneric(() => onResolved(a)),
			(e) => scheduler.Schedule(() => onRejected(e)));
	}
	public IPromise<T> Then (System.Action<T> onResolved, System.Action<System.Exception> onRejected)
	{
		return source.Then((a) => scheduler.Schedule(() => onResolved(a)),
			(e) => scheduler.Schedule(() => onRejected(e)));
	}

	//can't do forward because thread dispatch must do async(which contains Promise as parameter)
	public IPromise<ConvertedT> Then<ConvertedT> (System.Func<T, ConvertedT> transform)
	{
		return this.Then<ConvertedT> ((t) => Promise<ConvertedT>.Resolved(transform(t)));//source.Then<ConvertedT>((a) => warpOnSchedule<ConvertedT>(() => transform(a)));
	}

	//todo
	public IPromise<ConvertedT> Transform<ConvertedT> (System.Func<T, ConvertedT> transform)
	{
		return this.Then<ConvertedT> (transform);//source.Then<ConvertedT>((a) => transform);
	}
	public IPromise<IEnumerable<ConvertedT>> ThenAll<ConvertedT> (Func<T, IEnumerable<IPromise<ConvertedT>>> chain)
	{
		var x = source.Then<IEnumerable<ConvertedT>> (aa => { //so amazing! 
			var chainRst = new Promise<IEnumerable<IPromise<ConvertedT>>>();
			scheduler.Schedule(() => chainRst.Resolve(chain(aa)));
			var y = chainRst.Then<IEnumerable<ConvertedT>>((bb) => Promise<ConvertedT>.All(bb));
			return y;
		});

		return x;
	}

	public IPromise ThenAll (System.Func<T, System.Collections.Generic.IEnumerable<IPromise>> chain)
	{
		return source.Then (aa => {
			var chainRst = new Promise<IEnumerable<IPromise>>();
			scheduler.Schedule(() => chainRst.Resolve(chain(aa)));
			return chainRst.Then((bb) => Promise.All(bb));
		});
	}
	public IPromise<ConvertedT> ThenRace<ConvertedT> (System.Func<T, System.Collections.Generic.IEnumerable<IPromise<ConvertedT>>> chain)
	{
		return source.Then<ConvertedT> (aa => {
			var chainRst = new Promise<IEnumerable<IPromise<ConvertedT>>>();
			scheduler.Schedule(() => chainRst.Resolve(chain(aa)));
			return chainRst.Then<ConvertedT>(bb => Promise<ConvertedT>.Race(bb));
		});
	}

	public IPromise ThenRace (System.Func<T, System.Collections.Generic.IEnumerable<IPromise>> chain)
	{
		return source.Then (aa => {
			var chainRst = new Promise<IEnumerable<IPromise>>();
			scheduler.Schedule(() => chainRst.Resolve(chain(aa)));
			return chainRst.Then(bb => Promise.Race(bb));
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
	public static IPromise<T> SchedulerOn<T>(this IPromise<T> source, IScheduler scheduler) {
		return new ScheduleredPromise<T> (source, scheduler);
	}
}