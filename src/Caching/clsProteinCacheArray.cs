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
			ProteinDB = new Database("ProteinDB");
			ProteinDB.Open();
			ProteinDB.CreateTable("proteinList1");
			Fill();
		}

		private void Fill()
		{
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
					data[i].Value1	= Convert.ToDouble(temp[2]);
					data[i].Value2	= Convert.ToDouble(temp[3]);
					data[i].Value3	= Convert.ToDouble(temp[4]);
					data[i].Value4	= Convert.ToDouble(temp[5]);
					data[i].Value5	= Convert.ToDouble(temp[6]);
					data[i].Value6	= Convert.ToDouble(temp[7]);
					data[i].Value7	= Convert.ToDouble(temp[8]);
					data[i].Value8	= Convert.ToDouble(temp[9]);
					data[i].Value9	= Convert.ToDouble(temp[10]);
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
		private double dataValue1;
		private double dataValue2;
		private double dataValue3;
		private double dataValue4;
		private double dataValue5;
		private double dataValue6;
		private double dataValue7;
		private double dataValue8;
		private double dataValue9;
		
		public classProtein()
		{
			Random r = new Random();
			dataValue1 = r.NextDouble();
			dataValue2 = r.NextDouble();
			dataValue3 = r.NextDouble();
			dataValue4 = r.NextDouble();
			dataValue5 = r.NextDouble();
			dataValue6 = r.NextDouble();
			dataValue7 = r.NextDouble();
			dataValue8 = r.NextDouble();
			dataValue9 = r.NextDouble();
			name = "Test";
		}

		public double Value1
		{
			get	{ return dataValue1; }
			
			set	{ dataValue1 = value; }
		}

		public double Value2
		{
			get { return dataValue2; }

			set { dataValue2 = value; }
		}

		public double Value3
		{
			get { return dataValue3; }

			set { dataValue3 = value; }
		}

		public double Value4
		{
			get { return dataValue4; }

			set { dataValue4 = value; }
		}

		public double Value5
		{
			get { return dataValue5; }

			set { dataValue5 = value; }
		}

		public double Value6
		{
			get { return dataValue6; }

			set { dataValue6 = value; }
		}

		public double Value7
		{
			get { return dataValue7; }

			set { dataValue7 = value; }
		}

		public double Value8
		{
			get { return dataValue8; }

			set { dataValue8 = value; }
		}

		public double Value9
		{
			get { return dataValue9; }

			set { dataValue9 = value; }
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