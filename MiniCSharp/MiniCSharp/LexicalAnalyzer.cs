using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniCSharp
{
    class LexicalAnalyzer
    {
        List<string> reserved = new List<string>()
        { "void", "int", "double", "bool", "string",
          "class", "const", "interface", "null" ,"this" ,"for",
          "while", "foreach", "if", "else" ,"return" ,"breack",
          "New", "NewArray", "Console", "WriteLine" };
        Regex id = new Regex(@"[a-zA-Z]+[a-zA-z|_|0-9]*");
        Regex ndouble = new Regex(@"^[0-9]+[.]?[0-9]*$");
        //^[0-9]+[.]?[0-9]*(E[+]|E)[0-9]+$
        Regex heza = new Regex(@"([0-9]+[.]?[0-9](e|e[+]|E[+]|E)?[0-9])");
        Regex hexa = new Regex(@"0([0-9]*)?[x|X]?[0-9]*[a-fA-F]*");
        public void ToAnalyze()
        {
            string s = "sapo sapo_9 9sapo";
            var b = id.Match(s);
            var c = id.Matches(s);

            string f = ".12 12.5 12. 12.E2 12.e+2";
            var g = heza.Matches(f);
        }
    }
}