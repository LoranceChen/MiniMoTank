using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
//using Functional;

namespace Lorance.RxSocket {
	
	/**
	 * simple future warp system async callback
	 * */
	public class Future<T> {
//		private object cbLock = new object(); not support concurrent
		private List<Action<T>> callBacks = new List<Action<T>>();
		private T value;

		public Future() {}

		public Future(T t) {
			this.value = t;
		}

		public Future(Func<T> cb) {
			completeWith(cb);
		}

		public void onComplete(Action<T> func) {
//			lock (cbLock) {
				if (this.value == null) {
					callBacks.Add(func);
				}
				else {
					func (value);
				}
//			}
		}
			
		public void completeWith (Func<T> func) {
//			lock (cbLock) {
				this.value = func ();
				foreach (Action<T> act in callBacks) {
					act (this.value);
				}

				callBacks.Clear ();
//			}
		}

		public Future<U> map<U>(Func<T, U> f) {
			Future<U> p = new Future<U> ();
			this.onComplete ((val) => p.completeWith(() => f(val)));
			return p;
		}

		public Future<U> flatMap<U>(Func<T, Future<U>> f){
			Future<U> p = new Future<U>();
			this.onComplete ((val) => p = f(val));
			return p;
		}

		public static Future<T> Apply(T t) {
			return new Future<T>(t);
		}
	}
}
