using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace PNNLControls.Data
{
	public class clsSerializableProteinCacheArray : clsCacheArray<serializableProtein>, IDisposable
	{
		private BinaryFormatter binFormatter = new BinaryFormatter();

		private string dataFileName;

		private Hashtable offsetHT = new Hashtable();

		public clsSerializableProteinCacheArray(string tempDataFileName, long cacheSize):
		base(cacheSize, arraySize)
		{
			dataFileName = tempDataFileName;
			Fill(dataFileName);
		}

		private void Fill(string dataFileName)
		{
			using (FileStream serializationStream = new FileStream(dataFileName, FileMode.Create, FileAccess.Write))
			{
				//using (StreamWriter textFile = File.CreateText("offsetText.txt"))
				//{
					for (long i = 0; i < mlng_arrayLength; i++)
					{
						serializableProtein tempProtein = new serializableProtein();
						tempProtein.Name = "TestProtein";
						tempProtein.Value1 = 0;
						//textFile.WriteLine("{0}, {1}", i, serializationStream.Position);
						offsetHT.Add(i, serializationStream.Position);
						binFormatter.Serialize(serializationStream, tempProtein);
					}
					serializationStream.Close();
					//textFile.Close();
				//}
			}
		}


		#region Cache
		/// <summary>
		/// Writes data from buffer into database.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="startIndex"></param>
		/// <param name="length"></param>
		protected override void Cache(serializableProtein[] data, long startIndex, long length)
		{
			FileStream serializationStream = new FileStream(dataFileName, FileMode.Open, FileAccess.Write);
			long streamOffset = Convert.ToInt64(offsetHT[startIndex]);
			serializationStream.Seek(streamOffset, SeekOrigin.Begin);

			for (long i = 0; i < length; i++)
			{
				serializableProtein tempProtein = new serializableProtein();
				tempProtein.Name = data[i].Name;
				tempProtein.Value1 = data[i].Value1;
				if (serializationStream.Position + 160 > Convert.ToInt64(offsetHT[startIndex + i + 1]))
				{
					serializationStream.Close();
					writeToEndOfFile(tempProtein, startIndex, i);
					serializationStream = new FileStream(dataFileName, FileMode.Open, FileAccess.Write);
				}
				else
				{
					offsetHT[startIndex + i] = serializationStream.Position;
					binFormatter.Serialize(serializationStream, tempProtein);
				}
			}
			serializationStream.Close();
			serializationStream.Dispose();
		}
		#endregion



		#region Retrieve
		/// <summary>
		/// Takes data from database and puts it into buffer.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="length"></param>
		protected override void Retrieve(serializableProtein[] data, long index, long length)
		{
			using (FileStream serializationStream = new FileStream(dataFileName, FileMode.Open, FileAccess.Read))
			{
				long streamOffset = Convert.ToInt64(offsetHT[index]);//readOffsetFromText(index);

				for (long i = 0; i < length; i++)
				{
					serializationStream.Seek(streamOffset, SeekOrigin.Begin);
					data[i] = (serializableProtein)binFormatter.Deserialize(serializationStream);
					streamOffset = Convert.ToInt64(offsetHT[index + i]);
				}
				serializationStream.Close();
			}
		}


		#region CODE FROM GORDON SYLSZ
		/*
		public class IsosResultDeSerializer
		{
			private string binaryDataFilename;
			FileStream stream;
			long streamPosition;

			public IsosResultDeSerializer(string binaryDataFilename)
			{
				this.binaryDataFilename = binaryDataFilename;
				try
				{
					stream = new FileStream(binaryDataFilename, FileMode.Open, FileAccess.Read);

				}
				catch (Exception ex)
				{
	                
					throw new System.IO.IOException("De-serializer could not find temporary binary file. Details: "+ex.Message);
				}
			}


			public ResultCollection GetNextSetOfResults()
			{
				ResultCollection resultcollection= null;
				BinaryFormatter formatter = new BinaryFormatter();

				if (streamPosition < stream.Length)
				{
					stream.Seek(streamPosition, SeekOrigin.Begin);
					resultcollection = (ResultCollection)formatter.Deserialize(stream);
					streamPosition = stream.Position;
				}

				return resultcollection;

			}


			internal void Close()
			{
				try
				{
					stream.Close();
				}
				catch (Exception ex)
				{
	                
					throw new System.IO.IOException("Deserializer couldn't close the binary stream. Details: " + ex.Message);
				} 
			}
		}
		*/
		#endregion
		#endregion

		#region readOffsetFromText
		/*public long readOffsetFromText(long proteinNumber)
		{
			string[] goodData = new string[2];

			using (StreamReader textFile = File.OpenText("offsetText.txt"))
			{
				string[] textFileData = Regex.Split(textFile.ReadToEnd(), "\r\n");

				for (int i = 0; i < textFileData.Length; i++)
				{
					goodData = Regex.Split(textFileData[i], ", ");

					if (goodData[0] == proteinNumber.ToString())
					{
						textFile.Close();
						break;
					}
				}
			}
			return Convert.ToInt64(goodData[1]);
		}*/
		#endregion

		public void writeToEndOfFile(serializableProtein tempProtein, long startIndex, long i)
		{
			using (FileStream serializationStream = new FileStream(dataFileName, FileMode.Append, FileAccess.Write))
			{
				offsetHT[startIndex + i] = serializationStream.Position;
				binFormatter.Serialize(serializationStream, tempProtein);
			}
		}
	
#region IDisposable Members

		
public override void  Dispose()
{
	base.Dispose();	
}

#endregion
}

	[Serializable()]
	public class serializableProtein : ISerializable
	{
		private string name;
		private double dataValue1;
		/* EXTRA PROPERTIES
		private double dataValue2;
		private double dataValue3;
		private double dataValue4;
		private double dataValue5;
		private double dataValue6;
		private double dataValue7;
		private double dataValue8;
		private double dataValue9;
		 */


		public serializableProtein()
		{
		}


		public serializableProtein(SerializationInfo info, StreamingContext ctxt)
		{
			name = (String)info.GetValue("Name", typeof(string));
			dataValue1 = (double)info.GetValue("Value1", typeof(double));

			#region EXTRA PROPERTIES
			/*name = "Test";
			Random r = new Random();
			dataValue1 = r.NextDouble();
			dataValue2 = r.NextDouble();
			dataValue3 = r.NextDouble();
			dataValue4 = r.NextDouble();
			dataValue5 = r.NextDouble();
			dataValue6 = r.NextDouble();
			dataValue7 = r.NextDouble();
			dataValue8 = r.NextDouble();
			dataValue9 = r.NextDouble();*/
			#endregion
		}


		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			
			info.AddValue("Name", name);
			info.AddValue("Value1", dataValue1);
		}


		public string Name
		{
			get { return name; }

			set { name = value; }
		}

		public double Value1
		{
			get	{ return dataValue1; }
			
			set	{ dataValue1 = value; }
		}

		/* EXTRA PROPERTIES
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
		*/
	}
}