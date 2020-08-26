using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Clases
{
  /// <summary>Manages how to read the input file and how to write the output file.</summary>
  class FileManager
  {
    #region Variables and builder

    public  StreamReader sr;
    private StreamWriter sw;
    private Dictionary<string, int> LastMatch;


    /// <summary>
    /// Manages how to read the input file and how to write the output file.<br/>
    /// Output file will be the same path as input but with .out extension
    /// </summary>
    /// <param name="FilePath">Imput file that its going to be readed.</param>
    public FileManager(string FilePath){
      sr = new StreamReader(FilePath, Encoding.UTF8);
      sw = new StreamWriter(new FileStream(FilePath.Replace("frag", "out"), FileMode.Create), Encoding.UTF8);
      LastMatch = new Dictionary<string, int>(){
        {"Line", 1},
        {"BeginingCol", 1}
      };
    }

    #endregion


    #region  Public Functions

    /// <summary>Reads the next word on the file</summary>
    /// <returns>Next analizable char</returns>
    public string ReadNext(){
      EatWhiteSpaces((char)sr.Peek());
      return ReadNextWord((char)sr.Peek());
    }



    /// <summary>Writes a new line on the file with a default format<br/>and the requested params</summary>
    /// <param name="MatchedString">String that was classified</param>
    /// <param name="Type">Type of the string that was classified</param>
    public void WriteMatch(string MatchedString, string Type){
      string line = BuildMatchString(MatchedString, Type);
      UpdateNewMatchPosition(false, MatchedString.Length);
      sw.WriteLine(line);
    }



    /// <summary>Writes a new line on the file with a default format<br/>and the requested params</summary>
    /// <param name="MatchedString">String that was classified</param>
    /// <param name="Type">Type of the string that was classified</param>
    /// <param name="value">Value of the string that was classified</param>
    public void WriteMatch(string MatchedString, string Type, string value){
      string line = BuildMatchString(MatchedString, Type, value);
      UpdateNewMatchPosition(false, MatchedString.Length);
      sw.WriteLine(line);
    }



    /// <summary>Writes a new line on the file with an error format</summary>
    /// <param name="UndefinedCharacter">Character that doesnt fit in any type</param>
    public void WriteError(string UndefinedCharacter){
      string line = BuildErrorString(UndefinedCharacter);
      UpdateNewMatchPosition(false, UndefinedCharacter.Length);
      sw.WriteLine(line);
    }



    /// <summary>Closes the streams used to read and write the files</summary>
    public void Close(){
      sr.Close();
      sw.Close();
    }
    
    #endregion


    #region  Private Functions

    /// <summary>Builds the string that it's going to be written on the file with a default format.</summary>
    /// <param name="MatchedString">String that was classified</param>
    /// <param name="Type">Type of the string that was classified</param>
    /// <returns>A formatted string with data</returns>
    private string BuildMatchString(string MatchedString, string Type){
      return String.Format(
        "{0}encontrado en la linea {1}, en el rango de columnas {2}-{3}, de tipo {4}", 
        MatchedString.PadRight(12, ' '),
        LastMatch["Line"], 
        LastMatch["BeginingCol"],
        LastMatch["BeginingCol"] + MatchedString.Length - 1,
        Type
      );
    }



    /// <summary>Builds the string that it's going to be written on the file with a default format.</summary>
    /// <param name="MatchedString">String that was classified</param>
    /// <param name="Type">Type of the string that was classified</param>
    /// <param name="value">Value of the matched string</param>
    /// <returns>A formatted string with data</returns>
    private string BuildMatchString(string MatchedString, string Type, string value){
      return String.Format(
        "{0} (valor = {1})",
        BuildMatchString(MatchedString, Type),
        value
      );
    }
    


    /// <summary>Builds the error string that it's going to be written on the file with a default format.</summary>
    /// <param name="UndefinedCharacter">Undefined char that causes conflict on the grammar</param>
    /// <returns>A formatted string with data</returns>
    private string BuildErrorString(string UndefinedCharacter){
      return String.Format(
        "*** Error in line {0}*** Unrecognized char: {1} at col {2}",
        LastMatch["Line"], 
        UndefinedCharacter, 
        LastMatch["BeginingCol"]
      );
    }



    /// <summary>Updates the beginning column for the next match when a match it's written on a file.</summary>
    /// <param name="itsNewLine">indicates if the previous match that was done ist a new line</param>
    /// <param name="MatchLenght">Length that we will move the beginning column to match the next match</param>
    private void UpdateNewMatchPosition(bool itsNewLine, int MatchLenght){
      if(itsNewLine){
        LastMatch["Line"]++;
        LastMatch["BeginingCol"] = 1;
      }
      else LastMatch["BeginingCol"] = LastMatch["BeginingCol"] + MatchLenght;
    }
    


    /// <summary>Reads the next word in the file, new line separators count as a words</summary>
    /// <param name="nextChar">next char to evalueate if its a white space</param>
    /// <returns>next word in the file</returns>
    private string ReadNextWord(char nextChar){
      if (nextChar == '\r' || nextChar == '\n'){
        if(nextChar == '\r'){
          UpdateNewMatchPosition(true, default);
          //Its twice because in windows a new line its conformed by "\r\n"
          return string.Concat((char)sr.Read(), (char)sr.Read());
        }else{
          UpdateNewMatchPosition(true, default);
          return string.Concat("\r" + (char)sr.Read());
        }
      }
      else{
        string newWord = "";
        while(nextChar != ' ' && nextChar != '\t' && nextChar != '\r'  && nextChar != '\n'){
          newWord += (char)sr.Read();
          nextChar = (char)sr.Peek();
          if (sr.Peek() == -1) return newWord;
        }
        return newWord;
      }
    }



    /// <summary>Jumps the withe spaces '\t' and ' ' and updates the LastMatch["BeginingCol"] position</summary>
    /// <param name="lastChar">Last readed char to evalueate if it was a blank space</param>
    private void EatWhiteSpaces(char lastChar){
      while(lastChar == (int)' ' || lastChar == (int)'\t'){
        LastMatch["BeginingCol"]++;
        sr.Read();
        lastChar = (char)sr.Peek();
      }

    }

    #endregion
  }
}