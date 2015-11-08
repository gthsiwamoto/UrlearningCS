using System;
using System.Collections.Generic;
using System.IO;

namespace DataStructures
{
    class RecordFile
    {
        public RecordFile()
        {
            this.header = new List<string>();
            this.record = new List<List<string>>();
        }

        ~RecordFile() { }

        public void readRecord(string filePath, bool hasHeader = true, char delim = ',')
        {
            StreamReader sr;
            try
            {
                sr = new StreamReader(filePath);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }

            if (hasHeader && !sr.EndOfStream)
            {
                string header_line = sr.ReadLine();
                header = new List<string>(header_line.Split(delim));
            }

            int idx = 0;
            while (sr.EndOfStream == false)
            {
                string read_line = sr.ReadLine();
                List<string> line = new List<string>(read_line.Split(delim));
                record.Add(line);
            }
            sr.Close();

        }

        public void print()
        {
            // ヘッダの出力
            Console.WriteLine(string.Join(",", header));

            // データの出力
            record.ForEach(line => Console.WriteLine(string.Join(",", line)));

        }

        private List<string> header;
        private List<List<string>> record;

    }
}
