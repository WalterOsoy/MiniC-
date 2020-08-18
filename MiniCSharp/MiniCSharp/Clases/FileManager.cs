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
        {"Line", 0},
        {"FirstCol", 1},
        {"LastCol", 1}
      };
    }


    #region  Public Functions
    public char ReadChar(){
      return (char)sr.Read();
    }

    public void WriteMatch(string MatchedString, string Type){
      string line = BuildMatchString(MatchedString, Type);
      sw.WriteLine(line);
    }

    public void WriteMatch(string MatchedString, string Type, string value){
      string line = BuildMatchString(MatchedString, Type, value);
      sw.WriteLine(line);
    }

    public void WriteError(string UndefinedCharacter){
      string line = String.Format(
        "*** Error in line {0}*** Unrecognized char: {1}",
        LastMatch["Line"], UndefinedCharacter
      ) + "\n";
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
        LastMatch["FirstCol"], 
        LastMatch["LastCol"], 
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
    
    private Dictionary<string, int> match(){
      Dictionary<string, int> rtrnDict = new Dictionary<string, int>(LastMatch);
      LastMatch["FirstCol"] = LastMatch["LastCol"];
      return rtrnDict;
    }
    #endregion
  }
}