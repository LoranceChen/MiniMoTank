﻿using UnityEngine;
using System.Collections.Generic;
using Lorance.Util;
using Lorance.RxSocket;

namespace Lorance.RxSocket.Session{
	class TmpBufferOverLoadException : RuntimeException{
		public TmpBufferOverLoadException(string message)
			: base(message)
		{
		}
		public TmpBufferOverLoadException(){
		}
	}

	public class ReaderDispatch {
		private int maxLength;
		private PaddingProto tmpProto;
		private static None<byte> NoneByte = None<byte>.Apply;
		private static None<CompletedProto> NoneProto = None<CompletedProto>.Apply;
		public ReaderDispatch(PaddingProto tmpProto, int maxLength) {
			this.tmpProto = tmpProto;
			this.maxLength = maxLength;
		}

		public ReaderDispatch(PaddingProto tmpProto) {
			maxLength = Configration.TEMPBUFFER_LIMIT;
		}

		public ReaderDispatch(){
			this.tmpProto = new PaddingProto (None<byte>.Apply, None<BufferedLength>.Apply, new ByteBuffer(new byte[0]));
			maxLength = Configration.TEMPBUFFER_LIMIT;
		}

		public Option<Queue<CompletedProto>> receive(ByteBuffer src){
			src.Flip ();
			Option<Queue<CompletedProto>> rst = receiveHelper (src, None<Queue<CompletedProto>>.Apply);
			src.Clear ();
			return rst;
		}

		private Option<Queue<CompletedProto>> receiveHelper(ByteBuffer src, Option<Queue<CompletedProto>> completes) {
			if (tmpProto.uuidOpt is None<byte>) {//not get uuid
				Option<byte> uuidOpt = tryGetByte (src);
				tmpProto = new PaddingProto (uuidOpt, None<BufferedLength>.Apply, null);
				Option<BufferedLength> lengthOpt = uuidOpt.FlatMap<BufferedLength> ((byte uuid) => {
					return TryGetLength (src, None<BufferedLength>.Apply);
				});

//				Option<CompletedProto> protoOpt = Option<CompletedProto>.Empty;
				Option<CompletedProto> protoOpt = lengthOpt.FlatMap<CompletedProto> ((lengthVal) => {
					if (lengthVal.IsCompleted) {
						int length = lengthVal.Value ();
						if (length > maxLength) {
							Debug.LogError("TmpBufferOverLoadException context - " + tmpProto.ToString());
							//how to handle the msg?
							throw new TmpBufferOverLoadException ("length - " + length);
						} else if (src.Remaining () < length) {
							var newBf = new ByteBuffer (length);
							tmpProto = new PaddingProto (uuidOpt, lengthOpt, newBf.Put (src));
							return None<CompletedProto>.Apply;
						} else {
							tmpProto = new PaddingProto (None<byte>.Apply, None<BufferedLength>.Apply, new ByteBuffer (0));
							var newAf = new byte[length];
							src.Get (newAf, 0, length);
							var completed = new CompletedProto (uuidOpt.Get (), length, new ByteBuffer (newAf));
							return new Some<CompletedProto> (completed);
						}
					} else {
//						var lengthPending = (BufferedLength)lengthVal;
//						byte[] arrived = lengthPending.arrived;
//						int number = lengthPending.arrivedNumber;
						tmpProto = new PaddingProto (uuidOpt, lengthOpt, new ByteBuffer (0));
						return None<CompletedProto>.Apply;
					}
				});

				if (protoOpt is None<CompletedProto>) {
					Debug.LogError ("Not completed yet - " + tmpProto);
					return completes;
				} else {
					var completed = (CompletedProto)protoOpt.Get ();
					Debug.LogError ("Completed Proto - " + completed.ToString() + " load to string - " + completed.loaded.Bytes.GetString());
					Debug.LogError ("Current `src` status - " + src.ToString());

					if (completes.IsEmpty ()) {
						var completedQueue = new Queue<CompletedProto> ();
						completedQueue.Enqueue (completed);
						return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
					} else {
						return receiveHelper (src, completes.Map ((a) => {
							a.Enqueue (completed);
							return a;
						}));
					}
				}
			} else if (!tmpProto.uuidOpt.IsEmpty () && tmpProto.lengthOpt.IsEmpty ()) {//get uuid but not any length data 
				var uuid = tmpProto.uuidOpt.Get ();
				var lengthOpt = TryGetLength (src, None<BufferedLength>.Apply);
				var protoOpt = lengthOpt.FlatMap<CompletedProto> ((lengthValue) => {
					if (lengthValue.IsCompleted) {
//						var length = lengthValue.Value ();
						tmpProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
						return ReadLoad (src, tmpProto);
					} else { //if (tmpProto is PaddingProto) {
						tmpProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
						return NoneProto;
					}
				});

				if (protoOpt is None<CompletedProto>) {
					return completes;
				} else {
					var completed = (CompletedProto)protoOpt.Get ();
					Debug.LogError ("Completed Proto - " + completed.ToString() + " load to string - " + completed.loaded.Bytes.GetString());
					Debug.LogError ("Current `src` status - " + src.ToString());

					if (completes.IsEmpty ()) {
						var completedQueue = new Queue<CompletedProto> ();
						completedQueue.Enqueue (completed);
						return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
					} else {
						return receiveHelper (src, completes.Map ((a) => {
							a.Enqueue (completed);
							return a;
						}));
					}
				}
			} else if (!tmpProto.uuidOpt.IsEmpty () && tmpProto.lengthOpt is Some<BufferedLength>) {//get uuid and get some/all bufferedLength data
				var uuid = tmpProto.uuidOpt.Get ();
				var pending = (BufferedLength)tmpProto.lengthOpt.Get ();
				if (pending.IsCompleted) {
//					var uuid = tmpProto.uuidOpt.Get ();
					var lengthOpt = tmpProto.lengthOpt;
//					var length = lengthOpt.Get().Value ();
					var length = pending.Value ();
					var padding = tmpProto.loading;
					Option<CompletedProto> protoOpt = null;
					if(padding.Position + src.Remaining() < length){
						tmpProto = new PaddingProto (new Some<byte>(uuid), lengthOpt, padding.Put(src));
						protoOpt = NoneProto;
					} else {
						tmpProto = new PaddingProto (None<byte>.Apply, None<BufferedLength>.Apply, new ByteBuffer (0));
						var needLength = length - padding.Position;//10 - 5 = 5
						var newAf = new byte[needLength];
						src.Get (newAf, 0, needLength);

						var completed = new CompletedProto (uuid, length, padding.Put (newAf));
						protoOpt = new Some<CompletedProto> (completed);
					}

					if (protoOpt is None<CompletedProto>) {
						return completes;
					} else {
						var completed = (CompletedProto)protoOpt.Get ();
						Debug.LogError ("Completed Proto - " + completed.ToString() + " load to string - " + completed.loaded.Bytes.GetString());
						Debug.LogError ("Current `src` status - " + src.ToString());

						if (completes.IsEmpty ()) {
							var completedQueue = new Queue<CompletedProto> ();
							completedQueue.Enqueue (completed);
							return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
						} else {
							return receiveHelper (src, completes.Map ((a) => {
								a.Enqueue (completed);
								return a;
							}));
						}
					}
				} else {
					Debug.LogError ("TryGetLength before - " + src.ToString() + " BufferedLength - " + pending.ToString());
					var lengthOpt = TryGetLength (src, new Some<BufferedLength> (pending));
					Debug.LogError ("TryGetLength after - " + src.ToString() + " lengthOpt:Option<BufferedLength> - " + lengthOpt.ToString());

					var protoOpt = lengthOpt.FlatMap<CompletedProto> ((lengthValue) => {
						if (lengthValue.IsCompleted) {
//						var length = lengthValue.Value ();
							var lengthedProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
							return ReadLoad (src, lengthedProto);
						} else {
							tmpProto = new PaddingProto (new Some<byte> (uuid), lengthOpt, new ByteBuffer (0));
							return NoneProto;
						}
					});

					if (protoOpt is None<CompletedProto>) {
						return completes;
					} else {
						var completed = (CompletedProto)protoOpt.Get ();
						Debug.LogError ("Completed Proto - " + completed.ToString() + " load to string - " + completed.loaded.Bytes.GetString());
						Debug.LogError ("Current `src` status - " + src.ToString());

						if (completes.IsEmpty ()) {
							var completedQueue = new Queue<CompletedProto> ();
							completedQueue.Enqueue (completed);
							return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
						} else {
							return receiveHelper (src, completes.Map ((a) => {
								a.Enqueue (completed);
								return a;
							}));
						}
					}
				}
			} else { //IsCompleted Length
				Debug.LogError("can't achieve ReaderDispatch else {");
				throw new System.Exception("can't achieve ReaderDispatch else {");
//				var uuid = tmpProto.uuidOpt.Get ();
//				var lengthOpt = tmpProto.lengthOpt;
//				var length = lengthOpt.Get().Value ();
//				var padding = tmpProto.loading;
//				Option<CompletedProto> protoOpt = null;
//				if(padding.Position + src.Remaining() < length){
//					tmpProto = new PaddingProto (new Some<byte>(uuid), lengthOpt, padding.Put(src));
//					protoOpt = NoneProto;
//				} else {
//					tmpProto = new PaddingProto (None<byte>.Apply, None<BufferedLength>.Apply, new ByteBuffer (0));
//					var needLength = length - padding.Position;
//					var newAf = new byte[needLength];
//					src.Get (newAf, 0, needLength);
//
//					var completed = new CompletedProto (uuid, length, padding.Put (newAf));
//					protoOpt = new Some<CompletedProto> (completed);
//				}
//
//				if (protoOpt is None<CompletedProto>) {
//					return completes;
//				} else {
//					var completed = (CompletedProto)protoOpt.Get ();
//					if (completes.IsEmpty ()) {
//						var completedQueue = new Queue<CompletedProto> ();
//						completedQueue.Enqueue (completed);
//						return receiveHelper (src, new Some<Queue<CompletedProto>> (completedQueue));
//					} else {
//						return receiveHelper (src, completes.Map ((a) => {
//							a.Enqueue (completed);
//							return a;
//						}));
//					}
//				}
			}
//			return new None<Queue<CompletedProto>> ();
		}

		private Option<byte> tryGetByte(ByteBuffer bf) {
			if (bf.Remaining () >= 1) {
//				Debug.LogWarning ("bf.Remaining () - " + bf.Remaining ());
				return new Some<byte> (bf.Get ());
			}
			else 
				return NoneByte;
		}

		private Option<BufferedLength> TryGetLength (ByteBuffer bf, Option<BufferedLength> lengthOpt) {
			var remaining = bf.Remaining ();

			if (lengthOpt is None<BufferedLength>) {
				if (remaining < 1)
					return None<BufferedLength>.Apply;
				else if (1 <= remaining && remaining < 4) {
					byte[] lengthByte = new byte[4];
					bf.Get (lengthByte, 0, remaining);
					return new Some<BufferedLength> (new BufferedLength (lengthByte, remaining));
				} else {
					var length = bf.GetInt ();
					return new Some<BufferedLength> (new BufferedLength (length));
				}
			} else {
				var pendingLength = lengthOpt.Get ();
				int need = 4 - pendingLength.arrivedNumber;
				if (remaining >= need) {
					bf.Get (pendingLength.arrived, pendingLength.arrivedNumber, need);
					return new Some<BufferedLength> (new BufferedLength(Common.ToInt (pendingLength.arrived)));
				} else {
					bf.Get (pendingLength.arrived, pendingLength.arrivedNumber, remaining);
					pendingLength.arrivedNumber += remaining;
					return lengthOpt;
				}
			}
		}

		//require(paddingProto.lengthOpt.isDefined)
		//require(paddingProto.lengthOpt.get.IsCompleted)
		private Option<CompletedProto> ReadLoad(ByteBuffer src, PaddingProto paddingProto){
			int length = paddingProto.lengthOpt.Get ().Value();
			if (length > maxLength)
				throw new TmpBufferOverLoadException ();
			if (src.Remaining () < length) {
				var newBf = new ByteBuffer (length);
				tmpProto = new PaddingProto (paddingProto.uuidOpt, paddingProto.lengthOpt, newBf.Put (src));
				return NoneProto;
			} else {
				tmpProto = new PaddingProto (NoneByte, None<BufferedLength>.Apply, new ByteBuffer(0));
				var newAf = new byte[length];
				Debug.LogError ("ReadLoad src.Get before - " + src.ToString() + " paddingProto - " + paddingProto.ToString());
				src.Get (newAf, 0, length);
				Debug.LogError ("ReadLoad src.Get after - " + src.ToString() + " paddingProto - " + paddingProto.ToString());
				var completed = new CompletedProto (paddingProto.uuidOpt.Get (), length, new ByteBuffer(newAf));
				return new Some<CompletedProto> (completed);
			}
		}
	}
}
