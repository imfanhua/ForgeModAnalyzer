using System;
using System.IO;

namespace ForgeModAnalyzer.Java {

	public class ByteReader {

		public Stream Stream { get; private set; }
		
		public ByteReader(Stream stream) {
			this.Stream = stream;
		}

		public bool IsByte(int code) {
			return this.Stream.ReadByte() == code;
		}

		public bool IsNotByte(int code) {
			return this.Stream.ReadByte() != code;
		}

		public int ReadByte() {
			return this.Stream.ReadByte();
		}

		public ushort ReadShort() {
			int value1 = this.Stream.ReadByte();
			int value2 = this.Stream.ReadByte();

			if ((value1 | value2) < 0) throw new Exception();
			return (ushort) ((value1 << 8) + (value2 << 0));
		}

		public int ReadInt() {
			int value1 = this.Stream.ReadByte();
			int value2 = this.Stream.ReadByte();
			int value3 = this.Stream.ReadByte();
			int value4 = this.Stream.ReadByte();

			if ((value1 | value2 | value3 | value4) < 0) throw new Exception();
			return ((value1 << 24) + (value2 << 16) + (value3 << 8) + (value4 << 0));
		}

		public byte[] ReadBytes(int length) {
			byte[] bytes = new byte[length];
			this.Stream.Read(bytes, 0, length);
			return bytes;
		}

		public void Skip(int length) {
			this.Stream.Seek(length, SeekOrigin.Current);
		}

	}


}
