using System;
using System.IO;

namespace SnippetBro
{
	class Program
	{
		private static string _installDir;
		private static string _appDir;

		/*
		 * Default code language is C#. Edit source below to change install location if your snippets are in another language
		 */

		static void Main(string[] args)
		{
			_appDir = Path.Combine(Directory.GetCurrentDirectory(), "Snippets");
			_installDir = GetInstallDir();

			var overwrite = GetOverwrite();

			Console.Clear();

			InstallSnippets(overwrite);
		}

		private static bool GetOverwrite()
		{
			PrettyWriteLine("Do you want to overwrite existing files with the same name?", ConsoleColor.White);
			PrettyWrite("Any key = no, ", ConsoleColor.Red);
			PrettyWrite("\"Y\" = yes.", ConsoleColor.Green);
			Console.WriteLine();
			var input = Console.ReadKey();
			return input.Key == ConsoleKey.Y;
		}

		private static void InstallSnippets(bool overwrite)
		{
			var foldersToInstall = Directory.GetDirectories(_appDir);

			foreach (var folderPath in foldersToInstall)
			{
				var files = Directory.GetFiles(folderPath);
				if (files.Length == 0) continue;

				var folderName = GetFileOrFolderName(folderPath);
				var createdDir = Directory.CreateDirectory(Path.Combine(_installDir, folderName));

				foreach (var filePath in files)
				{
					var fileName = GetFileOrFolderName(filePath);

					var newFilePath = Path.Combine(createdDir.FullName, fileName);

					var exists = File.Exists(newFilePath);
					WriteStatus(fileName, exists, overwrite);

					if (exists && !overwrite) continue;
					File.Copy(filePath, newFilePath, overwrite);
				}
			}
		}

		private static string GetFileOrFolderName(string path)
		{
			var startIndex = path.LastIndexOf(Path.DirectorySeparatorChar);
			var fileOrFolderName = path.Substring(startIndex + 1, path.Length - startIndex - 1);

			return fileOrFolderName;
		}

		private static string GetInstallDir()
		{
			var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			var path = Path.Combine(documents, "Visual Studio 2019", "Code Snippets", "Visual C#", "My Code Snippets");

			PrettyWriteLine("The default directory for snippets is:", ConsoleColor.White);
			Console.WriteLine();
			PrettyWriteLine($"    {path}", ConsoleColor.Cyan);
			Console.WriteLine();

			PrettyWriteLine("Press enter to install to default directory, or enter a new directory path:", ConsoleColor.White);
			var input = Console.ReadLine();

			return input?.Length > 0 ? input : path;
		}

		private static void WriteStatus(string fileName, bool exists, bool overwrite)
		{
			var skip = "[SKIPPING]    ";
			var ovr = "[OVERWRITING] ";
			var copy = "[COPYING]     ";

			var status = exists && !overwrite
				? skip : exists && overwrite
				? ovr : copy;

			var color = status == skip
				? ConsoleColor.Magenta : status == ovr
				? ConsoleColor.Yellow : ConsoleColor.Green;

			PrettyWrite(status, color);
			PrettyWrite(fileName, ConsoleColor.White);
			Console.WriteLine();
		}
		private static void PrettyWriteLine(string input, ConsoleColor color)
		{
			PrettyWrite(input, color);
			Console.WriteLine();
		}

		private static void PrettyWrite(string input, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(input);
			Console.ResetColor();
		}
	}
}