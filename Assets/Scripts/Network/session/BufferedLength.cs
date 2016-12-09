using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace Lorance.RxSocket.Session{
	public class BufferedLength{
		public bool IsCompleted;

		public int length;

		public readonly byte[] arrived;
		public int arrivedNumber;

		public BufferedLength(
			byte[] arrived,
			int arrivedNumber) {
			this.arrived = arrived;
			this.arrivedNumber = arrivedNumber;
			this.IsCompleted = false;
		}

		public BufferedLength(int length) {
			this.length = length;
			this.IsCompleted = true;
		}

		public int Value(){
			if (IsCompleted) {
				Package.Log ("length - " + length + " arrivedNumber - " + arrivedNumber);
				return length;
			} else {
				var msg = "length not completed - " + " " + arrivedNumber + " " + length;
				Debug.LogError(msg);
				throw new System.Exception (msg);
			}
		}

		public override string ToString ()
		{
			string arrivedStr = null;
			if (arrived == null) {
				arrivedStr = "";
			} else {
				var bytesStr = new System.Text.StringBuilder ();
				for (var i = 0; i < arrived.Length; i++) {
					var bt = arrived [i];
					bytesStr.Append ("[" + i + "]" + bt.ToString () + ",");
				}
				arrivedStr = bytesStr.ToString ();
			}
			return string.Format ("BufferedLength(isCompleted:{0}, length:{1}, arrived:{2}, arrivedNumber:{3})", IsCompleted, length, arrivedStr, arrivedNumber);
		}

	}

//	public class PendingLength : BufferedLength {
//		public readonly byte[] arrived;
//		public int arrivedNumber;
//
//		public PendingLength(
//			byte[] arrived,
//			int arrivedNumber) {
//			this.arrived = arrived;
//			this.arrivedNumber = arrivedNumber;
//		}
//
//		public override int Value() {
//			throw new System.Exception ("length not completed");
//		}
//	}
//
//	public class CompletedLength : BufferedLength {
//		public readonly int length;
//		public CompletedLength(int length) {
//			this.length = length;
//		}
//
//		public override int Value() {
//			return length;
//		}
//	}
}