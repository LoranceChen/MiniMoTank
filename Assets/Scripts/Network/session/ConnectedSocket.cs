using System;
using System.Net.Sockets;
using System.Collections.Generic;
using UniRx;
using System.IO;
using System.Text;
using Lorance.RxSocket;
using System.Threading;
using System.Net;

namespace Lorance.RxSocket.Session {
	/// <summary>
	/// Warpped socket stream by CompletedProto data
	/// </summary>
	public class ConnectedSocket {
		public AddressPair addressPair{ get; private set;}

		public Socket socket;//{ get; private set;}
		private ISubject<CompletedProto> completedProtosSubj;// = new Subject<CompletedProto>();
		private Attachment readAttach;
		private ReaderDispatch readerDispatch = new ReaderDispatch();
		private Semaphore sendDone = new Semaphore(1, 1);

		public ConnectedSocket (Socket socket) {
			this.socket = socket;
			addressPair = new AddressPair((IPEndPoint)socket.LocalEndPoint, (IPEndPoint)socket.RemoteEndPoint);
			readAttach = new Attachment (new ByteBuffer(new byte[Configration.READBUFFER_LIMIT]), socket);
			completedProtosSubj = new Subject<CompletedProto> ();
		}
			
		public void Disconnect () {
			socket.Shutdown (SocketShutdown.Both);
			socket.Disconnect (false); // or .Close()
		}

		public IObservable<CompletedProto> startReading() {
			Package.Log ("beginReading - ");
			beginReadLoop ();
			return completedProtosSubj.AsObservable();
		}


		public void send(ByteBuffer data) {
			send (data.Bytes);
		}

		public void send(byte[] data) {
			sendDone.WaitOne();
			socket.BeginSend(data, 0, data.Length, SocketFlags.None,
				new AsyncCallback((ar) => {
					try {
						// Retrieve the socket from the state object.
						Socket skt = (Socket) ar.AsyncState;

						// Complete sending the data to the remote device.
						int bytesSent = skt.EndSend(ar);
						Package.Log("Sent " + bytesSent + " bytes.", 70);

						// Signal that all bytes have been sent.
						sendDone.Release();
					} catch (Exception e) {
						Package.Log("send data fail - " + e.ToString());
					}
				}), socket);
		}

		private void beginReadLoop() {
			read(readAttach).onComplete((ach) => {
				var protosOpt = readerDispatch.receive(ach.byteBuffer);
				protosOpt.Foreach( (protos) => {
					foreach(CompletedProto proto in protos) {
						completedProtosSubj.OnNext(proto);
					}
				});
				beginReadLoop();
			});
		}

		private Future<Attachment> read(Attachment readAttach) {
			Future<Attachment> f = new Future<Attachment> ();
			socket.BeginReceive (readAttach.byteBuffer.Bytes, 0, readAttach.byteBuffer.Bytes.Length, 0,
				ar => { try {
						Attachment state = (Attachment) ar.AsyncState;
						Socket client = state.client;
						// Read data from the remote device.
						int bytesRead = client.EndReceive(ar);
						if (bytesRead > 0) {
							readAttach.byteBuffer.Position = bytesRead;
							f.completeWith(() => readAttach);
						} else {
							Disconnect();
							Package.Log("[Socket Disconnected] read error, result - " + bytesRead);
						}
					} catch (Exception e) {
						Package.Log(e.ToString());
					}
				},
				readAttach);
			return f;
		}

		public override string ToString ()
		{
			return string.Format (socket.LocalEndPoint.ToString() + " -> "+ socket.RemoteEndPoint.ToString());
		}
	}

	public class AddressPair {
		public IPEndPoint local{ get; private set;}
		public IPEndPoint remote{ get; private set;}

		public AddressPair(IPEndPoint local, IPEndPoint remote){
			this.local = local;

			this.remote = remote;
		}
	}
}
