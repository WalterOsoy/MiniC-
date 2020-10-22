using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using DataStructures;

namespace Clases
{

    class TableParser
    {
        List<Token> tokensList;
        private string path;
        private Dictionary<int, Dictionary<string, string>> table;
        public TableParser(string path, ref List<Token> tokensList)
        {
            this.tokensList = new List<Token>(tokensList);
            this.path = path;
            table = new Dictionary<int, Dictionary<string, string>>();
            readTable();
        }

        public Dictionary<int, Dictionary<string, string>> getTable()
        {
            return table;
        }

        private void readTable()
        {
            using (StreamReader sr = new StreamReader(path))
            {
                Dictionary<string, string> headersTemplate = readHeaders(sr);
                string Line = "";
                int state = 0;

                while ((Line = sr.ReadLine()) != null)
                {
                    table.Add(state, readContent(Line, new Dictionary<string, string>(headersTemplate)));
                    state++;
                }
            }
        }

        private Dictionary<string, string> readHeaders(StreamReader sr)
        {
            string[] headers = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.IgnorePatternWhitespace);
            Dictionary<string, string> rtrnDict = new Dictionary<string, string>();
            foreach (string header in headers) rtrnDict.Add(CheckValue(header), "");
            return rtrnDict;
        }

        private Dictionary<string, string> readContent(string Line, Dictionary<string, string> template)
        {
            string[] lineValues = Line.Split(',');
            int count = 0;
            List<string> keys = new List<string>(template.Keys);

            foreach (var key in keys)
            {
                if (lineValues[count] == "") template[key] = "error";
                else template[key] = lineValues[count];
                count++;
            }

            return template;
        }

        private string CheckValue(string value)
        {
            return (Regex.IsMatch(value, @"([\S]*\,[\S]*)+"))
              ? value.Trim('"')
              : value;
        }
    }
}