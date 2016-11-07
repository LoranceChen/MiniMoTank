using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Lorance.RxSocket.Session;
using UniRx;
using Lorance.RxSocket;
using System.Threading;
using RSG;
using System.Collections.Generic;
using Lorance.Util.Helper;
using Realtime.Messaging.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Lorance.Util;
using Lorance.Util.Extension;

/// <summary>
/// gameobject handle network
/// </summary>
public class NetEntrance: MonoBehaviour {
	private ClientEntrance client;
	public IPromise<ConnectedSocket> socket;// todo init at construct = new Promise<ConnectedSocket>();
	private IObservable<ConnectedSocket> socketObv;
	private IObservable<CompletedProto> readObv;
	private IObservable<JObject> jsonObv;

	//	private ConcurrentDictionary<string, Action<CompletedProto>> registerRsp = new ConcurrentDictionary<string, Action<CompletedProto>>(); todo
	private ConcurrentDictionary<string, Action<CompletedProto>> registerJsonRsp = new ConcurrentDictionary<string, Action<CompletedProto>>();

	private void AddToRegisterJsonRsp(string key, Action<CompletedProto> action) {
		//		var itor = registerJsonRsp.GetEnumerator();

		registerJsonRsp.TryAdd (key, action);
		//		Debug.Log("after add keyyyyy count - " + registerJsonRsp.Count);
		//
		//		while(itor.MoveNext()){
		//			var curt = itor.Current;
		//			Debug.Log("after add keyyyyy - " + curt.Key);
		//		}

	}
	//
	//	private Option<Action<CompletedProto>> RemoveRegisterJsonRsp(string key) {
	//		var itor = registerJsonRsp.GetEnumerator();
	//
	//		Action<CompletedProto> resultHolder;
	//		registerJsonRsp.TryRemove (key, out resultHolder);
	//
	//		Debug.Log("after remove keyyyyy count - " + registerJsonRsp.Count);
	//
	//		while(itor.MoveNext()){
	//			var curt = itor.Current;
	//			Debug.Log("after remove keyyyyy - " + curt.Key);
	//		}
	//		return Option<Action<CompletedProto>>.Apply (resultHolder);
	//	}
	//
	//	private Option<Action<CompletedProto>> GetRegisterJsonRsp(string key) {
	//		var itor = registerJsonRsp.GetEnumerator();
	//		Debug.Log("keyyyyy count - " + registerJsonRsp.Count);
	//		while(itor.MoveNext()){
	//			var curt = itor.Current;
	//			Debug.Log("keyyyyy - " + curt.Key);
	//		}
	//
	//		Action<CompletedProto> resultHolder;
	//		registerJsonRsp.TryGetValue (key, out resultHolder);
	//		return Option<Action<CompletedProto>>.Apply (resultHolder);
	//	}

	public void Connect(string host, int port) {
		var conn = ConnectToServer (host, port);//10127
		conn.Done (x => Package.Log("Scoket Connected - " + x.ToString()));

		var reader = socketObv.SelectMany(x => {
			return OpenReading();
		});
		var jsonReader = socketObv.SelectMany (x => {
			return JsonStream();
		});

		//force emit stream. because Observable do NOT emit event if nobody Subscribe.
		reader.Subscribe ();
		jsonReader.Subscribe ();

		//give a subscribe to init
		//		reader.Subscribe (x => Package.Log("read form remote - " +  Encoding.UTF8.GetString(x.loaded.Bytes)));
		//		jsonReader.Subscribe (x => Package.Log("read json form remote - " + x.ToString()));
	}


	public void Send<Req>(Req req) {
		var jStr = JObject.FromObject (req).ToString();
		socket.Then(x => x.send(new ByteBuffer(Common.readyData((byte)1, jStr))));
	}

	public IPromise<Rsp> SendWithJsonResult<Rsp>(JObject jobj) {
		//		var xx = socket.Then (x => {
		//			//ready wait result
		//			var taskId = GetTaskId();
		//			var rst = BindJsonTaskValue<Rsp>(taskId);
		//
		//			//send
		//			var json = jobj.ToString();
		//			x.send(new ByteBuffer(Common.readyData((byte)1, json)));
		//
		//			//return result
		//			return rst;
		//		});
		//
		//		return xx;
		return SendWithJsonResult<JObject, Rsp> (jobj);
	}

	public IPromise<Rsp> SendWithJsonResult<Req, Rsp>(Req req) {
		var xx = socket.Then (x => {
			//ready wait result
			var taskId = GetTaskId();
			var rst = BindJsonTaskValue<Rsp>(taskId);

			//send
			var jobj = JObject.FromObject(req);
			//			Debug.Log("jsonnnnnnnnn11111 - " + jobj.ToString());

			//			try{
			jobj.Merge(JObject.FromObject(new TaskIdentity(taskId)), new JsonMergeSettings {
				// union array values together to avoid duplicates
				MergeArrayHandling = MergeArrayHandling.Union
			});
			//			} catch(Exception ex ) {
			//				Debug.Log("jsonnnnnnnnn - error - " + ex);
			//			}

			var jStr = jobj.ToString();

			//			Debug.Log("jsonnnnnnnnn - " + jStr);
			x.send(new ByteBuffer(Common.readyData((byte)1, jStr)));

			//return result
			return rst;
		});

		return xx;
	}

	private string GetTaskId() {
		return System.Diagnostics.Stopwatch.GetTimestamp().ToString() + "-" + Thread.CurrentThread.ManagedThreadId.ToString();
	}

	// Use this for initialization
	// Gate port 12652
	private IPromise<ConnectedSocket> ConnectToServer (string host, int port) {
		Package.s_level = 100;
		client = new ClientEntrance(host, port);
		socket = Helper.Futr2IPromise(client.Connect());

		socket.Done (rst => {
		}, error => Package.Log("Connect fail - " + error.ToString()));//you can recall client.Connect method
		socketObv = Helper.IPomise2Observable (socket);
		return socket;
	}

	private IObservable<CompletedProto> OpenReading() {
		return socketObv.SelectMany (sck => {
			readObv = sck.startReading();
			//todo dispatch registed callback `registerRsp`

			return readObv;
		});
	}

	private IObservable<JObject> JsonStream() {
		jsonObv = readObv.TakeWhile(proto => {
			//			Debug.Log("mmmmmmm - " + registerJsonRsp.Count);
			if(proto.uuid == (byte)1) {
				//callback json register
				var taskIdentity = JsonUtility.FromJson<TaskIdentity>(Encoding.UTF8.GetString(proto.loaded.Bytes));
				//				Debug.Log("dsadsadsada - " + taskIdentity.taskId);
				Option<String>.Apply(taskIdentity.taskId).Foreach(tid => {
					//					Debug.Log("taskIddsadsad - " + taskIdentity.taskId);
					//					Debug.Log("ggggggggggg  - " + tid);
					registerJsonRsp.GetValueEx(tid).Foreach( action => {
						//						Debug.Log("get taskId - " + taskIdentity.taskId);
						action(proto);
					});
				});

				return true;
			} else {
				return false;
			}
		}).Select(proto => {
			//....
			var jstr = Encoding.UTF8.GetString(proto.loaded.Bytes);

			var jsonRst =  JObject.Parse(jstr);
			//			Debug.Log("json - " + jstr);
			return jsonRst;
		});

		//		jsonObv.Subscribe ();
		return jsonObv;
	}

	/// <summary>
	/// common usage for json format.
	/// todo add timeout
	/// </summary>
	/// <returns>The json task value.</returns>
	/// <param name="taskId">Task identifier.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	private IPromise<T> BindJsonTaskValue<T>(string taskId) {
		var promise = new Promise<T>();
		AddToRegisterJsonRsp (taskId, proto => {
			try{
				//				Debug.Log("xxxxx - " + taskId);
				var rsp = Encoding.UTF8.GetString(proto.loaded.Bytes);
				Debug.Log("get task - " + taskId + " rsp - " + rsp);
				//"{"taskId":"14781869997550030-1","states":[{"host":"127.0.0.1","port":12553,"loadLevel":"0","name":"world-name-01"}]}"
				var t = JsonUtility.FromJson<T>(rsp);

				promise.Resolve(t);
				registerJsonRsp.RemoveEx(taskId);
			} catch(Exception ex) {
				promise.Reject(ex);
				registerJsonRsp.RemoveEx(taskId);
			}
		});
		return promise;
	}

	private IObservable<U> BindJsonTaskStream<T, U>(string taskId, Func<IObservable<T>, IObservable<U>> filter) {
		var obv = new Subject<T> ();
		AddToRegisterJsonRsp (taskId, proto => {
			try{
				var t = JsonUtility.FromJson<T>(Encoding.UTF8.GetString(proto.loaded.Bytes));
				obv.OnNext(t);
			} catch(Exception ex) {
				Package.Log(msg: "BindJsonTaskStream - " + ex, logger: new Package.LogError());
			}
		});

		var filted = filter (obv);
		filted.Timeout(System.TimeSpan.FromSeconds (30)).DoOnCompleted (() => registerJsonRsp.RemoveEx(taskId));
		return filted;
	}

	void OnDestory() {
		socket.Done (x => {
			x.Disconnect();
		});
	}
}

public interface JsonDecode {
}

public static class JsonDecodeEx {
	public static void Load(this JsonDecode src, string json) {
		try{
			JsonUtility.FromJsonOverwrite(json, src);
		} catch(System.ArgumentException ex) {
			Package.Log (String.Format("JsonDecode: src is NOT json format - {0} - {1}", json, ex));
		}
	}
}

public interface JsonEncode {
}

public static class JsonEncodeEx {
	public static string ToJson(this JsonEncode src) {
		//todo use JSON.net
		return JsonUtility.ToJson(src);
	}
}

public class TaskIdentity {
	public string taskId;

	public TaskIdentity(){
	}

	public TaskIdentity(string taskId) {
		this.taskId = taskId;
	}
}