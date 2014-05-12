#region

using System;
using System.IO;

#endregion

namespace MultiAlignTestSuite.Papers.Alignment.IO
{
    /// <summary>
    ///     Class for writing
    /// </summary>
    public abstract class PaperFileWriter
    {
        private TextWriter m_writer;
        private bool m_isOpen;
        private readonly bool m_shouldAppend;

        public PaperFileWriter(bool append)
        {
            m_shouldAppend = append;
        }

        ~PaperFileWriter()
        {
            try
            {
                Close();
            }
            catch
            {
            }
        }

        public PaperFileWriter(string name, string path, bool append)
            : this(append)
        {
            var now = DateTime.Now;
            var newName = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}.csv", name,
                now.Year,
                now.Month,
                now.Day,
                now.Hour,
                now.Minute,
                now.Second);

            Path = System.IO.Path.Combine(path, newName);
        }


        public string Path { get; protected set; }

        protected void Open()
        {
            if (!m_isOpen)
            {
                if (m_shouldAppend)
                    m_writer = File.AppendText(Path);
                else
                    m_writer = File.CreateText(Path);
            }
            m_isOpen = true;
        }

        public void Close()
        {
            if (m_isOpen)
            {
                m_writer.Flush();
                m_writer.Close();
            }
            m_isOpen = false;
        }

        public virtual void WriteLine(string line)
        {
            m_writer.WriteLine(line);
            Console.WriteLine(line);
        }

        public virtual void WriteLine()
        {
            WriteLine("");
        }
    }
}