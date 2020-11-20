using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataStructures;

namespace Clases {
  class TableParser {
    List<Token> tokensList;
    Stack<int> stack;
    Stack<string> symbol;
    string action;
    Dictionary<int, Dictionary<string, string>> LR1table;
    Dictionary<int, Dictionary<string, List<string>>> grammar;
    Stack<TrackItem> StackSymbolTrack = new Stack<TrackItem>();
    List<string> Scope = new List<string>();
    SymbolTable symbolTable = new SymbolTable();

    dataPrinter VERBOSE = new dataPrinter();
    string getnumbre = @"[0-9]+";
    public TableParser (ref List<Token> tokensList) {
      this.tokensList = tokensList;
      LR1table = new Dictionary<int, Dictionary<string, string>> ();
      grammar = new Dictionary<int, Dictionary<string, List<string>>> ();
      new DataLoader (ref LR1table, ref grammar);
      stack = new Stack<int> ();
      symbol = new Stack<string> ();
      stack.Push (0);
      action = "";
      StackSymbolTrack.Push(new TrackItem(){ symbol = "Inicio Lista", stackNumber = 0, accepted = true });
      tokensList.Add (new Token { type = "$", Value = "$" });
      Scope = new List<string>(){ "Program" };
      symbolTable = new SymbolTable();
    }
    public void parse () {
      bool end = false;
      int fila;
      Token entrada;
      do {
        fila = stack.Peek ();
        entrada = tokensList[0];
        action = LR1table[fila][entrada.type];
        mainSwitch(ref end);
      } while (end != true);
    }
    private void mainSwitch(ref bool end){
      switch (action[0]) {

        case 's': //desplazamiento                        
        VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
          Displacement (false);
          break;
       case 'r': //reduccion 
        VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
          Reduction ();
          break;
        case 'a': //Aceptar
        VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
          end = true;
          break;
        case 'e': //error 
          action = LR1table[stack.Peek()]["Ɛ"];

          VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
          switch (action[0]) {
            case 's':
              Displacement (true);
              break;
            case 'r':
              Reduction ();
              break;
            case 'e':
              ManageError(ref end);
              break;
          }
          break;
      }
    }
    private void Displacement (bool epsilon) {
      int newStackNumber = Convert.ToInt32 (Regex.Match (action, getnumbre).ToString ());
      string newSymbol = "";
      string aux = "";

      stack.Push (newStackNumber);
      if (!epsilon) {
        newSymbol = tokensList[0].type;
        aux =  (newSymbol == "ident" || 
                newSymbol == "intConstant" ||
                newSymbol == "doubleConstant" ||
                newSymbol == "boolConstant" ||
                newSymbol == "stringConstant" ||
                newSymbol == "null"
                ) ? tokensList[0].Value : "";
        symbol.Push(newSymbol);
        tokensList.RemoveAt (0);
      } else {
        newSymbol = "Ɛ";
        symbol.Push (newSymbol);
      }

      addToStackSymbolTrack(newStackNumber, newSymbol,(newSymbol == "{" || newSymbol == "}"), aux);
    }
    private void Reduction () {
      int num = Convert.ToInt32 (Regex.Match (action, getnumbre).ToString ());
      List<string> production = new List<string> (grammar[num].Keys);
      List<string> elemens = new List<string> (grammar[num][production[0]]);
      elemens.RemoveAll(x => x == "");

      UpdateScope(production[0]);
      AddToSimTable(production[0]);
      checkMethodAttributes(production[0]);


      //quita la cantidad de elementos en la pila segun la cantidad de elementos en la produccion
      string type = "";
      for (int i = 0; i < elemens.Count; i++) {
        stack.Pop();
        if ((production[0] == "Type" && i == 1) || (production[0] == "ConstType"))
          type = symbol.Peek();
        symbol.Pop();
        StackSymbolTrack.Pop();
      }
      //inserta la produccion 
      symbol.Push(production[0]);
      GoTo (production[0], type);
    }
    private void GoTo (string newTrackSymbol, string type) {
      List<string> checkpoints = new List<string>(){"Program","Decl","VariableDecl","ConstDecl","FunctionDecl","FunctionType","StmtBlock","ClassDecl","ClassHeader","Field","InterfaceDecl","InterfaceHeader","Prototype","Stmt","IfHeader","WhileHeader","ForHeader" };

      int fila = stack.Peek ();
      string entrada = symbol.Peek ();
      action = LR1table[fila][entrada];
                VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));

      int newStackItem = Convert.ToInt32 (action);
      stack.Push (newStackItem);

      addToStackSymbolTrack(newStackItem, newTrackSymbol, checkpoints.Contains(newTrackSymbol), type);
    }
    private void ManageError(ref bool end){
      if(tokensList[0].type.Equals("$")){
        end = true;
        return;
      }
      Console.WriteLine("Error en el parse en: \n Token: '" + tokensList[0].Value + "' en la linea: " + tokensList[0].line + " columnas: " + tokensList[0].column);
      while (!StackSymbolTrack.Peek().accepted) {
        stack.Pop();
        symbol.Pop();
        StackSymbolTrack.Pop();
      }
      
      tokensList.RemoveAt(0);
      Console.WriteLine("empezando otra vez, intentaremos parsear el siguiente metodo");
    }
    private void addToStackSymbolTrack(int newStackItem, string newTrackSymbol, bool accepted, string aux){
      StackSymbolTrack.Push(new TrackItem(){
        stackNumber = newStackItem,
        symbol = newTrackSymbol,
        accepted = accepted,
        aux = aux
      });
    }
    private void UpdateScope(string reductionType){
      switch (reductionType) {
        case "ClassDecl":
          Scope.RemoveAt(Scope.Count -1);
          break;
        case "FunctionDecl":
          Scope.RemoveAt(Scope.Count -1);
          break;
        case "InterfaceDecl":
          Scope.RemoveAt(Scope.Count -1);
          break;
        
        default: return;
      }
    }
    private void AddToSimTable(string reductionType){
      switch (reductionType) {

        case "Variable":{
          symbolTable.Insert(
            new Variable(){
              id = StackSymbolTrack.Peek().aux,
              isConstant = false,
              type = StackSymbolTrack.Skip(1).First().aux,
              value = ""
            }
          , Scope
          , "Variable"
          );
          break;
        }

        case "ConstDecl": {
          symbolTable.Insert(
            new Variable(){
              id = StackSymbolTrack.Skip(1).Take(1).First().aux,
              isConstant = true,
              type = StackSymbolTrack.Skip(2).Take(1).First().aux,
              value = ""
            }
            , Scope
            , "ConstDecl"
          );
          break;
        }

        case "FunctionType": {
          string id = StackSymbolTrack.Skip(3).Take(1).First().aux;
          symbolTable.Insert(
            new Function(){
              id = id,
              type = StackSymbolTrack.Skip(4).Take(1).First().aux,
              Return = ""
            }
            , new List<string>(Scope){ id }
            , "Function"
          );
          Scope.Add(id);
          break;
        }

        case "ClassHeader": {
          string id = StackSymbolTrack.Skip(1).Take(1).First().aux; 
          symbolTable.Insert(
            new Class(){ id = id }
            , Scope
            , "Class"
          );
          Scope.Add(id);
          break;
        }

        case "InterfaceHeader":{
          string id = StackSymbolTrack.Peek().aux;
          symbolTable.Insert(
            new Interface(){ id = id }
            , Scope
            , "Interface"
          );
          Scope.Add(id);
          break;
        }

        case "Prototype":{
          symbolTable.Insert(
            new Prototype(){
              id = StackSymbolTrack.Skip(4).First().aux,
              type = StackSymbolTrack.Skip(5).First().aux
            }
            , new List<string>(Scope){ StackSymbolTrack.Skip(4).First().aux }
            , "Prototype"
          );
          break;
        }

        case "Formals":{
          symbolTable.AddFormals();
          break;
        }

        default: return;
      }
    }
  
    private void checkMethodAttributes(string reductionType) {
      switch (reductionType) {
        case "Actuals":

          symbolTable.tempActuals.Insert(0, symbolTable.exprM.);
          break;

        case "CallStmt":
          Scope.RemoveAt(Scope.Count -1);
          break;

        default: return;
      }
    }
  }
}