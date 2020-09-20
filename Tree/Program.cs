using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Tree
{
	public class DirectoryStructure
	{
		public DirectoryStructure(DirectoryInfo directoryInfo)
		{
			this.directoryInfo = directoryInfo;

			RefreshSubdirectories();
			RefreshFiles();
		}

		public void RefreshSubdirectories()
		{
			subdirectories.Clear();
			DirectoryInfo [] directoriesInfo = directoryInfo.GetDirectories();

			foreach (DirectoryInfo directory in directoriesInfo)
				subdirectories.Add(new DirectoryStructure(directory));
		}

		public void RefreshFiles()
		{
			files.Clear();
			FileInfo[] filesInfo = directoryInfo.GetFiles();

			foreach(FileInfo file in filesInfo)
				files.Add(file.Name);
		}

		public void DrawStructure(int spacesToAdd = 0)
        {
			for(int i = 1; i <= spacesToAdd; i++) Console.WriteLine(" ");
			Console.WriteLine("+" + directoryInfo.Name);

			foreach (DirectoryStructure subdirectory in subdirectories)
			{
				subdirectory.DrawStructure(spacesToAdd + 4);
			}

			foreach (string file in files)
			{
				for (int i = 1; i <= spacesToAdd; i++) Console.WriteLine(" ");
				Console.WriteLine(file);
			}

			Console.WriteLine();
        }


		public List<DirectoryStructure> subdirectories = new List<DirectoryStructure>();
		public List<string> files = new List<string>();

		DirectoryInfo directoryInfo;

		public string Name { get; set; }
	}

	class Program
	{
        //directorystructure getdirectories(string path)
        //{
        //    directoryinfo directory = new directoryinfo(path);

        //    directoryinfo[] subdirectories = directory.getdirectories();

        //    return new directoryinfo[] { };
        //}

        static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				foreach (string arg in args)
					Console.WriteLine(arg);
			}
			else
			{
				string path = Directory.GetCurrentDirectory();

				DirectoryInfo thisDirectory = new DirectoryInfo(path);

				DirectoryStructure structure = new DirectoryStructure(thisDirectory);

				structure.DrawStructure();
			}

			Console.ReadKey();
		}
	}
}
