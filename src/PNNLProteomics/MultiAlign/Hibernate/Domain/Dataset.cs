using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace PNNLProteomics.MultiAlign.Hibernate.Domain
{
	public class Dataset : ISerializable
	{
		private int m_datasetId;
		private int m_job;
		private string m_fileName;
		private string m_parameter_file;
		private string m_jobDateTime;
		private string m_directoryPath;
		private string m_jobType;

		public Dataset()
		{
		}

		public Dataset(int datasetId, int job)
		{
			m_datasetId = datasetId;
			m_job = job;
		}

		#region ISerializable Members

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		public override bool Equals(object obj)
		{
			Dataset dataset = (Dataset)obj;

			if (dataset == null)
			{
				return false;
			}
			else if (!this.DatasetId.Equals(dataset.DatasetId))
			{
				return false;
			}
			else
			{
				return this.Job.Equals(dataset.Job);
			}
		}

		public override int GetHashCode()
		{
			int hash = 17;

			hash = hash * 23 + m_datasetId.GetHashCode();
			hash = hash * 23 + m_job.GetHashCode();

			return hash;
		}

		public int DatasetId
		{
			get { return m_datasetId; }
			set { m_datasetId = value; }
		}

		public int Job
		{
			get { return m_job; }
			set { m_job = value; }
		}

		public string FileName
		{
			get { return m_fileName; }
			set { m_fileName = value; }
		}

		public string ParameterFile
		{
			get { return m_parameter_file; }
			set { m_parameter_file = value; }
		}

		public string JobDateTime
		{
			get { return m_jobDateTime; }
			set { m_jobDateTime = value; }
		}

		public string DirectoryPath
		{
			get { return m_directoryPath; }
			set { m_directoryPath = value; }
		}

		public string JobType
		{
			get { return m_jobType; }
			set { m_jobType = value; }
		}
	}
}
