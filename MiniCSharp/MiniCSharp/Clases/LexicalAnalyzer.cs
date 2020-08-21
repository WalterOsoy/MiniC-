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
          fileManager.WriteMatch(fileManager.ReadNext(), "Saludo jajaja", "Testing the other method jajaja");
          fileManager.WriteMatch(fileManager.ReadNext(), "Los PIBES", "X2");
          fileManager.ReadNext();
          fileManager.WriteMatch(fileManager.ReadNext(), "IO");
          fileManager.WriteMatch(fileManager.ReadNext(), "UST");
          fileManager.ReadNext();
          fileManager.WriteMatch(fileManager.ReadNext(), "Vulgaridad xdxd", "X3");
          fileManager.ReadNext();
          fileManager.ReadNext();
          fileManager.WriteError(fileManager.ReadNext());
          fileManager.ReadNext();
          fileManager.ReadNext();
          fileManager.WriteMatch(fileManager.ReadNext(), "autismo");

          fileManager.Close();
        }
    }
}