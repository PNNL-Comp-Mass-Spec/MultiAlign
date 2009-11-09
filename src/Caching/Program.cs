using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using PNNLControls.Data;



namespace cacheTest
{
    class Program
    {
		/* TestSerialization
		static void TestSerialization(string dataFileName, long bufferSize, long logicalSize, string textFileName)
		{
			StreamWriter textFile = File.CreateText(textFileName);
			List<string> benchmarks = new List<string>();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			clsSerializableProteinCacheArray data = new clsSerializableProteinCacheArray(dataFileName, bufferSize, logicalSize);

			long memory = GC.GetTotalMemory(false);

			//Stream serializationStream = File.Open(dataFileName, FileMode.Open);
			//BinaryFormatter binFormatter = new BinaryFormatter();

			for (long i = 0; i < logicalSize; i++)
			{
				serializableProtein tempProtein = new serializableProtein();
				tempProtein.Name = "Protein" + i.ToString();
				tempProtein.Value1 = 1.337;
				data[i] = tempProtein;
				//binFormatter.Serialize(serializationStream, tempProtein);

				if ((i % 5000) == 0)
				{
					memory = GC.GetTotalMemory(false);
					string benchmark = string.Format("Time:	{0}	Memory (in KB):	{1}",
									stopwatch.ElapsedMilliseconds / 1000.000,
									memory/1024);
					benchmarks.Add(benchmark);
				}
			}

			foreach (string bench in benchmarks)
			{
				textFile.WriteLine(bench);
			}

			//serializationStream.Close();			
			textFile.Close();
		}
		*/

		static void TestCache(long bufferSize, long logicalSize, string fileName)
		{
			StreamWriter textFile		= File.CreateText(fileName);
			Stopwatch stopwatch			= new Stopwatch();
			stopwatch.Start();
			//clsProteinCacheArray data = new clsProteinCacheArray(bufferSize, logicalSize);	//test with objects from the protein class
			clsSQLiteCacheArray<classProtein> data = new clsSQLiteCacheArray<classProtein>("SQLiteProteinDB.db3", enmSQLiteDataCacheMode.OpenCreate, bufferSize, logicalSize);	//test with objects from the protein class using the sqlitecachearray class

			Random r = new Random();

			long memory = GC.GetTotalMemory(false);

			for (int i = 0; i < logicalSize; i++)
			{
				classProtein tempProtein = new classProtein();
				tempProtein.Name = "protein" + i.ToString();
				tempProtein.Value1 = (r.NextDouble() * 100);
				tempProtein.Value2 = (r.NextDouble() * 100);
				tempProtein.Value3 = (r.NextDouble() * 100);
				tempProtein.Value4 = (r.NextDouble() * 100);
				tempProtein.Value5 = (r.NextDouble() * 100);
				tempProtein.Value6 = (r.NextDouble() * 100);
				tempProtein.Value7 = (r.NextDouble() * 100);
				tempProtein.Value8 = (r.NextDouble() * 100);
				tempProtein.Value9 = (r.NextDouble() * 100);
				data[i] = tempProtein;

				if ((i % 5000) == 0)
				{
					memory = GC.GetTotalMemory(false);
					textFile.Write("Time:	{0}", (stopwatch.ElapsedMilliseconds / 1000.000).ToString());
					textFile.WriteLine(string.Format("	Memory (in KB):	{0}", memory / (1024)));
				}
			}
			data.Dispose();
			
			textFile.Close();
		}

		static void TestList(long logicalSize, string fileName)
		{
			StreamWriter textFile		= File.CreateText(fileName);
			Stopwatch stopwatch			= new Stopwatch();

			stopwatch.Start();	
			List<classProtein> heapList = new List<classProtein>();

			Random r	= new Random();
			long memory = GC.GetTotalMemory(false);

			for (int i = 0; i < logicalSize; i++)
			{
				classProtein tempProtein = new classProtein();
				tempProtein.Name = "protein" + i.ToString();
				tempProtein.Value1 = (r.NextDouble() * 100);
				tempProtein.Value2 = (r.NextDouble() * 100);
				tempProtein.Value3 = (r.NextDouble() * 100);
				tempProtein.Value4 = (r.NextDouble() * 100);
				tempProtein.Value5 = (r.NextDouble() * 100);
				tempProtein.Value6 = (r.NextDouble() * 100);
				tempProtein.Value7 = (r.NextDouble() * 100);
				tempProtein.Value8 = (r.NextDouble() * 100);
				tempProtein.Value9 = (r.NextDouble() * 100);

				heapList.Add(tempProtein);
				
				if ((i % 5000) == 0)
				{
					memory = GC.GetTotalMemory(false);
					textFile.Write("Time:	{0}", (stopwatch.ElapsedMilliseconds / 1000.000).ToString());
					textFile.WriteLine(string.Format("	Memory (in KB):	{0}", memory / (1024)));
				}
			}
			textFile.Close();
		}

		static void Main(string[] args)
		{
			long logicalSize = 1400000;
			long bufferSize  = 4096;

			for (long i = 0; i < 1; i++)
			{
				TestCache(bufferSize, logicalSize, "TESTING/SQLiteTEST-transaction-" + bufferSize.ToString() + "-generic-full-release-jetbrains01.txt");
				bufferSize = bufferSize * 2;
				GC.Collect();
			}

		}
	}
}










/*	ORIGINAL EXAMPLE -- TEXT FILE
	static void Main()
	{
		int bufferSize = 10;
		int logicalSize = 100;
		clsTextFileCacheArray data = new clsTextFileCacheArray(bufferSize, logicalSize);

		for (int i = 0; i < logicalSize; i++)
		{
			classProtein protein = data[i];
			protein.Name = "protein" + i.ToString();
			protein.Value = Convert.ToDouble(i);
			data[i] = protein;
		}
		Random r = new Random();

		for (int i = 15; i < 20; i++)
		{
			classProtein protein = data[i];
			protein.Name = "NEWprotein#" + (i + 100).ToString();
			protein.Value = (r.NextDouble() * 100);
		}

		data.Flush();
	}
*/