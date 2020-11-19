using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DataStructures;

namespace Clases {
  class dataPrinter {
    string path = Directory.GetCurrentDirectory() + "\\TestFiles\\VERBOSE.csv";
    public dataPrinter(){
      string header = "TRACK" + "," + "STACK" + "," + "SYMBOL" + "," + "ACTION" + "," + "TOKEN LIST";
      File.WriteAllText(path, header);
    }
    
    public void print(Stack<TrackItem> StackSymbolTrack, Stack<int> stack, Stack<string> symbol, string action, List<Token> tokensList){
      string text = File.ReadAllText(path);
      text  += "\r\n"
            +  string.Join(" ", StackSymbolTrack) + ","
            +  string.Join(" ", stack) + ","
            +  string.Join(" ", symbol) + ","
            +  action + ","
            +  string.Join(" ", tokensList);
      File.WriteAllText(path, text);
    }
  }
}