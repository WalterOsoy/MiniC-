using System.Runtime.InteropServices.WindowsRuntime;
using System.Reflection.Metadata.Ecma335;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using DataStructures;
using System.Collections.Concurrent;

namespace Clases
{
    class LexicalAnalyzer
    {
        List<string> keywords = new List<string>(){
          "void"     ,     "int"   ,      "double"   ,      "bool"    ,     "string" ,
          "class"    ,     "const" ,      "interface",      "null"    ,     "this"   ,
          "for"      ,     "while" ,      "foreach"  ,      "if"      ,     "else"   ,
          "return"   ,     "break",      "New"      ,      "NewArray",     "Console",
          "WriteLine",     "Print"
          };
        List<string> Operators = new List<string>()
        {
            "+",    "-",    "*",    "/" ,   "%",    "<" ,   "<=",   ">",   ">=",    "=",    "==",   "!=",
            "&&",   "||",   "!" ,   ";" ,   ",",    ".",    "[",    "]",    "(",    ")",    "{",    "}",
            "[]",   "()",   "{}",   ":"  ,  "|",    "&"
        };
        Dictionary<string, Regex> MiniCSharpConstants = new Dictionary<string, Regex>(){
            {"identifier",  new Regex(@"^[a-zA-Z]+\w*")},
            {"boolean",     new Regex(@"true|false")},
            {"double",      new Regex(@"^[0-9]+[.]?[0-9]*")},
            {"hexadecimal", new Regex(@"^0([0-9]*)?[x|X]?[0-9|a-fA-F]*")},
            {"exponet",     new Regex(@"^([0-9]+[.]?[0-9]*(e|e[+-]|E[+-]|E)?[0-9]+)")},
            { "ComentMulti", new Regex(@"^[/][*].*[*][/]")},
            { "string", new Regex("^[\"].*[\"]")},
            { "bool", new Regex("^(true|false)")}

        };
        private bool isCorrect =true;
        FileManager fileManager;

        List<Token> tokenslist = new List<Token>();
        public LexicalAnalyzer(string FilePath) {
            fileManager = new FileManager(FilePath);
        }
        public bool Analize(out List<Token> tokenslist)
        {
            do
            {
                ToAnalyzeWord(fileManager.ReadNext());
            } while (!fileManager.sr.EndOfStream);
            fileManager.Close();
            tokenslist = this.tokenslist;
            return isCorrect;
        }
        /// <summary>
        /// reconoce el primer caracter del string que se resive para poder sacar un analisis presiminar del contenido 
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        private void ToAnalyzeWord(string word)
        {
            bool complete = false;
            do
            {
                char inicial = word[0];
                if (MiniCSharpConstants["bool"].IsMatch(word) == true)
                {
                    fileManager.WriteMatch(MiniCSharpConstants["bool"].Match(word).Value, "booleano");
                    tokenslist.Add(new Token { type = "bool", Value = MiniCSharpConstants["bool"].Match(word).Value , line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                    word = word.Remove(0, MiniCSharpConstants["bool"].Match(word).Length);
                }
                else if (char.IsLetter(inicial))//inicia con un caracter entonces o es una reservada o un id
                {
                    string temp = PossibleKeyWord(word);
                    if (temp !=word)
                    {
                        word = temp;
                    }else if (word.Length != 0)
                    {
                        word = PossibleID(word);
                    }
                }
                else if (char.IsDigit(inicial))//posible double, hexadecimal or exponent
                {
                    word = PossibleDigit(word);
                }
                else if (Operators.Contains(inicial.ToString()) == true)
                {
                    word = PossibleOperators(word);
                }
                else if (inicial == 34)
                {
                    word = text(word);
                }                
                else
                {
                    if (!char.IsWhiteSpace(inicial))
                    {
                        fileManager.WriteError(inicial.ToString());
                    }
                    word = word.Substring(1, word.Length - 1);
                }
                if (word.Length == 0)
                {
                    complete = true;
                }
            } while (complete != true);
        }
        /// <summary>
        /// compara la string resivido contra la lista de palabras reservadas
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        /// <returns>retorna es sobrante de la plara que no hace match</returns>
        private string PossibleKeyWord(string word)
        {

            foreach (var item in keywords)
            {
                if (word.Length < item.Length) 
                    continue;
                string tempo = word.Substring(0,item.Length);
                if (item.Equals(tempo))
                {
                    fileManager.WriteMatch(tempo, "Palabra reservada");
                    tokenslist.Add(new Token { type = tempo, Value = tempo , line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                    word = word.Substring(item.Length);
                    break;
                }
            }
            return word;
        }
        /// <summary>
        /// compara la string resivido contra la expresion regular de identificadores
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        /// <returns>retorna es sobrante de la plara que no hace match</returns>
        private string PossibleID(string word)
        {
            bool key = false;
            bool match = true;
            int size = 1;
            if (MiniCSharpConstants["identifier"].IsMatch(word))
            {
                var id = MiniCSharpConstants["identifier"].Match(word);
                word = word.Remove(0, id.Length);
                if (id.Length>31)
                {
                    fileManager.WriteMatch(id.Value.Substring(0,31)+" ", "Identificador");
                    tokenslist.Add(new Token { type = "ident", Value = id.Value.Substring(0, 31) , line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                    fileManager.WriteError("identificador excede el largo permitido");                    
                }
                else
                {
                    fileManager.WriteMatch(id.Value, "Identificador");
                    tokenslist.Add(new Token { type = "ident", Value = id.Value, line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                }                               
            }
            else
            {
                do
                {
                    if (size <= word.Length)
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
                if (key == true)
                {
                    fileManager.WriteMatch(word.Substring(0, size - 1), "Palabra Reservada");
                    tokenslist.Add(new Token { type = word.Substring(0, size - 1), Value = word.Substring(0, size - 1), line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2] });
                }
                else
                {
                    fileManager.WriteMatch(word.Substring(0, size - 1), "Identificador");
                    tokenslist.Add(new Token { type = "ident", Value = word.Substring(0, size - 1), line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2] });
                }
                word = word.Substring(size - 1, word.Length - (size - 1));
            }
            return word;
        }
        /// <summary>
        /// compara la string resivido contra la lista de expresiones regulares
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        /// <returns>retorna es sobrante de la plara que no hace match</returns>
        private string PossibleOperators(string word)
        {
            if (word.Length > 1)
            {
                if (word.Substring(0, 2) == "//")
                {
                    word = ComentSingle(word);
                }
                else if (word.Substring(0, 2) == "/*")
                {
                    word = ComentMulti(word);
                }
                else if (word.Substring(0, 2) == "*/")
                {
                    word = word.Remove(0, 2);
                    fileManager.WriteError("comentario sin iniciar");
                    isCorrect=false;
                }
                else if (Operators.Contains(word.Substring(0, 2)) == true)
                {
                    fileManager.WriteMatch(word.Substring(0, 2), "Operador");
                    tokenslist.Add(new Token { type = word.Substring(0,2), Value = word.Substring(0,2), line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                    word = word.Remove(0, 2);
                }
                else
                {
                    fileManager.WriteMatch(word.Substring(0, 1), "Operador");
                    tokenslist.Add(new Token { type = word.Substring(0, 1), Value = word.Substring(0, 1), line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2] });
                    word = word.Remove(0, 1);
                }
            }
            else
            {
                fileManager.WriteMatch(word.Substring(0, 1), "Operador");
                tokenslist.Add(new Token { type = word.Substring(0, 1), Value = word.Substring(0, 1), line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2] });
                word = word.Remove(0, 1);
            }
            return word;
        }
        /// <summary>
        /// compara la string resivido contra las 3 expresion regular de numeros para identificar a cual pertenece
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        /// <returns>retorna es sobrante de la plara que no hace match</returns>
        private string PossibleDigit(string word)
        {
            var digit = MiniCSharpConstants["double"].Match(word);
            var hexa = MiniCSharpConstants["hexadecimal"].Match(word);
            var expo = MiniCSharpConstants["exponet"].Match(word);
            if (hexa.Length > expo.Length && hexa.Length > digit.Length)
            {
                fileManager.WriteMatch(hexa.Value, "Valor Hexadecimal", hexa.Value);
                tokenslist.Add(new Token { type = "doubleConstant", Value = hexa.Value , line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                word = word.Remove(0, hexa.Length);
            }
            else if (expo.Length > hexa.Length && expo.Length > digit.Length)
            {
                fileManager.WriteMatch(expo.Value, "Valor Exponencial", expo.Value);
                tokenslist.Add(new Token { type = "doubleConstant", Value = expo.Value , line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                word = word.Remove(0, expo.Length);
            }
            else
            {
                fileManager.WriteMatch(digit.Value, "Valor Decimal", digit.Value);
                tokenslist.Add(new Token { type = "intConstant", Value = digit.Value , line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                word = word.Remove(0, digit.Length);
            }
            return word;
        }
        /// <summary>
        /// compara la string resivido contra la expresion regular de Comentario multiple
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        /// <returns>retorna es sobrante de la plara que no hace match</returns>
        private string ComentMulti(string word)
        {
            bool end = false;
            do
            {
                string tempo = fileManager.ReadNext();
                if (!tempo.Contains("\r\n"))
                {
                    word += tempo;
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
            } while (fileManager.sr.EndOfStream != true || end != false);
            if (end == true)
            {
                //fileManager.WriteMatch(MiniCSharpConstants["ComentMulti"].Match(word).Value, "Comentario Multilinea");
                word = word.Remove(0, MiniCSharpConstants["ComentMulti"].Match(word).Length);
            }
            else
            {
                fileManager.WriteError("EOF, comentario sin cierre");
                isCorrect=false;
                word = "";
            }
            return word;
        }
        /// <summary>
        /// compara la string resivido contra la expresion regular de Comentario simple
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        /// <returns>retorna es sobrante de la plara que no hace match</returns>
        private string ComentSingle(string word)
        {
            bool end = false;
            do
            {
                string tempo = fileManager.ReadNext();
                if (tempo.Contains("\r\n"))
                {
                    word += " " + tempo;
                    end = true;
                    break;
                }
                else
                {
                    word += " " + tempo;
                }
            } while (fileManager.sr.EndOfStream != true || end != false);
            string[] content = word.Split("\r\n", StringSplitOptions.None);
            //fileManager.WriteMatch(content[0], "Comentario simple");
            if (content.Length > 1) {
                word = content[1];
            }
            else
            {
                word = "";
            }
            return word;
        }
        /// <summary>
        /// compara la string resivido contra la expresion regular de string
        /// </summary>
        /// <param name="word">palabra leia del archivo</param>
        /// <returns>retorna es sobrante de la plara que no hace match</returns>
        private string text(string word)                    
        {
            do
            {
                string tempo = fileManager.ReadNext();
                if (tempo.Contains("\""))
                {        
                    word += " " + tempo;
                    break;
                }
                else if (tempo.Contains("\r\n"))
                {
                    word += " "+tempo;
                    break;
                }
                else
                {
                    word += " " + tempo;
                }
            } while (fileManager.sr.EndOfStream != true);
            if (MiniCSharpConstants["string"].IsMatch(word)==true)
            {
                fileManager.WriteMatch(MiniCSharpConstants["string"].Match(word).Value, "Cadena de texto");
                tokenslist.Add(new Token { type = "stringConstant", Value = MiniCSharpConstants["string"].Match(word).Value , line = fileManager.LineInfo[0].ToString(),column = fileManager.LineInfo[1]+","+fileManager.LineInfo[2]});
                word = word.Remove(0, MiniCSharpConstants["string"].Match(word).Length);
            }
            else
            {
                fileManager.WriteError("Strings sin terminar, falta una \" de cierre");
                isCorrect=false;
                word = "";
            }
            word.Trim();
            return word;
        }
    }
}