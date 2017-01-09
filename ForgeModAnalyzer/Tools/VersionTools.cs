using System.Collections.Generic;

namespace ForgeModAnalyzer.Tools {

	public static class VersionTools {

		public static string[] AllToVersion(string[] version) {
			List<string> list = new List<string>();
			for (int i = 0; i < version.Length; i++) list.AddRange(VersionTools.ToVersion(version[i]));
			return list.ToArray();
		}

		public static string[] ToVersion(string version) {
			if (version.Trim().Length < 1) return null;
			version = version.Replace("[", "").Replace("${", "").Replace("}", "").Replace("]", "").Replace(",)", "").Replace(")", "").Replace("required-after:", "> ").Replace("after:", "> ");
			return VersionTools.Split(version.Split(';'), ',');
		}

		public static string[] Split(string[] strings, params char[] separator) {
			List<string> list = new List<string>();
			foreach (string target in strings) if (target.Trim().Length > 0) list.AddRange(target.Split(separator));
			return list.ToArray();
		}

	}

}
