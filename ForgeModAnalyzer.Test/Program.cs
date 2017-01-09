using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForgeModAnalyzer.Test {
	
	class Program {

		public static void Main(string[] args) {
			Program.TestModFile("Test1.jar");
			Console.WriteLine();
			Program.TestModFile("Test2.jar");
			Console.WriteLine();
			Program.TestModFile("Test3.jar");
			Console.WriteLine();
			Program.TestModFile("Test4.jar");
			Console.WriteLine();
			Program.TestModFile("Test5.jar");
			Console.WriteLine();
			Program.TestModFile("Test6.jar");
			Console.WriteLine();

			Console.ReadLine();
			Environment.Exit(0);
		}

		private static void TestModFile(string file) {
			ModInfo[] info = Analyzer.Read(file);
			if (info == null) {
				Console.WriteLine("Error!");
				return;
			}

			Console.WriteLine("Mods: [File: " + file + "]");
			foreach (ModInfo target in info) {
				Console.WriteLine("    Mod: " + target.ID);
				Console.WriteLine("        Name: " + target.Name);

				if (target.Description != null) Console.WriteLine("        Description: " + target.Description);
				if (target.Version != null) Console.WriteLine("        Version: " + target.Version);
				if (target.MinecraftVersion != null) {
					Console.WriteLine("        MinecraftVersion: [" + target.MinecraftVersion.Length + "]");
					for (int i = 0; i < target.MinecraftVersion.Length; i++) Console.WriteLine("            [" + i + "] " + target.MinecraftVersion[i]);
				}

				if (target.URL != null) Console.WriteLine("        URL: " + target.URL);
				if (target.Credits != null) Console.WriteLine("        Credits: " + target.Credits);
				if (target.Authors != null) {
					Console.WriteLine("        Authors: [" + target.Authors.Length + "]");
					for (int i = 0; i < target.Authors.Length; i++) Console.WriteLine("            [" + i + "] " + target.Authors[i]);
				}

				if (target.Dependencies != null) {
					Console.WriteLine("        Dependencies: [" + target.Dependencies.Length + "]");
					for (int i = 0; i < target.Dependencies.Length; i++) Console.WriteLine("            [" + i + "] " + target.Dependencies[i]);
				}

				if (target.Logo != null) Console.WriteLine("        Logo: [File (" + target.Logo.Length + " Bytes)]");
			}
		}

	}

}
