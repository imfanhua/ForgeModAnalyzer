using ForgeModAnalyzer.Java;
using ForgeModAnalyzer.Tools;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace ForgeModAnalyzer {

	public class Analyzer {
		
		public static ModInfo[] Read(string file) {
			return Analyzer.Read(file, Encoding.Default);
		}

		public static ModInfo[] Read(string file, Encoding encoding) {
			using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) return Analyzer.Read(stream, encoding);
		}

		public static ModInfo[] Read(Stream stream) {
			return Analyzer.Read(stream, Encoding.Default);
		}

		public static ModInfo[] Read(Stream stream, Encoding encoding) {
			try {
				using (ZipArchive zip = new ZipArchive(encoding, stream)) return new Analyzer(zip).Read();
			} catch {
				return null;
			}
		}
		
		private static void MergeTo(ModInfo info, ModInfo to) {
			if (info.Name != null) to.Name = info.Name;
			if (info.Description != null) to.Description = info.Description;
			if (info.Version != null) to.Version = info.Version;
			if (info.MinecraftVersion != null) to.MinecraftVersion = info.MinecraftVersion;
			if (info.Dependencies != null) to.Dependencies = info.Dependencies;
		}

		private ZipArchive zip;
		private Dictionary<string, ModInfo> info;
		
		private Analyzer(ZipArchive zip) {
			this.zip = zip;
			this.info = new Dictionary<string, ModInfo>();
		}

		private ModInfo[] Read() {
			this.Merge(this.ReadFromInfoFile());
			foreach (ZipFileInfo file in this.zip.Files) this.Merge(this.ReadFromClassFile(file));
			return new List<ModInfo>(this.info.Values).ToArray();
		}

		private ModInfo[] ReadFromInfoFile() {
			return ModInfoStruct.ToInfo(ModInfoStruct.Load(this.ReadFileText("mcmod.info")), this);
		}

		private ModInfo ReadFromClassFile(ZipFileInfo file) {
			if (file.IsVolumeLabel || file.IsFolder || !file.Name.ToLower().EndsWith(".class")) return null;

			ClassAnalyzer analyzer = null;
			using (Stream stream = file.OpenStream()) analyzer = ClassAnalyzer.Read(stream);
			if (analyzer == null) return null;
			
			ModInfo info = new ModInfo();
			foreach (JavaAttribute attribute in analyzer.Attributes) {
				if (attribute == null) continue;
				if (attribute is JavaRuntimeAnnotations) {
					foreach (JavaAnnotation annotation in ((JavaRuntimeAnnotations) attribute).Annotations) {
						if (annotation.Name.EndsWith("/fml/common/Mod;")) {
							if (annotation.Elements.ContainsKey("modid")) info.ID = (string) annotation.Elements["modid"];
							if (annotation.Elements.ContainsKey("name")) info.Name = (string) annotation.Elements["name"];
							if (annotation.Elements.ContainsKey("version")) info.Version = (string) annotation.Elements["version"];
							if (annotation.Elements.ContainsKey("acceptedMinecraftVersions")) info.MinecraftVersion = VersionTools.ToVersion((string) annotation.Elements["acceptedMinecraftVersions"]);
							if (annotation.Elements.ContainsKey("dependencies")) info.Dependencies = VersionTools.ToVersion((string) annotation.Elements["dependencies"]);
						} else if (annotation.Name.EndsWith("/fml/relauncher/IFMLLoadingPlugin$Name;")) {
							info.Name = (string) annotation.Elements["value"];
							if (info.ID == null) info.ID = info.Name;
						} else if (annotation.Name.EndsWith("/fml/relauncher/IFMLLoadingPlugin$MCVersion;")) info.MinecraftVersion = VersionTools.ToVersion((string) annotation.Elements["value"]);
					}
				}
			}
			
			return info;
		}
		
		public byte[] ReadFile(string file) {
			try {
				using (Stream stream = zip.GetFile(file).OpenStream()) {
					byte[] bytes = new byte[(int) stream.Length];
					stream.Read(bytes, 0, bytes.Length);
					return bytes;
				}
			} catch {
				return null;
			}
		}

		private string ReadFileText(string file) {
			byte[] bytes = this.ReadFile(file);
			if (bytes == null) return null;
			
			try {
				return Encoding.UTF8.GetString(bytes);
			} catch {
				return null;
			}
		}

		private void Merge(params ModInfo[] info) {
			if (info == null) return;
			foreach (ModInfo target in info) {
				if (target == null || target.ID == null) continue;
				if (!this.info.ContainsKey(target.ID)) this.info.Add(target.ID, target);
				else Analyzer.MergeTo(target, this.info[target.ID]);
			}
		}
		
	}

	[DataContract]
	internal class ModInfoStruct {
		
		private static DataContractJsonSerializer DeseralizerArray = new DataContractJsonSerializer(typeof(ModInfoStruct[]));
		private static DataContractJsonSerializer Deseralizer = new DataContractJsonSerializer(typeof(ModInfoStruct));

		public static ModInfoStruct[] Load(string json) {
			if (json == null) return null;

			try {
				using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json))) return ModInfoStruct.LoadArray(stream);
			} catch {
				return null;
			}
		}

		private static ModInfoStruct[] LoadArray(MemoryStream stream) {
			ModInfoStruct[] array = (ModInfoStruct[]) ModInfoStruct.DeseralizerArray.ReadObject(stream);
			if (array.Length < 1) {
				stream.Seek(0, SeekOrigin.Begin);
				array = new ModInfoStruct[] { (ModInfoStruct) ModInfoStruct.Deseralizer.ReadObject(stream) };
			}

			return array;
		}

		public static ModInfo[] ToInfo(ModInfoStruct[] structs, Analyzer analyzer) {
			if (structs == null) return null;
			List<ModInfo> list = new List<ModInfo>();

			foreach (ModInfoStruct target in structs) {
				if (target.Structs != null) list.AddRange(ModInfoStruct.ToInfo(target.Structs, analyzer));
				if (target.ID == null && target.Name == null) continue;
				if (target.ID == null) target.ID = target.Name;
				else if (target.Name == null) target.Name = target.ID;
				list.Add(target.ToInfo(analyzer));
			}

			return list.ToArray();
		}
		
		[DataMember(Name = "modid")]
		public string ID { get; set; }
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "description")]
		public string Description { get; set; }
		[DataMember(Name = "version")]
		public string Version { get; set; }
		[DataMember(Name = "mcversion")]
		public string MinecraftVersion { get; set; }

		[DataMember(Name = "url")]
		public string URL { get; set; }
		[DataMember(Name = "credits")]
		public string Credits { get; set; }
		[DataMember(Name = "authorList")]
		public string[] Authors { get; set; }

		[DataMember(Name = "dependencies")]
		public string[] Dependencies1 { get; set; }
		[DataMember(Name = "requiredMods")]
		public string[] Dependencies2 { get; set; }
		
		[DataMember(Name = "logoFile")]
		public string LogoFile { get; set; }
		
		[DataMember(Name = "modList")]
		public ModInfoStruct[] Structs { get; set; }

		public ModInfoStruct() {}

		public ModInfo ToInfo(Analyzer analyzer) {
			ModInfo info = new ModInfo() {
				ID = this.ID,
				Name = this.Name,

				Description = this.Description,
				Version = this.Version,

				URL = this.URL,
				Credits = this.Credits,
				Authors = this.Authors,
			};

			if (this.MinecraftVersion != null) info.MinecraftVersion = VersionTools.ToVersion(this.MinecraftVersion);

			string[] dependencies = null;
			if (this.Dependencies1 != null && this.Dependencies1.Length > 0) dependencies = this.Dependencies1;
			else if (this.Dependencies2 != null && this.Dependencies2.Length > 0) dependencies = this.Dependencies2;

			if (dependencies != null) info.Dependencies = VersionTools.AllToVersion(dependencies);
			if (this.LogoFile != null) info.Logo = analyzer.ReadFile(this.LogoFile);

			return info;
		}

	}

}
