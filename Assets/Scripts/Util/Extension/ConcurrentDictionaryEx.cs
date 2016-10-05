using UnityEngine;
using System.Collections;
using System;
using Realtime.Messaging.Internal;
using Lorance.Util;

namespace Lorance.Util.Extension {
	public static class ConcurrentDictionaryEx
	{
//		public static bool AddToRegisterJsonRsp<T, U>(this ConcurrentDictionary<T, U> source, T key, U action) {
//			return source.TryAdd (key, action);
//		}

		public static Option<U> RemoveEx<T, U>(this ConcurrentDictionary<T, U> source, T key) {
			U resultHolder;
			source.TryRemove (key, out resultHolder);
//			var itor = source.GetEnumerator();
//
//			Debug.Log("after remove keyyyyy count - " + source.Count);
//
//			while(itor.MoveNext()){
//				var curt = itor.Current;
//				Debug.Log("after remove keyyyyy - " + curt.Key);
//			}
			return Option<U>.Apply (resultHolder);
		}

		public static Option<U> GetValueEx<T, U>(this ConcurrentDictionary<T, U> source,T key) {

			U resultHolder;
			source.TryGetValue (key, out resultHolder);
//			var itor = source.GetEnumerator();
//			Debug.Log ("after get keyyyyy count - value - " + get);
//			Debug.Log("after get keyyyyy count - " + source.Count);
//
//			while(itor.MoveNext()){
//				var curt = itor.Current;
//				Debug.Log("after get keyyyyy - " + curt.Key);
//			}

			return Option<U>.Apply (resultHolder);
		}
	}

}