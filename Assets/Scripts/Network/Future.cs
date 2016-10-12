using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using Lorance.Util;
//using Functional;

namespace Lorance.RxSocket {
	
	/**
	 * simple future warp system async callback
	 * todo add a catch error ways and throw error if not catch
	 * */
	public class Future<T> {
		private object cbLock = new object(); //not support concurrent
		private List<Action<T>> callBacks = new List<Action<T>>();
		private List<Action<Exception>> errorCallBacks = new List<Action<Exception>>();
		private T value;
		private Exception error;

		public Future() {}

		public Future(T t) {
			this.value = t;
		}

		public Future(Func<T> cb) {
			completeWith(cb);
		}

		public void onComplete(Action<T> func) {
			onComplete(Some<Action<T>>.Apply(func), None<Action<Exception>>.Apply);
		}

		public void onComplete(Action<Exception> doError) {
			onComplete(None<Action<T>>.Apply, Some<Action<Exception>>.Apply(doError));
		}

		public void onComplete(Action<T> func, Action<Exception> doError) {
			onComplete(Some<Action<T>>.Apply(func), Some<Action<Exception>>.Apply(doError));
		}

		private void onComplete(Option<Action<T>> func, Option<Action<Exception>> doError) {
			lock (cbLock) {
				if (this.value == null && this.error == null) {
					func.Foreach(x => callBacks.Add (x));
					doError.Foreach(x => errorCallBacks.Add (x));
				} else if (this.value != null) {
					func.Foreach(f =>  f(value));
				} else if (this.error != null) {
					doError.Foreach(e =>  e(this.error));
				}
			}
		}

		public void completeWith (Func<T> func) {
			lock (cbLock) {
				if (this.value == null && this.error == null) {
					this.value = func ();
					foreach (Action<T> act in callBacks) {
						act (this.value);
					}
					errorCallBacks.Clear ();
					callBacks.Clear ();
				} else {
					throw new Exception ("Future has completed");
				}
			}
		}

		public void completeWith (Func<Exception> error) {
			lock (cbLock) {
				if (this.value == null && this.error == null) {
					this.error = error ();
					foreach (Action<Exception> doError in errorCallBacks) {
						doError (this.error);
					}
					errorCallBacks.Clear ();
					callBacks.Clear ();
				} else {
					throw new Exception ("Future has completed");
				}
			}
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
