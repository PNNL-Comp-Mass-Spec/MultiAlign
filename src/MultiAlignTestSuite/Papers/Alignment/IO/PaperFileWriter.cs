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

        public PaperFileWriter(string name, string path)
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
            m_writer = File.CreateText(Path);
        }

        protected void Close()
        {
            m_writer.Close();
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
