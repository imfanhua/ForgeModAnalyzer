namespace ForgeModAnalyzer {

	public sealed class ModInfo {
		
		public string ID { get; set; }
		public string Name { get; set; }

		public string Description { get; set; }
		public string Version { get; set; }
		public string[] MinecraftVersion { get; set; }

		public string URL { get; set; }
		public string Credits { get; set; }
		public string[] Authors { get; set; }
		
		public string[] Dependencies { get; set; }

		public byte[] Logo { get; set; }
		
		public ModInfo() {}
		
	}

}
