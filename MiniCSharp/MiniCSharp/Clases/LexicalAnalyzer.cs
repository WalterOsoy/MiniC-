using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Clases
{
    class LexicalAnalyzer
    {
        List<string> keywords = new List<string>(){
          "void"     ,     "int"   ,      "double"   ,      "bool"    ,     "string" ,
          "class"    ,     "const" ,      "interface",      "null"    ,     "this"   ,
          "for"      ,     "while" ,      "foreach"  ,      "if"      ,     "else"   ,
          "return"   ,     "breack",      "New"      ,      "NewArray",     "Console", 
          "WriteLine"
          };
        Dictionary<string, Regex> MiniCSharpConstants = new Dictionary<string, Regex>(){
          {"identifier",  new Regex(@"[a-zA-Z]+\w*")},
          {"boolean",     new Regex(@"true|false")},
          {"double",      new Regex(@"^[0-9]+[.]?[0-9]*$")},
          {"hexadecimal", new Regex(@"0([0-9]*)?[x|X]?[0-9|a-fA-F]*")},
          {"exponet",     new Regex(@"([0-9]+[.]?[0-9](e|e[+]|E[+]|E)?[0-9])")}
        };
        FileManager fileManager;
        public LexicalAnalyzer(string FilePath){
          fileManager = new FileManager(FilePath);
        }

        public void ToAnalyze()
        {
            string s = "sapo sapo_9 9sapo";
            var b = MiniCSharpConstants["identifier"].Match(s);
            var c = MiniCSharpConstants["identifier"].Matches(s);

            string f = ".12 12.5 12. 12.E2 12.e+2";
            var g = MiniCSharpConstants["exponet"].Matches(f);
        }

        public void Analize(){
          fileManager.WriteMatch(CallNTimes(4), "Saludo jajaja", "Testing the other method jajaja");
          fileManager.WriteMatch(CallNTimes(5), "Los PIBES", "X2");
          fileManager.WriteMatch(CallNTimes(2), "IO");
          fileManager.WriteMatch(CallNTimes(2), "UST");
          fileManager.WriteMatch(CallNTimes(10), "Vulgaridad xdxd", "X3");
          fileManager.WriteError(CallNTimes(1));
          fileManager.WriteMatch(CallNTimes(4), "autismo");

          fileManager.Close();
        }

        public string CallNTimes(int Times){
          string myString = "";
          for (int i = 0; i < Times; i++){
            myString += fileManager.ReadChar();
          }
          return myString;
        }
    }
}