//using UnityEngine;
//using System.Collections.Generic;
//using Lorance.RxSocket.Session;
//using Lorance.RxSocket.Presentation.Json;
//using Lorance.Util.Helper;
//using UniRx;
//
///// <summary>
///// manager network resource
///// </summary>
//public class NetManager : MonoBehaviour {
//	public Dictionary<string, ConnectedSocket> nets = new Dictionary<string, ConnectedSocket>();//new List<ConnectedSocket>();
//	public static NetManager s_netManager;
//	// Use this for initialization
//	void Awake () {
//		s_netManager = this;
//	}
//
//	public JProtocol GateJProto(string host, int port) {
//		var client = new ClientEntrance (host, port);
//		var socket = Helper.Futr2IPromise(client.Connect());
//		var socketObv = Helper.IPomise2Observable (socket);
//		socket.Done(connetedS => nets.Add (connetedS.addressPair.remote.ToString(), connetedS));
//
//		return new JProtocol (socket, socketObv.SelectMany(skt => {
//			skt.startReading();
//		}));
//
//	}
//
//	public ConnectedSocket WorldConnected(string host, int port) {
//		var client = new ClientEntrance (host, port);
//		var socketFur = Helper.Futr2IPromise(client.Connect());
//		var socketObv = Helper.IPomise2Observable (socketFur);
//		socketFur.Done(connetedS => nets.Add (connetedS.addressPair.remote.ToString(), connetedS));
//
//		return new JProtocol (socketFur, socketObv.SelectMany(skt => {
//			return skt.startReading();
//		}));
//	}
//}
