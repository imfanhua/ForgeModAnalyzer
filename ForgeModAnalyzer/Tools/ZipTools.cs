using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ForgeModAnalyzer.Tools {

	internal static class ZipReflection {
		
		public readonly static Assembly Base = typeof(System.IO.Packaging.Package).Assembly;

		public readonly static Type ZipArchive = Base.GetType("MS.Internal.IO.Zip.ZipArchive");
		public readonly static Type ZipFileInfo = Base.GetType("MS.Internal.IO.Zip.ZipFileInfo");
		public readonly static Type ZipIOBlockManager = Base.GetType("MS.Internal.IO.Zip.ZipIOBlockManager");

		public readonly static Type DeflateOptionEnum = Base.GetType("MS.Internal.IO.Zip.DeflateOptionEnum");
		public readonly static Type CompressionMethodEnum = Base.GetType("MS.Internal.IO.Zip.CompressionMethodEnum");
		
	}

	internal static class ZipArchiveReflection {
		
		public readonly static MethodInfo OpenOnStream = ZipReflection.ZipArchive.GetMethod("OpenOnStream", BindingFlags.NonPublic | BindingFlags.Static);
		public readonly static MethodInfo GetFile = ZipReflection.ZipArchive.GetMethod("GetFile", BindingFlags.NonPublic | BindingFlags.Instance);
		public readonly static MethodInfo GetFiles = ZipReflection.ZipArchive.GetMethod("GetFiles", BindingFlags.NonPublic | BindingFlags.Instance);

		public readonly static FieldInfo ZipIOBlockManager = ZipReflection.ZipArchive.GetField("_blockManager", BindingFlags.NonPublic | BindingFlags.Instance);
		
	}

	internal static class ZipFileInfoReflection {
		
		public readonly static MethodInfo GetStream = ZipReflection.ZipFileInfo.GetMethod("GetStream", BindingFlags.NonPublic | BindingFlags.Instance);

		public readonly static PropertyInfo Name = ZipReflection.ZipFileInfo.GetProperty("Name", BindingFlags.NonPublic | BindingFlags.Instance);
		public readonly static PropertyInfo DeflateOption = ZipReflection.ZipFileInfo.GetProperty("DeflateOption", BindingFlags.NonPublic | BindingFlags.Instance);
		public readonly static PropertyInfo CompressionMethod = ZipReflection.ZipFileInfo.GetProperty("CompressionMethod", BindingFlags.NonPublic | BindingFlags.Instance);
		public readonly static PropertyInfo FolderFlag = ZipReflection.ZipFileInfo.GetProperty("FolderFlag", BindingFlags.NonPublic | BindingFlags.Instance);
		public readonly static PropertyInfo VolumeLabelFlag = ZipReflection.ZipFileInfo.GetProperty("VolumeLabelFlag", BindingFlags.NonPublic | BindingFlags.Instance);
		public readonly static PropertyInfo LastModFileDateTime = ZipReflection.ZipFileInfo.GetProperty("LastModFileDateTime", BindingFlags.NonPublic | BindingFlags.Instance);
		
	}

	internal static class ZipIOBlockManagerReflection {
		
		public readonly static FieldInfo Encoding = ZipReflection.ZipIOBlockManager.GetField("_encoding", BindingFlags.NonPublic | BindingFlags.Instance);
		
	}

	internal class InternalEncoding : ASCIIEncoding {
		
		internal readonly Encoding Encoding;
		
		public InternalEncoding(Encoding encoding) {
			this.Encoding = encoding;
		}

		public override byte[] GetPreamble() { return this.Encoding.GetPreamble(); }

		public override Decoder GetDecoder() { return this.Encoding.GetDecoder(); }
		public override Encoder GetEncoder() { return this.Encoding.GetEncoder(); }

		public override int GetHashCode() { return this.Encoding.GetHashCode(); }

		public override int GetMaxByteCount(int count) { return this.Encoding.GetMaxByteCount(count); }
		public override int GetMaxCharCount(int count) { return this.Encoding.GetMaxCharCount(count); }
		
		public override int GetByteCount(char[] chars, int index, int count) { return this.Encoding.GetByteCount(chars, index, count); }
		public override int GetByteCount(string chars) { return this.Encoding.GetByteCount(chars); }
		public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) { return this.Encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex); }
		public override int GetBytes(string target, int charIndex, int charCount, byte[] bytes, int byteIndex) { return this.Encoding.GetBytes(target, charIndex, charCount, bytes, byteIndex); }
		
		public override string GetString(byte[] bytes, int index, int count) { return this.Encoding.GetString(bytes, index, count); }

		public override int GetCharCount(byte[] bytes, int index, int count) { return this.Encoding.GetCharCount(bytes, index, count); }
		public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) { return this.Encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex); }

		public override bool Equals(object value) { return this.Encoding.Equals(value); }

	}

	public enum CompressionMethod {
		Stored,
		Deflated
	};
	
	public enum DeflateOption {
		Normal,
		Maximum,
		Fast,
		SuperFast
	};
	
	public class ZipArchive : IDisposable {
		
		private IDisposable Instance;
		
		private InternalEncoding Encode;
		
		public Encoding Encoding {
			
			get {
				return this.Encode;
			}

			set {
				if (value == null || this.Encode?.Encoding == value) return;
				this.Encode = new InternalEncoding(value);
				object io = ZipArchiveReflection.ZipIOBlockManager.GetValue(this.Instance);
				ZipIOBlockManagerReflection.Encoding.SetValue(io, this.Encode);
			}

		}
		
		public IEnumerable<ZipFileInfo> Files {
			get {
				IEnumerable collections = ZipArchiveReflection.GetFiles.Invoke(this.Instance, null) as IEnumerable;
				foreach (object info in collections) yield return new ZipFileInfo(info);
			}
		}
		
		public ZipArchive(Encoding encoding, Stream stream, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, bool streaming = false)
		: this(ZipArchiveReflection.OpenOnStream.Invoke(null, new object[] { stream, mode, access, streaming }), encoding) {}

		private ZipArchive(object Instance, Encoding encoding) {
			this.Instance = (IDisposable) Instance;
			this.Encoding = encoding;
		}
		
		public ZipFileInfo GetFile(string name) {
			return new ZipFileInfo(ZipArchiveReflection.GetFile.Invoke(this.Instance, new object[] { name }));
		}
		
		public bool HasFile(string name) {
			try {
				return ZipArchiveReflection.GetFile.Invoke(this.Instance, new object[] { name }) != null;
			} catch {
				 return false;
			}
		}
		
		public bool Validate() {
			try {
				foreach (ZipFileInfo info in this.Files) info.OpenStream().Close();
				return true;
			} catch {
				return false;
			}
		}
		
		public void Dispose() {
			this.Instance.Dispose();
		}

	}
	
	public class ZipFileInfo {
		
		private object Instance;
		
		internal ZipFileInfo(object instance) {
			this.Instance = instance;
		}
		
		private object GetProperty(PropertyInfo info) {
			return info.GetValue(this.Instance, null);
		}
		
		public string Name {
			get { return this.GetProperty(ZipFileInfoReflection.Name) as string; }
		}
		
		public DateTime LastModFileDateTime {
			get { return (DateTime) this.GetProperty(ZipFileInfoReflection.LastModFileDateTime); }
		}
		
		public bool IsFolder {
			get { return (bool) this.GetProperty(ZipFileInfoReflection.FolderFlag); }
		}
		
		public bool IsVolumeLabel {
			get { return (bool) this.GetProperty(ZipFileInfoReflection.VolumeLabelFlag); }
		}
		
		public DeflateOption DeflateOption {
			get { return (DeflateOption) Enum.Parse(typeof(DeflateOption), this.GetProperty(ZipFileInfoReflection.DeflateOption).ToString()); }
		}
		
		public CompressionMethod CompressionMethod {
			get { return (CompressionMethod) Enum.Parse(typeof(CompressionMethod), this.GetProperty(ZipFileInfoReflection.CompressionMethod).ToString()); }
		}
		
		public Stream OpenStream(FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read) {
			return (Stream) ZipFileInfoReflection.GetStream.Invoke(this.Instance, new object[] { mode, access });
		}
		
		public void CopyTo(Stream to) {
			using (Stream stream = this.OpenStream()) stream.CopyTo(to);
		}
		
	}

}
