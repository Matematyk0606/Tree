using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tree
{
	public class NumberOfSpecialSignsToDrawExeption : Exception
    {
		public NumberOfSpecialSignsToDrawExeption(string message) : base(message) { }
    }



	public class DirectoryStructure
	{
		public T DeepClone<T>(T obj)
		{
			using (var ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(ms, obj);
				ms.Position = 0;

				return (T)formatter.Deserialize(ms);
			}
		}

		public DirectoryStructure(DirectoryInfo directoryInfo, bool typeOfSpecialSigns = false)
		{
			this.directoryInfo = directoryInfo;

			this.typeOfSpecialSigns = typeOfSpecialSigns;
			selectedTypeOfSpecialSigns = typeOfSpecialSigns ? secondTypeOfSpecialSigns : firstTypeOfSpecialSigns;

			RefreshSubdirectories();
			RefreshFiles();
		}

		public void RefreshSubdirectories()
		{
			subdirectories.Clear();

			DirectoryInfo[] directoriesInfo = new DirectoryInfo[] { };

			try { directoriesInfo = directoryInfo.GetDirectories(); }
			catch (UnauthorizedAccessException ex) { }
			catch (Exception ex) { }

			foreach (DirectoryInfo directory in directoriesInfo)
				subdirectories.Add(new DirectoryStructure(directory, typeOfSpecialSigns));

		}

		public void RefreshFiles()
		{
			files.Clear();
			FileInfo[] filesInfo = new FileInfo[] { };

			try { filesInfo = directoryInfo.GetFiles(); }
			catch (UnauthorizedAccessException ex) { }
			catch (Exception ex) { }

			foreach (FileInfo file in filesInfo)
				files.Add(file.Name);
		}

		public void DrawStructure(int levelInStructure = 0, SortedSet<int> placesOfSpecialSigns = default(SortedSet<int>), char specialSignForThisdirectory = default)
		{
			// Wzór do użycia: pozycja_znaku_specjalnego = 4*levelInStructure + 1

			placesOfSpecialSigns ??= new SortedSet<int>();
			if(specialSignForThisdirectory == '\0') specialSignForThisdirectory = selectedTypeOfSpecialSigns[(int)SpecialSigns.Double];

			try
			{
				if (placesOfSpecialSigns.Count >= levelInStructure + 1)
					throw new NumberOfSpecialSignsToDrawExeption("WYJĄTEK: Ilość równoległych znaków gałęzi do narysowania nie może być większa od levelInStructure-1");
			}
			catch (NumberOfSpecialSignsToDrawExeption ex)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine(ex.Message);
				Console.WriteLine("Jeśli pomimo wszystko chcesz kontynuować wykonywanie programu naciśnij dowolny klawisz...");
				Console.ForegroundColor = ConsoleColor.White;
				Console.ReadKey();
			}

			////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////////////////////////////////////////////////////

			string signsBeforeNamesOfObjects = "";
			string signsBeforeNamesOfDirectory = "";

			if (levelInStructure != 0)
			{
				for (int i = 0; i < levelInStructure - 1; i++)
				{
					if (placesOfSpecialSigns.Contains(i))
						signsBeforeNamesOfObjects += selectedTypeOfSpecialSigns[(int)SpecialSigns.Vertical] + "   "; // znak: |
					else
						signsBeforeNamesOfObjects += "    ";
				}

				signsBeforeNamesOfDirectory = signsBeforeNamesOfObjects + specialSignForThisdirectory +
											  selectedTypeOfSpecialSigns[(int)SpecialSigns.Horizontal] + selectedTypeOfSpecialSigns[(int)SpecialSigns.Horizontal] +
											  selectedTypeOfSpecialSigns[(int)SpecialSigns.Horizontal]; // znak: -
			}


			Console.Write(signsBeforeNamesOfDirectory);

			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine(directoryInfo.Name);
			Console.ForegroundColor = ConsoleColor.White;

			////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////////////////////////////////////////////////////

			if (files.Count > 0)
			{
				string signsBeforeNameOfFile = signsBeforeNamesOfObjects;
				if (levelInStructure != 0 && placesOfSpecialSigns.Contains(levelInStructure - 1)) signsBeforeNameOfFile += selectedTypeOfSpecialSigns[(int)SpecialSigns.Vertical] + "   "; // znak: |
				else if (levelInStructure != 0 && !placesOfSpecialSigns.Contains(levelInStructure - 1)) signsBeforeNameOfFile += "    ";

				if (subdirectories.Count > 0) signsBeforeNameOfFile += selectedTypeOfSpecialSigns[(int)SpecialSigns.Vertical] + "   "; // znak: |
				else signsBeforeNameOfFile += "    ";

				foreach (string file in files)
				{ 
					Console.Write(signsBeforeNameOfFile);

					Console.ForegroundColor = ConsoleColor.DarkYellow;
					Console.WriteLine(file);
					Console.ForegroundColor = ConsoleColor.White;
				}

				Console.WriteLine(signsBeforeNameOfFile);
			}

			////////////////////////////////////////////////////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////////////////////////////////////////////////////

			if (subdirectories.Count > 0)
			{
				for (int i = 0; i < subdirectories.Count; i++)
				{
					char specialSignForSubdirectory;
					if (i + 1 == subdirectories.Count) specialSignForSubdirectory = selectedTypeOfSpecialSigns[(int)SpecialSigns.Double]; // znak: \
					else specialSignForSubdirectory = selectedTypeOfSpecialSigns[(int)SpecialSigns.Triple]; // znak: +

					if (i < subdirectories.Count - 1)
					{
						SortedSet<int> nextPlacesOfSpecialSigns = DeepClone(placesOfSpecialSigns);
						nextPlacesOfSpecialSigns.Add(levelInStructure);
						
						subdirectories[i].DrawStructure(levelInStructure + 1, nextPlacesOfSpecialSigns, specialSignForSubdirectory);
					}
					else
						subdirectories[i].DrawStructure(levelInStructure + 1, placesOfSpecialSigns, specialSignForSubdirectory);
				}
			}

        }


		public List<DirectoryStructure> subdirectories = new List<DirectoryStructure>();
		public List<string> files = new List<string>();

		DirectoryInfo directoryInfo;

		enum SpecialSigns
        {
			Vertical = 0,
			Horizontal = 1,
			Double = 2,
			Triple = 3
        }
		bool typeOfSpecialSigns = false;
		static string firstTypeOfSpecialSigns = @"│─└├";
		static string secondTypeOfSpecialSigns = @"|-\+";
		string selectedTypeOfSpecialSigns;

	}



	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;

			try { Console.SetBufferSize(Console.BufferWidth, 32766); }
			catch (IOException ex) { }
			catch (Exception ex) { }

			if (args.Length > 0)
			{
				foreach (string arg in args)
					Console.WriteLine(arg);

				if(args[0] != "-")
				{
					string path = args[0];

					DirectoryInfo thisDirectory = new DirectoryInfo(path);

					DirectoryStructure structure = new DirectoryStructure(thisDirectory);

					structure.DrawStructure();
				}
			}
			else
			{
				string path = @"E:\Visual Studio Projects\Source\Repos\Tree";

				DirectoryInfo thisDirectory = new DirectoryInfo(path);

				DirectoryStructure structure = new DirectoryStructure(thisDirectory);

				structure.DrawStructure();
			}

			//Console.ReadKey();
		}
	}
}
