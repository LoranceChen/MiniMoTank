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
			} else 
				throw new System.Exception ("length not completed - " + " " + arrivedNumber + " " + length);
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