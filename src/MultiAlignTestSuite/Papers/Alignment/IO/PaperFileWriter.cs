using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MultiAlignTestSuite.Papers.Alignment.IO
{

    /// <summary>
    /// Class for writing 
    /// </summary>
    public abstract class PaperFileWriter
    {
        private TextWriter m_writer;
        private bool m_isOpen = false;
        private bool m_shouldAppend;
        public PaperFileWriter(bool append)
        {
            m_shouldAppend = append;
        }

        public PaperFileWriter(string name, string path, bool append)
            :this(append)
        {            
            DateTime now    = DateTime.Now;
            string newName  = string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}.csv", name,
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

        protected void Close()
        {
            if (m_isOpen)
                m_writer.Close();
            m_isOpen = false;
        }

        protected void WriteLine(string line)
        {
            m_writer.WriteLine(line);
            Console.WriteLine(line);
        }
        protected void WriteLine()
        {
            WriteLine("");
        }

    }
}
