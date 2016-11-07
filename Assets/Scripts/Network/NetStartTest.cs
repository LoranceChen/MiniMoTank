using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Lorance.RxSocket.Session;
using UniRx;
using Lorance.RxSocket;
using Lorance.Util;
using System.Threading;
using Lorance.Util.Helper;
using RSG;

public class NetStartTest : MonoBehaviour {
	ClientEntrance client;
	IPromise<ConnectedSocket> socket;
	IObservable<ConnectedSocket> socketObv;
	IObservable<CompletedProto> readObv;

	void Start() {
		Package.Log (Thread.CurrentThread.ManagedThreadId);
		ConnectToServer ("localhost", 10127);
	}

	// Use this for initialization
	// Gate 12652
	void ConnectToServer (string host, int port) {
		Package.s_level = 100;
		client = new ClientEntrance(host, port);
		socket = Helper.Futr2IPromise(client.Connect());
		socketObv = Helper.IPomise2Observable (socket);
		readObv = socketObv.SelectMany ((x) => {
			return x.startReading();
		});
		readObv.SubscribeOnMainThread ();
		//for debug
		readObv.Subscribe ((x) => {
			Package.Log("completed proto - " + x.uuid + ";" + x.length + ";" + x.loaded.Bytes.GetString());
		});
			
		readObv.First ((x) => {
			Package.Log ("get first - ");
			return (x.uuid == (byte)2);
		}).ObserveOnMainThread().Subscribe (x => {
			Package.Log ("do transform - ");
			transform.LookAt(new Vector3(1f, 0f, 0f));
		});
		//test send msg
		socketObv.Subscribe ((x) => {
			print("connect~");
			x.send(new ByteBuffer(Common.readyData((byte)1, "hi server ~")));
		});
	}
}
