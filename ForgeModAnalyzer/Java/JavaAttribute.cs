namespace ForgeModAnalyzer.Java {

	public abstract class JavaAttribute {

		public static void SkipAll(ByteReader reader) {
			// ATTRIBUTE POOL LENGTH (U2)
			int length = reader.ReadShort();

			for (int i = 0; i < length; i++) JavaAttribute.Skip(reader);
		}

		public static void Skip(ByteReader reader) {
			// ATTRIBUTE NAME INDEX (U2)
			reader.Skip(2);
			// ATTRIBUTE LENGTH (U4)
			int length = reader.ReadInt();
			// ATTRIBUTE BYTES (LENGTH * U1)
			reader.Skip(length);
		}
		
		public static JavaAttribute[] ReadAll(ByteReader reader, JavaConstant[] constants) {
			// ATTRIBUTE POOL LENGTH (U2)
			int length = reader.ReadShort();

			JavaAttribute[] attributes = new JavaAttribute[length];
			for (int i = 0; i < length; i++) attributes[i] = JavaAttribute.Read(reader, constants);
			return attributes;
		}

		public static JavaAttribute Read(ByteReader reader, JavaConstant[] constants) {
			// ATTRIBUTE NAME INDEX (U2)
			string tag = (string) constants[reader.ReadShort() - 1].Value;
			// ATTRIBUTE LENGTH (U4)
			int length = reader.ReadInt();
			
			switch(tag) {
				case "RuntimeVisibleAnnotations":
					return JavaAttribute.ReadRuntimeVisibleAnnotations(reader, constants);
				default:
					reader.Skip(length);
					return null;
			}
		}

		public static JavaAttribute ReadRuntimeVisibleAnnotations(ByteReader reader, JavaConstant[] constants) {
			// ATTRIBUTE RUNTIMEANNOTATIONS ANNOTATIONS
			return new JavaRuntimeAnnotations(true, JavaAnnotation.ReadAll(reader, constants));
		}
		
		public JavaAttribute() {}
		
	}

	public class JavaRuntimeAnnotations : JavaAttribute {
		
		public bool IsVisible { get; private set; }
		public JavaAnnotation[] Annotations { get; private set; }
		
		public JavaRuntimeAnnotations(bool visible, JavaAnnotation[] annotations) {
			this.IsVisible = visible;
			this.Annotations = annotations;
		}

	}
	
}
