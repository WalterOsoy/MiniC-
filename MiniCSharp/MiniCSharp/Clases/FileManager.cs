using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Clases
{
  class FileManager
  {
    private StreamReader sr;
    private StreamWriter sw;
    private Dictionary<string, int> LastMatch;

    public FileManager(string FilePath){
      sr = new StreamReader(FilePath, Encoding.UTF8);
      sw = new StreamWriter(new FileStream(FilePath.Replace("txt", "out"), FileMode.Create), Encoding.UTF8);
      LastMatch = new Dictionary<string, int>(){
        {"Line", 1},
        {"BeginingCol", 1}
      };
    }


    #region  Public Functions
    public char ReadChar(){
      EatWhiteSpaces();
      return (char)sr.Read();
    }

    public void WriteMatch(string MatchedString, string Type){
      string line = BuildMatchString(MatchedString, Type);
      UpdateLastMatchPosition(MatchedString.Length);
      sw.WriteLine(line);
    }

    public void WriteMatch(string MatchedString, string Type, string value){
      string line = BuildMatchString(MatchedString, Type, value);
      UpdateLastMatchPosition(MatchedString.Length);
      sw.WriteLine(line);
    }

    public void WriteError(string UndefinedCharacter){
      string line = BuildErrorString(UndefinedCharacter);
      UpdateLastMatchPosition(UndefinedCharacter.Length);
      sw.WriteLine(line);
    }

    public void Close(){
      sr.Close();
      sw.Close();
    }
    
    #endregion



    #region  Private Functions
    private string BuildMatchString(string MatchedString, string Type){
      return String.Format(
        "{0}found in line {1}, in cols range {2}-{3}, of type {4}", 
        MatchedString.PadRight(12, ' '),
        LastMatch["Line"], 
        LastMatch["BeginingCol"],
        LastMatch["BeginingCol"] + MatchedString.Length - 1,
        Type
      );
    }

    
    private string BuildMatchString(string MatchedString, string Type, string value){
      return String.Format(
        "{0} (value = {1})",
        BuildMatchString(MatchedString, Type),
        value
      );
    }
    

    private string BuildErrorString(string UndefinedCharacter){
      return String.Format(
        "*** Error in line {0}*** Unrecognized char: {1} at col {2}",
        LastMatch["Line"], 
        UndefinedCharacter, 
        LastMatch["BeginingCol"]
      );
    }


    private void EatWhiteSpaces(){
      char nextChar = (char)sr.Peek();
      if (nextChar == (int)'\r'){
        LastMatch["Line"]++;
        LastMatch["BeginingCol"] = 1;
        //Its twice because in windows a new line its conformed by \r\n
        sr.Read();
        sr.Read();
        EatWhiteSpaces();
      }
      else if (nextChar == (int)' ' || nextChar == (int)'\t'){
        LastMatch["BeginingCol"]++;
        sr.Read();
        EatWhiteSpaces();
      }
    }

    private void UpdateLastMatchPosition(int MatchLenght){
      LastMatch["BeginingCol"] = LastMatch["BeginingCol"] + MatchLenght;
    }
    
    #endregion
  }
}