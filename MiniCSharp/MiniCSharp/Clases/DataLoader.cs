using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace Clases{

  class DataLoader{
    internal class LR1TableLoader{
      private string path;
      private Dictionary<int, Dictionary<string, string>> table;
      public LR1TableLoader(string path){
        this.path = path;
        table = new Dictionary<int, Dictionary<string, string>>();
        readTable();
      }

      public Dictionary<int, Dictionary<string, string>> getTable(){
        return table;
      }

      private void readTable(){
        using (StreamReader sr = new StreamReader(path)){
          Dictionary<string, string> headersTemplate = readHeaders(sr);
          string Line = "";
          int state = 0;

          while ((Line = sr.ReadLine()) != null){
            table.Add(state, readContent(Line, new Dictionary<string, string>(headersTemplate)));
            state++;
          }
        }
      }

      private Dictionary<string, string> readHeaders(StreamReader sr){
        string[] headers = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.IgnorePatternWhitespace);
        Dictionary<string, string> rtrnDict = new Dictionary<string, string>();
        foreach (string header in headers) rtrnDict.Add(CheckValue(header), "");
        return rtrnDict;
      }

      private Dictionary<string, string> readContent(string Line, Dictionary<string, string> template){
        string[] lineValues = Line.Split(',');
        int count = 0;
        List<string> keys = new List<string>(template.Keys);

        foreach (var key in keys){
          if (lineValues[count] == "") template[key] = "error";
          else template[key] = lineValues[count];
          count++;
        }
        return template;
      }

      private string CheckValue(string value){
        return (Regex.IsMatch(value, @"([\S]*\,[\S]*)+"))
          ? value.Trim('"')
          : value;
      }
    }

    internal class GrammarLoader{
      private string path;
      private Dictionary<int, Dictionary<string, List<string>>> grammar;


      public GrammarLoader(string path){
        this.path = path;
        grammar = new Dictionary<int, Dictionary<string, List<string>>>();
        readGrammar();
      }


      public Dictionary<int, Dictionary<string, List<string>>> getGrammar(){
        return grammar;
      }


      public Dictionary<int, Dictionary<string, List<string>>> getTable(){
        return grammar;
      }


      private void readGrammar(){
        using (StreamReader sr = new StreamReader(path)){
          string Line = "";
          int state = 0;

          while ((Line = sr.ReadLine()) != null){
            grammar.Add(state, readContent(Line, new Dictionary<string, List<string>>()));
            state++;
          }
        }
      }


      private Dictionary<string, List<string>> readContent(string Line, Dictionary<string, List<string>> template){
        List<string> lineValues = Regex.Split(Line, ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.IgnorePatternWhitespace).ToList();
        
        string GrammarIndex = lineValues[0];
        lineValues.RemoveAt(0);
        template.Add(GrammarIndex, lineValues.Select(val => CheckValue(val)).ToList());

        return template;
      }


      private string CheckValue(string value){
        return (Regex.IsMatch(value, @"([\S]*\,[\S]*)+"))
          ? value.Trim('"')
          : value;
      }
    }

    public DataLoader(
      ref Dictionary<int, Dictionary<string, string>> table,
      ref Dictionary<int, Dictionary<string, List<string>>> grammar){
      string tablePath = "./utils/LR1 Table.csv";
      string grammarPath = "./utils/Grammar.csv";
      table = new LR1TableLoader(tablePath).getTable();
      grammar = new GrammarLoader(grammarPath).getGrammar();
    }
  }
}