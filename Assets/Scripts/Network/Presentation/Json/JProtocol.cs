//using UnityEngine;
//using System.Collections;
//using System;
//using System.Text;
//using Lorance.RxSocket.Session;
//using UniRx;
//using Lorance.RxSocket;
//using System.Threading;
//using RSG;
//using System.Collections.Generic;
//using Lorance.Util.Helper;
//using Realtime.Messaging.Internal;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Lorance.Util;
//using Lorance.Util.Extension;
//
///// <summary>
///// JProtocol stream
///// </summary>
//namespace Lorance.RxSocket.Presentation.Json {
//	public class JProtocol {
//		public IPromise<ConnectedSocket> socket{ get; private set;}// todo init at construct = new Promise<ConnectedSocket>();
//
//		private ClientEntrance client;
////		private IObservable<ConnectedSocket> socketObv;
//		private IObservable<CompletedProto> readObv;
////		private IObservable<JObject> jsonObv;
//
//		private ConcurrentDictionary<string, Action<CompletedProto>> registerJsonRsp = new ConcurrentDictionary<string, Action<CompletedProto>>();
//
//		private void AddToRegisterJsonRsp(string key, Action<CompletedProto> action) {
//			registerJsonRsp.TryAdd (key, action);
//		}
//	//
//		public JProtocol(IPromise<ConnectedSocket> connectedSocket, IObservable<CompletedProto> read) {
//			this.socket = connectedSocket;
//			this.readObv = read;
//		}
//
////		public JProtocol(String host, int port) {
////			this.host = host;
////			this.port = port;
////		}
//
//		public IObservable<JObject> JRead() {
//			//var conn = ConnectToServer ("localhost", 12652);//10127
////			var conn = ConnectToServer (host, port);
////			conn.Done (x => Package.Log("Scoket Connected - " + x.ToString()));
//
//			var reader = socketObv.SelectMany(x => {
//				return OpenReading();
//			});
//			var jsonReader = socketObv.SelectMany (x => {
//				return JsonStream();
//			});
//
//			//force emit stream. because Observable do NOT emit event if nobody Subscribe.
//			reader.Subscribe ();
//			jsonReader.Subscribe ();
//			return jsonReader;
//		}
//
//		public void Send<Req>(Req req) {
//			var jStr = JObject.FromObject (req).ToString();
//			socket.Then(x => x.send(new ByteBuffer(Common.readyData((byte)1, jStr))));
//		}
//
//		public IPromise<Rsp> SendWithJsonResult<Rsp>(JObject jobj) {
//			return SendWithJsonResult<JObject, Rsp> (jobj);
//		}
//
//		public IPromise<Rsp> SendWithJsonResult<Req, Rsp>(Req req) {
//			return socket.Then (x => {
//				//ready wait result
//				var taskId = GetTaskId();
//				var rst = BindJsonTaskValue<Rsp>(taskId);
//
//				var jobj = JObject.FromObject(req);
//				jobj.Merge(JObject.FromObject(new TaskIdentity(taskId)), new JsonMergeSettings {
//			    	// union array values together to avoid duplicates
//				    MergeArrayHandling = MergeArrayHandling.Union
//				});
//				var jStr = jobj.ToString();
//
//				x.send(new ByteBuffer(Common.readyData((byte)1, jStr)));
//
//				return rst;
//			});
//		}
//
//		public void Close() {
//			socket.Done (x => {
//				x.Disconnect();
//			});
//		}
//
//		private string GetTaskId() {
//			return System.Diagnostics.Stopwatch.GetTimestamp().ToString() + "-" + Thread.CurrentThread.ManagedThreadId.ToString();
//		}
//
//		// Use this for initialization
//		// Gate port 12652
//		private IPromise<ConnectedSocket> ConnectToServer (string host, int port) {
//			Package.s_level = 100;
//			client = new ClientEntrance(host, port);
//			socket = Helper.Futr2IPromise(client.Connect());
//
//			socket.Done (rst => {
//			}, error => Package.Log("Connect fail - " + error.ToString()));//you can recall client.Connect method
//			socketObv = Helper.IPomise2Observable (socket);
//			return socket;
//		}
//
//		private IObservable<CompletedProto> OpenReading() {
//			return socketObv.SelectMany (sck => {
//				readObv = sck.startReading();
//
//				return readObv;
//			});
//		}
//
//		private IObservable<JObject> JsonStream() {
//			jsonObv = readObv.TakeWhile(proto => {
//				if(proto.uuid == (byte)1) {
//					//callback json register
//					var taskIdentity = JsonUtility.FromJson<TaskIdentity>(Encoding.UTF8.GetString(proto.loaded.Bytes));
//					Option<String>.Apply(taskIdentity.taskId).Foreach(tid => {
//						registerJsonRsp.GetValueEx(tid).Foreach( action => {
//							action(proto);
//						});
//					});
//
//					return true;
//				} else {
//					return false;
//				}
//			}).Select(proto => {
//				var jstr = Encoding.UTF8.GetString(proto.loaded.Bytes);
//
//				var jsonRst =  JObject.Parse(jstr);
//				return jsonRst;
//			});
//
//			return jsonObv;
//		}
//
//		/// <summary>
//		/// common usage for json format.
//		/// todo add timeout
//		/// </summary>
//		/// <returns>The json task value.</returns>
//		/// <param name="taskId">Task identifier.</param>
//		/// <typeparam name="T">The 1st type parameter.</typeparam>
//		private IPromise<T> BindJsonTaskValue<T>(string taskId) {
//			var promise = new Promise<T>();
//			AddToRegisterJsonRsp (taskId, proto => {
//				try{
//					var rsp = Encoding.UTF8.GetString(proto.loaded.Bytes);
//					Debug.Log("get task - " + taskId + " rsp - " + rsp);
//					//"{"taskId":"14781869997550030-1","states":[{"host":"127.0.0.1","port":12553,"loadLevel":"0","name":"world-name-01"}]}"
//					var t = JsonUtility.FromJson<T>(rsp);
//
//					promise.Resolve(t);
//					registerJsonRsp.RemoveEx(taskId);
//				} catch(Exception ex) {
//					promise.Reject(ex);
//					registerJsonRsp.RemoveEx(taskId);
//				}
//			});
//			return promise;
//		}
//
//		private IObservable<U> BindJsonTaskStream<T, U>(string taskId, Func<IObservable<T>, IObservable<U>> filter) {
//			var obv = new Subject<T> ();
//			AddToRegisterJsonRsp (taskId, proto => {
//				try{
//					var t = JsonUtility.FromJson<T>(Encoding.UTF8.GetString(proto.loaded.Bytes));
//					obv.OnNext(t);
//				} catch(Exception ex) {
//					Package.Log(msg: "BindJsonTaskStream - " + ex, logger: new Package.LogError());
//				}
//			});
//
//			var filted = filter (obv);
//			filted.Timeout(System.TimeSpan.FromSeconds (30)).DoOnCompleted (() => registerJsonRsp.RemoveEx(taskId));
//			return filted;
//		}
//	}
//}
//
//public interface JsonDecode {
//}
//
//public static class JsonDecodeEx {
//	public static void Load(this JsonDecode src, string json) {
//		try{
//			JsonUtility.FromJsonOverwrite(json, src);
//		} catch(System.ArgumentException ex) {
//			Package.Log (String.Format("JsonDecode: src is NOT json format - {0} - {1}", json, ex));
//		}
//	}
//}
//
//public interface JsonEncode {
//}
//
//public static class JsonEncodeEx {
//	public static string ToJson(this JsonEncode src) {
//		//todo use JSON.net
//		return JsonUtility.ToJson(src);
//	}
//}
//
//public class TaskIdentity {
//	public string taskId;
//
//	public TaskIdentity(){
//	}
//
//	public TaskIdentity(string taskId) {
//		this.taskId = taskId;
//	}
//}