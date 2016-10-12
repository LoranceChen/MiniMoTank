using System.Collections;
using UniRx;
using System;
using System.Net;
using System.Net.Sockets;
using Lorance.Util;

namespace Lorance.RxSocket.Session {
	public class ClientEntrance {
		private string remoteHost;
		private int remotePort;
		public ClientEntrance(string remoteHost, int remotePort) {
			this.remoteHost = remoteHost;
			this.remotePort = remotePort;
		}

		public Future<ConnectedSocket> Connect() {
			IPHostEntry ipHostInfo = Dns.GetHostEntry(remoteHost);
			IPAddress ipAddress = ipHostInfo.AddressList[0];
			IPEndPoint remoteEP = new IPEndPoint(ipAddress, remotePort);

			// Create a TCP/IP socket.
			Socket client = new Socket(AddressFamily.InterNetwork,
				SocketType.Stream, ProtocolType.Tcp);

			var connectFur = new Future<ConnectedSocket> ();

//			IObservable<ConnectedSocket> connectObv = Observable.Create (new Func<IObserver<ConnectedSocket>, IDisposable>())
			// Connect to the remote endpoint.
//			try{
			client.BeginConnect( remoteEP, 
				new AsyncCallback(ConnectCallback), new ConnectObject(connectFur, client));
//			} catch(Exception e) {
//				Package.Log ("dada - " +e.ToString());
//			}
			return connectFur;
		}

		private class ConnectObject {
			public Future<ConnectedSocket> connectFur;
			public Socket client;

			public ConnectObject(
				Future<ConnectedSocket> connectFur,
				Socket client) {
				this.connectFur = connectFur;
				this.client = client;
			}
		}

		private void ConnectCallback(IAsyncResult ar) {
			// Retrieve the socket from the state object.
			ConnectObject connectObject = (ConnectObject) ar.AsyncState;

			try {
				// Complete the connection.
				connectObject.client.EndConnect(ar); // throw at this point if connect fail
				connectObject.connectFur.completeWith(() => new ConnectedSocket(connectObject.client));
				Package.Log(string.Format("Socket connected to {0}",
					connectObject.client.RemoteEndPoint.ToString()));
			} catch (Exception e) {
				connectObject.connectFur.completeWith (() => e);
				Package.Log(string.Format("Socket connected to - {0} : fail - {1}",
					this.remoteHost + ":" + this.remotePort.ToString(), e));
			}
		}
	}
}
