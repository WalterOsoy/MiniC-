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
          {"exponet",     new Regex(@"([0-9]+[.]?[0-9](e|e[+-]|E[+-]|E)?[0-9])")}
        };
        FileManager fileManager;
        public LexicalAnalyzer(string FilePath){
          fileManager = new FileManager(FilePath);
        }

        public void ToAnalyze(string word, bool isNewLine)
        {
            word = "Perro(if";
            bool complete = false;
            do
            {
                char inicial = word[0];
                if (char.IsLetter(inicial))//inicia con un caracter entonces o es una reservada o un id
                {
                    word = PossibleKeyWord(word);
                }
                else if (inicial.Equals('/') && word.Length > 1) //posible comentario
                {

                }
                else if (char.IsDigit(inicial))//posible double, hexadecimal or exponent
                {

                }
                else //error
                {

                }
                if (word.Length==0)
                {
                    complete = true;
                }
            } while (complete!=true);

        }
        public string PossibleKeyWord(string word)
        {
            var words = keywords.Contains(word);
            fileManager.WriteMatch(word, "Palabra reservada");
            word = "";
            return word;
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