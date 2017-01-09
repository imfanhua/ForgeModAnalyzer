namespace ForgeModAnalyzer.Java {

	public abstract class JavaElementValue {
		
		public static object Read(ByteReader reader, JavaConstant[] constants) {
			// ELEMENTVALUE TAG (U1)
			int tag = reader.ReadByte();

			switch(tag) {
				case '@':
					return JavaElementValue.ReadAnnotationType(reader, constants);
				case 'e':
					return JavaElementValue.ReadEnum(reader, constants);
				case '[':
					return JavaElementValue.ReadArray(reader, constants);
				default:
					return JavaElementValue.ReadConstValue(reader, constants, tag);
			}
		}

		private static object ReadConstValue(ByteReader reader, JavaConstant[] constants, int tag) {
			switch(tag) {
				case 'B':
				case 'C':
				case 'D':
				case 'F':
				case 'I':
				case 'J':
				case 'S':
				case 'Z':
				case 's':
				case 'c':
					// ELEMENTVALUE CONSTVALUE CONSTANT INDEX (U2)
					JavaConstant constant = constants[reader.ReadShort() - 1];
					return constant != null ? constant.Value : null;
				default:
					return null;
			}
		}

		private static object ReadEnum(ByteReader reader, JavaConstant[] constants) {
			// ELEMENTVALUE ENUM TYPE NAME INDEX (U2)
			string name = (string) constants[reader.ReadShort() - 1].Value;
			// ELEMENTVALUE ENUM CONST NAME INDEX (U2)
			string value = (string) constants[reader.ReadShort() - 1].Value;

			return new JavaEnum(name, value);
		}

		private static object ReadAnnotationType(ByteReader reader, JavaConstant[] constants) {
			// ELEMENTVALUE ANNOTATIONTYPE
			return JavaAnnotation.Read(reader, constants);
		}

		private static object ReadArray(ByteReader reader, JavaConstant[] constants) {
			// ELEMENTVALUE ARRAY LENGTH (U2)
			int length = reader.ReadShort();

			object[] array = new object[length];
			for (int i = 0; i < length; i++) array[i] = JavaElementValue.Read(reader, constants);
			return array;
		}
		
		public JavaElementValue() {}
		
	}

	public sealed class JavaEnum : JavaElementValue {
		
		public string Name { get; private set; }
		public string Value { get; private set; }
		
		public JavaEnum(string name, string value) {
			this.Name = name;
			this.Value = value;
		}
		
	}

}
