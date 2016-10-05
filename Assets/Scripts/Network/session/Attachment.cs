using UnityEngine;
using System.Collections;
using System.IO;
using System.Net.Sockets;

namespace Lorance.RxSocket.Session {
	public class Attachment {
		public ByteBuffer byteBuffer;
		public Socket client;

		public Attachment(ByteBuffer byteBuffer, Socket client) {
			this.byteBuffer = byteBuffer;
			this.client = client;
		}
	}
}