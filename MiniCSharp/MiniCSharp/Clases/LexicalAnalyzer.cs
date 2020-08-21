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
        List<string> Operators = new List<string>()
        {
            "+",    "-",    "*",    "/" ,   "%",    "<" ,   "<=",   ">",   ">=",    "=",    "==",   "!=",
            "&&",   "||",   "!" ,   ";" ,   ",",    ".",    "[",    "]",    "(",    ")",    "{",    "}",    
            "[]",   "()",   "{}"
        };
        Dictionary<string, Regex> MiniCSharpConstants = new Dictionary<string, Regex>(){
          {"identifier",  new Regex(@"^[a-zA-Z]+\w*$")},
          {"boolean",     new Regex(@"true|false")},
          {"double",      new Regex(@"^[0-9]+[.]?[0-9]*$")},
          {"hexadecimal", new Regex(@"0([0-9]*)?[x|X]?[0-9|a-fA-F]*")},
          {"exponet",     new Regex(@"([0-9]+[.]?[0-9](e|e[+-]|E[+-]|E)?[0-9])")}
        };
        FileManager fileManager;
        public LexicalAnalyzer(string FilePath){
          fileManager = new FileManager(FilePath);
        }
        public void Analize()
        {            
            do
            {
                ToAnalyzeWord(fileManager.ReadNext());
            } while (!fileManager.sr.EndOfStream);
            fileManager.Close();
        }
        private void ToAnalyzeWord(string word)
        {            
            bool complete = false;
            bool PendingComentSingle = false;
            bool PendingComentMulti = false;
            do
            {
                char inicial = word[0];
                if (char.IsLetter(inicial))//inicia con un caracter entonces o es una reservada o un id
                {
                    word = PossibleKeyWord(word);
                    if (word.Length !=  0)
                    {
                        word = PossibleID(word);
                    }
                }
                else if (inicial.Equals('/') && word.Length > 1) //posible comentario
                {

                }
                else if (char.IsDigit(inicial))//posible double, hexadecimal or exponent
                {

                }
                else if (Operators.Contains(inicial.ToString())==true)
                {

                }
                else if (word.Equals("\r\n"))
                {

                }
                else //error
                {
                    fileManager.WriteError(inicial.ToString());
                    word = word.Substring(1,word.Length-1);
                }
                if (word.Length==0)
                {
                    complete = true;
                }
            } while (complete!=true);

        }
        public string PossibleKeyWord(string word)
        {            
            if (keywords.Contains(word))
            {
                fileManager.WriteMatch(word, "Palabra reservada");
                word = "";
            }                        
            return word;
        }
        public string PossibleID(string word)
        {
            bool match = true;
            int size = 1;            
            if (MiniCSharpConstants["identifier"].IsMatch(word))
            {
                fileManager.WriteMatch(word, "Identificador");
                word = "";
            }
            else
            {
                do
                {
                    if (size <= word.Length )
                    {                        
                        if (!MiniCSharpConstants["identifier"].IsMatch(word.Substring(0, size)))
                        {
                            match = false;
                        }
                        else
                        {
                            size++;
                        }
                    }
                    else
                    {
                        match = false;
                    }                    
                } while (match);
                fileManager.WriteMatch(word.Substring(0, size-1),"Identificador");                
                word = word.Substring(size-1 , word.Length-(size-1) );
            }
            return word;
        }        
    }
}