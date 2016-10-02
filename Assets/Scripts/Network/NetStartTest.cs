using UnityEngine;
using System.Collections;
using System;
using System.Text;
using Lorance.RxScoket.Session;
using UniRx;
using Lorance.RxScoket;
using Lorance.Util;
using System.Threading;

public class NetStartTest : MonoBehaviour {
	ClientEntrance client;
	IObservable<ConnectedSocket> socket;
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
		socket = client.Connect();
		readObv = socket.SelectMany ((x) => {
			return x.startReading();
		});

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
		socket.Subscribe ((x) => {
			print("connect~");
			x.send(new ByteBuffer(Common.readyData((byte)1, "hi server ~")));
		});
	}
}
