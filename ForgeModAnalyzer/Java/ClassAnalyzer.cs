using System;
using System.IO;

namespace ForgeModAnalyzer.Java {

	public sealed class ClassAnalyzer {
		
		public static ClassAnalyzer Read(Stream stream) {
			ClassAnalyzer analyzer = new ClassAnalyzer(stream);

			try {
				analyzer.Read();
				return analyzer;
			} catch {
				return null;
			}
		}

		public ByteReader Reader { get; private set; }

		public JavaConstant[] Constants { get; private set; }
		public JavaAttribute[] Attributes { get; private set; }
		
		private ClassAnalyzer(Stream stream) {
			this.Reader = new ByteReader(stream);
		}
		
		private bool VerifyMagicCode() {
			// 202 CA
			if (this.Reader.IsNotByte(202)) return false;
			// 254 FE
			if (this.Reader.IsNotByte(254)) return false;
			// 186 BA
			if (this.Reader.IsNotByte(186)) return false;
			// 190 BE
			if (this.Reader.IsNotByte(190)) return false;
			return true;
		}

		private void Read() {
			// CA FE BA BE (U4)
			if (!this.VerifyMagicCode()) throw new Exception();

			// VERSION MINOR (U2)
			this.Reader.Skip(2);
			// VERSION MAJOR (U2)
			this.Reader.Skip(2);

			// CONSTANT POOL
			this.Constants = JavaConstant.ReadAll(this.Reader);

			// ACCESS FLAGS (U2)
			this.Reader.Skip(2);
			// THIS CLASS (U2)
			this.Reader.Skip(2);
			// SUPER CLASS (U2)
			this.Reader.Skip(2);
			
			// INTERFACES
			this.ReadInterfaces();
			// FIELDS
			this.ReadFields();
			// METHODS
			this.ReadMethods();

			// ATTRIBUTES POOL
			this.Attributes = JavaAttribute.ReadAll(this.Reader, this.Constants);
		}

		private void ReadInterfaces() {
			// INTERFACES LENGTH (U2)
			int length = this.Reader.ReadShort();

			// INTERFACES (LENGTH * U2)
			this.Reader.Skip(length * 2);
		}

		private void ReadFields() {
			// FIELDS LENGTH (U2)
			int length = this.Reader.ReadShort();
			for (int i = 0; i < length; i++) this.ReadField();
		}

		private void ReadField() {
			// FIELD ACCESS FLAGS (U2)
			this.Reader.Skip(2);

			// FIELD NAME INDEX (U2)
			this.Reader.Skip(2);

			// FIELD DESCRIPTOR INDEX (U2)
			this.Reader.Skip(2);

			// FIELD ATTRIBUTES
			JavaAttribute.SkipAll(this.Reader);
		}

		private void ReadMethods() {
			// METHODS LENGTH (U2)
			int length = this.Reader.ReadShort();
			for (int i = 0; i < length; i++) this.ReadField();
		}

		private void ReadMethod() {
			// METHOD ACCESS FLAGS (U2)
			this.Reader.Skip(2);

			// METHOD NAME INDEX (U2)
			this.Reader.Skip(2);

			// METHOD DESCRIPTOR INDEX (U2)
			this.Reader.Skip(2);

			// METHOD ATTRIBUTES
			JavaAttribute.SkipAll(this.Reader);
		}
		
	}

}
