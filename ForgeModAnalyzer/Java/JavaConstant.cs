using System.Text;

namespace ForgeModAnalyzer.Java {

	public class JavaConstant {
		
		public static JavaConstant[] ReadAll(ByteReader reader) {
			// CONSTANT POOL LENGTH (U2)
			int length = reader.ReadShort() - 1;

			IConstantHolder[] holders = new IConstantHolder[length];
			for (int i = 0; i < length; i++) holders[i] = JavaConstant.Read(reader);

			JavaConstant[] array = new JavaConstant[length];
			for (int i = 0; i < length; i++) array[i] = holders[i].GetConstant(holders);
			return array;
		}

		public static IConstantHolder Read(ByteReader reader) {
			// CONSTANT TAG (U1)
			int tag = reader.ReadByte();

			switch(tag) {
				case 1:
					return JavaConstant.ReadUTF8(reader);
				case 3:
					return JavaConstant.ReadInteger(reader);
				case 4:
					return JavaConstant.ReadFloat(reader);
				case 5:
					return JavaConstant.ReadLong(reader);
				case 6:
					return JavaConstant.ReadDouble(reader);
				case 7:
					return JavaConstant.ReadClass(reader);
				case 8:
					return JavaConstant.ReadString(reader);
				case 9:
					return JavaConstant.ReadFieldRef(reader);
				case 10:
					return JavaConstant.ReadMethodRef(reader);
				case 11:
					return JavaConstant.ReadInterfaceMethodRef(reader);
				case 12:
					return JavaConstant.ReadNameAndType(reader);
				case 15:
					return JavaConstant.ReadMethodHandle(reader);
				case 16:
					return JavaConstant.ReadMethodType(reader);
				case 18:
					return JavaConstant.ReadInvokeDynamic(reader);
				default:
					return null;
			}
		}

		// TAG 1
		private static IConstantHolder ReadUTF8(ByteReader reader) {
			// CONSTANT UTF8 LENGTH (U2)
			int length = reader.ReadShort();
			// CONSTANT UTF8 BYTES (LENGTH * U1)
			byte[] bytes = reader.ReadBytes(length);

			return new StaticConstantHolder(new JavaConstant(Encoding.UTF8.GetString(bytes)));
		}

		// TAG 3
		private static IConstantHolder ReadInteger(ByteReader reader) {
			// CONSTANT INTEGER BYTES (U4)
			reader.Skip(4);

			return VoidConstantHolder.Holder;
		}

		// TAG 4
		private static IConstantHolder ReadFloat(ByteReader reader) {
			// CONSTANT FLOAT BYTES (U4)
			reader.Skip(4);

			return VoidConstantHolder.Holder;
		}

		// TAG 5
		private static IConstantHolder ReadLong(ByteReader reader) {
			// CONSTANT LONG HIGH BYTES (U4)
			reader.Skip(4);
			// CONSTANT LONG LOW BYTES (U4)
			reader.Skip(4);

			return VoidConstantHolder.Holder;
		}

		// TAG 6
		private static IConstantHolder ReadDouble(ByteReader reader) {
			// CONSTANT DOUBLE HIGH BYTES (U4)
			reader.Skip(4);
			// CONSTANT DOUBLE LOW BYTES (U4)
			reader.Skip(4);

			return VoidConstantHolder.Holder;
		}

		// TAG 7
		private static IConstantHolder ReadClass(ByteReader reader) {
			// CONSTANT CLASS NAME INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}

		// TAG 8
		private static IConstantHolder ReadString(ByteReader reader) {
			// CONSTANT STRING INDEX (U2)
			return new StringConstantHolder(reader.ReadShort() - 1);
		}
		
		// TAG 9
		private static IConstantHolder ReadFieldRef(ByteReader reader) {
			// CONSTANT FIELDREF CLASS INDEX (U2)
			reader.Skip(2);
			// CONSTANT FIELDREF NAME AND TYPE INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}

		// TAG 10
		private static IConstantHolder ReadMethodRef(ByteReader reader) {
			// CONSTANT METHODREF CLASS INDEX (U2)
			reader.Skip(2);
			// CONSTANT METHODREF NAME AND TYPE INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}

		// TAG 11
		private static IConstantHolder ReadInterfaceMethodRef(ByteReader reader) {
			// CONSTANT INTERFACEMETHODREF CLASS INDEX (U2)
			reader.Skip(2);
			// CONSTANT INTERFACEMETHODREF NAME AND TYPE INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}

		// TAG 12
		private static IConstantHolder ReadNameAndType(ByteReader reader) {
			// CONSTANT NAMEANDTYPE NAME INDEX (U2)
			reader.Skip(2);
			// CONSTANT NAMEANDTYPE DESCRIPTOR INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}

		// TAG 15
		private static IConstantHolder ReadMethodHandle(ByteReader reader) {
			// CONSTANT METHODHANDLE REFERENCE KIND (U1)
			reader.Skip(1);
			// CONSTANT METHODHANDLE REFERENCE INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}

		// TAG 16
		private static IConstantHolder ReadMethodType(ByteReader reader) {
			// CONSTANT METHODTYPE DESCRIPTOR INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}

		// TAG 18
		private static IConstantHolder ReadInvokeDynamic(ByteReader reader) {
			// CONSTANT INVOKEDYNAMIC BOOTSTRAP METHOD ATTR INDEX (U2)
			reader.Skip(2);
			// CONSTANT INVOKEDYNAMIC NAME AND TYPE INDEX (U2)
			reader.Skip(2);

			return VoidConstantHolder.Holder;
		}
		
		public object Value { get; private set; }
		
		public JavaConstant(object value) {
			this.Value = value;
		}
		
	}

	public interface IConstantHolder {
		
		JavaConstant GetConstant();
		JavaConstant GetConstant(IConstantHolder[] pool);
		
	}

	public sealed class StaticConstantHolder : IConstantHolder {
		
		public JavaConstant Constant { get; set; }
		
		public StaticConstantHolder(JavaConstant constant) {
			this.Constant = constant;
		}

		public JavaConstant GetConstant() {
			return this.Constant;
		}

		public JavaConstant GetConstant(IConstantHolder[] pool) {
			return this.Constant;
		}

	}

	public sealed class VoidConstantHolder : IConstantHolder {
		
		public static VoidConstantHolder Holder { get; private set; } = new VoidConstantHolder();
		
		private VoidConstantHolder() {}

		public JavaConstant GetConstant() {
			return null;
		}

		public JavaConstant GetConstant(IConstantHolder[] pool) {
			return null;
		}

	}

	public sealed class StringConstantHolder : IConstantHolder {
		
		public int Index { get; set; }
		
		public StringConstantHolder(int index) {
			this.Index = index;
		}

		public JavaConstant GetConstant() {
			return null;
		}

		public JavaConstant GetConstant(IConstantHolder[] pool) {
			try {
				return pool[this.Index].GetConstant();
			} catch {
				return null;
			}
		}

	}

}
