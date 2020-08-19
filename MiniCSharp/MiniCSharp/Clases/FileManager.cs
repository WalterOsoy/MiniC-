using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace Clases
{
  /// <summary>
  /// Class that manages how to read the input file and 
  /// how to write the output file.
  /// </summary>
  class FileManager
  {

    #region Variables and builder

    private StreamReader sr;
    private StreamWriter sw;
    private Dictionary<string, int> LastMatch;


    /// <summary>Initilizes the class requesting path of the the imput file.</summary>
    /// <param name="FilePath">Imput file that its going to be readed.</param>
    public FileManager(string FilePath){
      sr = new StreamReader(FilePath, Encoding.UTF8);
      sw = new StreamWriter(new FileStream(FilePath.Replace("txt", "out"), FileMode.Create), Encoding.UTF8);
      LastMatch = new Dictionary<string, int>(){
        {"Line", 1},
        {"BeginingCol", 1}
      };
    }

    #endregion


    #region  Public Functions

    /// <summary>Reads the next usefull char (jumps white spaces and new line)</summary>
    /// <returns>the next char of the file</returns>
    public char ReadChar(){
      EatWhiteSpaces();
      return (char)sr.Read();
    }



    /// <summary>Writes a new line on the file, with a default format</summary>
    /// <param name="MatchedString">String that was identified</param>
    /// <param name="Type">Type of the string that was identified</param>
    public void WriteMatch(string MatchedString, string Type){
      string line = BuildMatchString(MatchedString, Type);
      UpdateNewMatchPosition(MatchedString.Length);
      sw.WriteLine(line);
    }



    /// <summary>Writes a new line on the file, with a default format</summary>
    /// <param name="MatchedString">String that was identified</param>
    /// <param name="Type">Type of the string that was identified</param>
    /// <param name="value">Value of the string that was found</param>
    public void WriteMatch(string MatchedString, string Type, string value){
      string line = BuildMatchString(MatchedString, Type, value);
      UpdateNewMatchPosition(MatchedString.Length);
      sw.WriteLine(line);
    }



    /// <summary>Writes a new line on the file with an error format</summary>
    /// <param name="UndefinedCharacter">Character that doesnt fit in any type</param>
    public void WriteError(string UndefinedCharacter){
      string line = BuildErrorString(UndefinedCharacter);
      UpdateNewMatchPosition(UndefinedCharacter.Length);
      sw.WriteLine(line);
    }



    /// <summary>Closes the streams used to read and write the files</summary>
    public void Close(){
      sr.Close();
      sw.Close();
    }
    
    #endregion


    #region  Private Functions

    /// <summary>Builds the string that its going to be writed on the file with a default format.</summary>
    /// <param name="MatchedString">String that was identified</param>
    /// <param name="Type">Type of the string that was identified</param>
    /// <returns>A string formatted with data</returns>
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



    /// <summary>Builds the string that its going to be writed on the file with a default format.</summary>
    /// <param name="MatchedString">String that was identified</param>
    /// <param name="Type">Type of the string that was identified</param>
    /// <param name="value">Value of the matched string</param>
    /// <returns>A string formatted with data</returns>
    private string BuildMatchString(string MatchedString, string Type, string value){
      return String.Format(
        "{0} (value = {1})",
        BuildMatchString(MatchedString, Type),
        value
      );
    }
    


    /// <summary>Builds the error string that its going to be writed on the file with a default format.</summary>
    /// <param name="UndefinedCharacter">Undefined char that causes conflict on the grammar</param>
    /// <returns>A string formatted with data</returns>
    private string BuildErrorString(string UndefinedCharacter){
      return String.Format(
        "*** Error in line {0}*** Unrecognized char: {1} at col {2}",
        LastMatch["Line"], 
        UndefinedCharacter, 
        LastMatch["BeginingCol"]
      );
    }



    /// <summary>Jumps all the white spaces and new lines, updates the data of the position of the current Line and colummn</summary>
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



    /// <summary>Updates the beginnig column for the next match when a match its written on a file.</summary>
    /// <param name="MatchLenght">Lenght that we will move the beginnig column to match the next match</param>
    private void UpdateNewMatchPosition(int MatchLenght){
      LastMatch["BeginingCol"] = LastMatch["BeginingCol"] + MatchLenght;
    }
    
    #endregion
  }
}