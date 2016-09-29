using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
//using Functional;

namespace Lorance.Util {
	
	/**
	 * simple future warp system async callback
	 * */
	public class Future<T> {
		private Action<T> callBacks;
//		private HashSet<Action<T>> actionSet = new HashSet<Action<T>>();
		private T value;
		private ExecutionContext ec;

		public Future() {}

		public Future(T t) {
			this.value = t;
		}
//
//		public Future(T t, ExecutionContext ec) {
//			this.value = t;
//			this.ec = ec;
//		}

		public Future(Func<T> cb) {
			completeWith(cb);
		}

		public void onComplete(Action<T> func) {
			lock (this) {
				if (this.value == null) {
//					actionSet.Add (func);
					callBacks += func;
				}
				else {
					func (value);

				}
			}
		}
			
		public void completeWith (Func<T> value) {
			lock (this) {
				this.value = value ();
				if (callBacks != null)
					callBacks (this.value);
			}
		}

		public Future<U> map<U>(Func<T, U> f) {
			Future<U> p = new Future<U> ();
			this.onComplete ((val) => p.completeWith(() => f(val)));//val equals this.value
			return p;
		}

		public Future<U> flatMap<U>(Func<T, Future<U>> f){
			Future<U> p = new Future<U>();
			this.onComplete ((val) => p = f(val));
			return p;
		}

		public static Future<T> Apply(T t) {
			return new Future<T>(() => {return t;});
		}
	}
}
