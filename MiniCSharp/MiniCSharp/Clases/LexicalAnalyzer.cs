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
            {"double",      new Regex(@"^[0-9]+[.]?[0-9]*")},
            {"hexadecimal", new Regex(@"^0([0-9]*)?[x|X]?[0-9|a-fA-F]*")},
            {"exponet",     new Regex(@"^([0-9]+[.]?[0-9](e|e[+-]|E[+-]|E)?[0-9]+)")},
            { "ComentMulti", new Regex(@"^[/][*].*[*][/]")}
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
                else if (char.IsDigit(inicial))//posible double, hexadecimal or exponent
                {                                                            
                    word = PossibleDigit(word);
                }
                else if (Operators.Contains(inicial.ToString())==true )
                {
                    word = PossibleOperators(word);
                }                
                else 
                {
                    if (!char.IsWhiteSpace(inicial))
                    {
                        fileManager.WriteError(inicial.ToString());
                    }
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
            bool key = false;
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
                        if (keywords.Contains(word.Substring(0, size)))
                        {
                            match = false;
                            key = true;
                            size++;
                        }
                        else
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
                    }
                    else
                    {
                        match = false;
                    }                    
                } while (match);
                if (key==true)
                {
                    fileManager.WriteMatch(word.Substring(0, size - 1), "Palabra Reservada");
                }
                else
                {
                    fileManager.WriteMatch(word.Substring(0, size - 1), "Identificador");
                }
                word = word.Substring(size-1 , word.Length-(size-1) );
            }
            return word;
        }
        public string PossibleOperators(string word)
        {
            if (word.Length > 1)
            {
                if (word.Substring(0, 2)=="//" )
                {
                    word = ComentSingle(word);
                }
                else if (word.Substring(0, 2) == "/*")
                {
                    word = ComentMulti(word);
                }                
                else if (Operators.Contains(word.Substring(0, 2)) == true)
                {
                    fileManager.WriteMatch(word.Substring(0, 2), "Operador");
                    word = word.Remove(0, 2);                    
                }
                else
                {
                    fileManager.WriteMatch(word.Substring(0, 1), "Operador");
                    word = word.Remove(0, 1);
                }
            }
            else
            {
                fileManager.WriteMatch(word.Substring(0, 1), "Operador");
                word = word.Remove(0,1);
            }
            return word;
        }
        public string PossibleDigit(string word)
        {
            var digit = MiniCSharpConstants["double"].Match(word);
            var hexa = MiniCSharpConstants["hexadecimal"].Match(word);
            var expo = MiniCSharpConstants["exponet"].Match(word);
            if (hexa.Length > expo.Length && hexa.Length > digit.Length)
            {
                fileManager.WriteMatch(hexa.Value, "Valor Hexadecimal", hexa.Value);
                word = word.Remove(0,hexa.Length);
            }
            else if (expo.Length>hexa.Length && expo.Length> digit.Length)
            {
                fileManager.WriteMatch(expo.Value, "Valor Exponencial", expo.Value);
                word = word.Remove(0, expo.Length);
            }
            else
            {
                fileManager.WriteMatch(digit.Value, "Valor Decimal", digit.Value);
                word = word.Remove(0, digit.Length);
            }
            return word;
        }
        public string ComentMulti(string word)
        {
            bool end = false;
            do
            {
                string tempo = fileManager.ReadNext();
                if (!tempo.Contains("\r\n"))
                {
                    word += tempo ;
                }                               
                if (MiniCSharpConstants["ComentMulti"].IsMatch(word))
                {
                    end = true;
                    break;
                }
                else
                {
                    word += " ";
                }
            } while (fileManager.sr.EndOfStream != true || end != false );
            if (end == true)
            {
                fileManager.WriteMatch(MiniCSharpConstants["ComentMulti"].Match(word).Value,"Comentario Multilinea");
                word = word.Remove(0, MiniCSharpConstants["ComentMulti"].Match(word).Length);
            }
            else
            {
                fileManager.WriteError("EOF, comentario sin cierre");
                word = "";
            }
            return word;
        }
        public string ComentSingle(string word)
        {
            bool end = false;
            do
            {
                string tempo = fileManager.ReadNext();
                if (!tempo.Contains("\r\n"))
                {
                    word += tempo;
                    end = true;
                    break;
                }                
                else
                {
                    word += tempo + " ";
                }
            } while (fileManager.sr.EndOfStream != true || end != false);
            string[] content = word.Split("\r\n",StringSplitOptions.None);
            fileManager.WriteMatch(content[0], "Comentario simple");
            if(content.Length > 1){
                word = content[1];
            }
            else
            {
                word = "";
            }
            return word;
        }
    }
}