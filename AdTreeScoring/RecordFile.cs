using System;
using System.Collections.Generic;
using System.IO;

namespace Datastructures
{
    class RecordFile
    {
        public RecordFile()
        {
            header = new List<string>();
            record = new List<List<string>>();
            hasHeader = false;
        }

        public void ReadRecord(string filePath, bool hasHeader = true, char delimiter = ',')
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

            this.hasHeader = hasHeader;
            if (hasHeader && !sr.EndOfStream)
            {
                string header_line = sr.ReadLine();
                header = new List<string>(header_line.Split(delimiter));
            }

            while (sr.EndOfStream == false)
            {
                string read_line = sr.ReadLine();
                List<string> line = new List<string>(read_line.Split(delimiter));
                Record.Add(line);
            }
            sr.Close();

        }

        public void Print()
        {
            // ヘッダの出力
            Console.WriteLine(string.Join(",", Header));

            // データの出力
            Record.ForEach(line => Console.WriteLine(string.Join(",", line)));

        }


        private List<string> header;
        public List<string> Header
        {
            get
            {
                return header;
            }
        }

        private List<List<string>> record;
        public List<List<string>> Record
        {
            get
            {
                return record;
            }
        }

        private bool hasHeader;
        public bool HasHeader
        {
            get
            {
                return hasHeader;
            }
        }

    }
}
