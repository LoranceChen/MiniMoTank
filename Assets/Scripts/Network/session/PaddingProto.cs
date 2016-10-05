using System;
using System.IO;
using Lorance.Util;

namespace Lorance.RxSocket.Session
{
	public abstract class BufferedProto{}

	public class PaddingProto : BufferedProto {
		public readonly Option<byte> uuidOpt;
		public readonly Option<BufferedLength> lengthOpt;
		public readonly ByteBuffer loading;

		public PaddingProto (
			Option<byte> uuidOpt, 
			Option<BufferedLength> lengthOpt,
			ByteBuffer loading){
			this.uuidOpt = uuidOpt;
			this.lengthOpt = lengthOpt;
			this.loading = loading;
		}
	}

	public class CompletedProto : BufferedProto {
		public readonly byte uuid;
		public readonly int length;
		public readonly ByteBuffer loaded;

		public CompletedProto (
			byte uuid, 
			int length,
			ByteBuffer loaded){

			this.uuid = uuid;
			this.length = length;
			this.loaded = loaded;
		}

		public override string ToString ()
		{
			return string.Format ("[CompletedProto] uuid:{0}, length:{1}, loaded{2}", uuid, length, loaded);
		}
	}
}
