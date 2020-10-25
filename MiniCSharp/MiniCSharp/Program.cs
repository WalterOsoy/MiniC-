using System;
using System.Collections.Generic;
using Clases;

namespace MiniCSharp
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      new MainMenu().Run();

      // User for testing grammar loader
      // Dictionary<int, Dictionary<string, string>> table = new Dictionary<int, Dictionary<string, string>>();
      // Dictionary<int, Dictionary<string, List<string>>> grammar = new Dictionary<int, Dictionary<string, List<string>>>();
      // new DataLoader(ref table, ref grammar);

      // string Line = "";
      // foreach (var item in grammar){
      //   Line = string.Format("{0}) ", item.Key);
      //   Console.Write(Line.PadLeft(4, '0'));
        
      //   foreach (var item2 in item.Value){
      //     Line = string.Format("{0}", item2.Key);
      //     Line = Line.PadRight(15, ' ');
      //     Console.Write(Line + "--> ");
          
      //     foreach (var item3 in item2.Value){
      //       Line = string.Format("{0}", item3);
      //       Console.Write(Line.PadRight(15, ' '));
      //     }
      //   }
      //   Console.WriteLine();
      }
    }
  }
}