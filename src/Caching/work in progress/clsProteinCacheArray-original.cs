using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace PNNLControls.Data
{
	public class clsProteinCacheArray : clsCacheArray<classProtein>, IDisposable
	{
		private Database ProteinDB;

		public clsProteinCacheArray(long cacheSize, long arraySize):
		base(cacheSize, arraySize)
		{
			ProteinDB = new Database("ProteinDB-test");
			ProteinDB.Open();
			ProteinDB.CreateTable("proteinList1");
			Fill();
			//Console.WriteLine("AT CREATION:");
			//ProteinDB.Output("proteinList1");
			//Console.WriteLine();
		}

		private void Fill()
		{
			/*for(long i = 0; i < mlng_arrayLength/1000; i++)
			{
				
				string tempParameter = null;
				for (long j = 0; j < 1000; j++)
				{
					tempParameter += "('protein" + (j+1).ToString() + "', 0.00)";

					if (j < 999)
					{ tempParameter += " INSERT INTO proteinList1(name, value) VALUES "; }
				}
				
				ProteinDB.InsertInto("proteinList1", tempParameter);
			}*/

			ProteinDB.InsertInto("proteinList1", "NULLprotein", mlng_arrayLength);
		}


		#region Cache
		/// <summary>
		/// Writes data from buffer into database.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="startIndex"></param>
		/// <param name="length"></param>
		protected override void Cache(classProtein[] data, long startIndex, long length)
		{
			ProteinDB.UpdateCache(data, startIndex, length);


			/*for (int i = 0; i < length; i++)
			{
				ProteinDB.Update(	"proteinList1",
									"name='" + data[i].Name + "', value=" + data[i].Value,
									"tableIndex=" + Convert.ToString(startIndex + i + 1));
			}*/
		}
		#endregion



		#region Retrieve
		/// <summary>
		/// Takes data from database and puts it into buffer.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="length"></param>
		protected override void Retrieve(classProtein[] data, long index, long length)
		{
			string[] results = ProteinDB.SelectWhere("*", "proteinList1", "tableIndex>=" + (index + 1) + " AND tableIndex<" + (index + length + 1));

			for (int i = 0; i < length && index + i < mlng_arrayLength; i++)
			{
				if (i < results.Length)
				{
					string[] temp	= Regex.Split(results[i], ", ");
					data[i]			= new classProtein();
					data[i].Name	= temp[1];
					data[i].Value	= Convert.ToDouble(temp[2]);
				}
				else
				{
					throw new IndexOutOfRangeException("Index was outside the bounds of the array.");
				}
			}
		}
		#endregion


		public void PrintIt()
		{
			ProteinDB.Output("proteinList1");
		}
	
#region IDisposable Members

		
public override void  Dispose()
{
	base.Dispose();	
	ProteinDB.Close();
}

#endregion
}

	public class classProtein
	{
		private string name;
		private double dataValue;
		
		public classProtein()
		{
			Random r = new Random();
			dataValue = r.NextDouble();
			name = "Test";
		}

		public double Value
		{
			get	{ return dataValue; }
			
			set	{ dataValue = value; }
		}

		public string Name
		{
			get	{ return name; }
			
			set	{ name = value; }
		}
	}
}





/* TO READ THE WHOLE TEXT FILE
StreamReader textFile = File.OpenText("myArrayTextFile.txt");

string [] textFileData = Regex.Split(textFile.ReadToEnd(), "\n\r");

string[] goodData = new string[3];

for (int i = 0; i < length; i++)
{
	goodData = Regex.Split(textFileData[i], ", ");
	/// gooddata = "index, name, value"
	/// 0, 1, 2
	data[Convert.ToInt32(goodData[0])].Value = Convert.ToDouble(goodData[2]);
	data[Convert.ToInt32(goodData[0])].Name = goodData[1];
}

textFile.Close();
*/