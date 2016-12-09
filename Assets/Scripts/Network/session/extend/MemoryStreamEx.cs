using System;
using System.IO;
using UnityEngine;
using System.Text;

namespace Lorance.RxSocket.Session
{
	public class ByteBuffer
	{
		private byte[] src;
		private int position = 0;
		private int limit;
		private int capacity;
		private int mark = -1;
		public byte[] Bytes{get{return src;}}

		public int Position{
			get{ return position;}
			set{ //difference with java ByteBuffer which return `this`
				if ((value > limit) || (value < 0))
					throw new IllegalArgumentException ();
				position = value;
				if (mark > position) mark = -1;
			}
		}

		public ByteBuffer(byte[] src){
			this.src = src;
			capacity = src.Length;
			position = 0;
			limit = src.Length;
		}

		public ByteBuffer(int length) {
			src = new byte[length];
			capacity = src.Length;
			limit = src.Length;
			position = 0;
		}

		public ByteBuffer Flip()
		{
			limit = position;
			position = 0;
			mark = -1;
			return this;
		}

		public ByteBuffer Clear(){
			position = 0;
			limit = capacity;
			mark = -1;
			return this;
		}

		public int Remaining() {
			return limit - position;
		}

		public byte Get() {
			var nextPos = NextGetIndex ();
//			Debug.LogWarning ("NextPos - " + nextPos);
			return src [nextPos];
		}

		public ByteBuffer Get(byte[] dst, int offset, int length) {
			int end = offset + length;
			for (int i = offset; i < end; i++)
				dst[i] = Get();
			return this;
		}

		public ByteBuffer Put(byte b) {
			src[NextPutIndex()] = b;
			return this;
		}

		public ByteBuffer Put(byte[] bs) {
			for (int i = 0; i < bs.Length; i++)
				Put(bs[i]);
			return this;
		}

		public ByteBuffer Put(ByteBuffer src) {
			int n = src.Remaining();
			for (int i = 0; i < n; i++)
				Put(src.Get());
			return this;
		}

		public int GetInt() {
			var byte4 = new byte[4];
			for (int i = 0; i < 4; i++) {
				byte4 [i] = Get ();
			}
			return Common.ToInt (byte4);
		}

		private int NextGetIndex() {
//			Debug.LogWarning ("NextGetIndex - poistion - " + position + " limit - " + limit);
//			Debug.LogWarning ("NextGetIndex position < limit - " + (position < limit));
			if (position >= limit) {
//				Debug.LogError ("BufferUnderflowException - poistion - " + position + " limit - " + limit);
				throw new BufferUnderflowException ();
			}
			return position++;
		}

		private int NextPutIndex() {                          // package-private
			if (position >= limit)
				throw new BufferOverflowException();
			return position++;
		}

		public override string ToString ()
		{
//			private byte[] src;
//			private int position = 0;
//			private int limit;
//			private int capacity;
//			private int mark = -1;
			StringBuilder bytesStr = new StringBuilder();
			for (var i = 0; i < src.Length; i++) {
				var bt = src [i];
				bytesStr.Append("[" + i + "]" + bt.ToString () + ",");
			}

			return string.Format ("ByteBuffer(src:{0},position:{1},limit:{2},capacity:{3},mark:{4})", bytesStr.ToString(), position, limit, capacity, mark);
		}
	}

	public class RuntimeException : Exception{
		public RuntimeException(string message)
			: base(message)
		{
		}

		public RuntimeException(){}
	}

	public class BufferUnderflowException: RuntimeException{}
	public class BufferOverflowException: RuntimeException{}
	public class IllegalArgumentException:RuntimeException {}
}

