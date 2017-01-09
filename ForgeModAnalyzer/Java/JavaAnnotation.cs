using System.Collections.Generic;

namespace ForgeModAnalyzer.Java {

	public class JavaAnnotation {
		
		public static JavaAnnotation[] ReadAll(ByteReader reader, JavaConstant[] constants) {
			// ANNOTATIONS LENGTH (U2)
			int length = reader.ReadShort();

			JavaAnnotation[] annotations = new JavaAnnotation[length];
			for (int i = 0; i < length; i++) annotations[i] = JavaAnnotation.Read(reader, constants);
			return annotations;
		}

		public static JavaAnnotation Read(ByteReader reader, JavaConstant[] constants) {
			// ANNOTATION TYPE INDEX (U2)
			JavaAnnotation annotation = new JavaAnnotation((string) constants[reader.ReadShort() - 1].Value);
			// ANNOTATION ELEMENTVALUE LENGTH (U2)
			int length = reader.ReadShort();

			// ANNOTATION ELEMENTVALUE NAME INDEX (U2)
			for (int i = 0; i < length; i++) annotation.Elements.Add((string) constants[reader.ReadShort() - 1].Value, JavaElementValue.Read(reader, constants));
			return annotation;
		}
		
		public string Name { get; set; }
		public Dictionary<string, object> Elements { get; private set; } 
		
		public JavaAnnotation(string name) {
			this.Name = name;
			this.Elements = new Dictionary<string, object>();
		}

	}

}
