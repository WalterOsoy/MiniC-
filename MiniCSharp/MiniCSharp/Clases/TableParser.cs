using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using DataStructures;
using System.Runtime.InteropServices;

namespace Clases
{

    class TableParser
    {
        List<Token> tokensList;
        Stack<int> stack;
        Stack<Token> symbol;
        string action;
        private string path;
        private Dictionary<int, Dictionary<string, string>> table;
        public TableParser(string path, ref List<Token> tokensList)
        {            
            this.path = path;
            table = new Dictionary<int, Dictionary<string, string>>();
            readTable();
            this.tokensList = new List<Token>(tokensList);
            stack = new Stack<int>();
            symbol = new Stack<Token>();
            action = "";
        }

        public Dictionary<int, Dictionary<string, string>> getTable()
        {
            return table;
        }
        public void parse()
        {
            bool end = false;
            bool complete = false;
            do
            {
                int fila = stack.Peek();
                Token entrada = tokensList[0];
                action = table[fila][entrada.Value];
                switch (action[0])
                {
                    case 's'://desplazamiento
                        Displacement();
                        break;
                    case 'r'://reduccion 
                        Reduction();
                        break;
                    case 'a'://Aceptar
                        complete = true;
                        end = true;
                        break;
                    case 'e'://error
                        complete = false;
                        end = true;
                        break;
                    default://ir a 
                        GoTo();
                        break;
                }
            } while (end != true);
        }
        private void Displacement()
        {   
            
        }
        private void Reduction()
        {

        }
        private void GoTo()
        {

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