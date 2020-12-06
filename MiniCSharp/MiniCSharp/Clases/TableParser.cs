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

    // dataPrinter VERBOSE = new dataPrinter();

    string getNumber = @"[0-9]+";
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
      symbolTable.printTable();
    }


    private void mainSwitch(ref bool end){
      switch (action[0]) {

        case 's': //desplazamiento                        
        // VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
          Displacement (false);
          break;
       case 'r': //reduccion 
        // VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
          Reduction ();
          break;
        case 'a': //Aceptar
        // VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
          end = true;
          break;
        case 'e': //error 
          action = LR1table[stack.Peek()]["Ɛ"];

          // VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));
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
      int newStackNumber = Convert.ToInt32 (Regex.Match (action, getNumber).ToString ());
      string newSymbol = "";
      string aux = "";

      stack.Push (newStackNumber);
      if (!epsilon) {
        newSymbol = tokensList[0].type;
        List<string> tmplst = new List<string>() { "ident", "intConstant", "doubleConstant", "boolConstant", "stringConstant", "null" };
        aux =  (tmplst.Contains(newSymbol)) ? tokensList[0].Value : "";
        symbol.Push(newSymbol);
        tokensList.RemoveAt (0);
      } else {
        newSymbol = "Ɛ";
        symbol.Push (newSymbol);
      }

      addToStackSymbolTrack(newStackNumber, newSymbol,(newSymbol == "{" || newSymbol == "}"), aux);
    }


    private void Reduction () {
      int num = Convert.ToInt32 (Regex.Match (action, getNumber).ToString ());
      List<string> production = new List<string> (grammar[num].Keys);
      List<string> elemens = new List<string> (grammar[num][production[0]]);
      elemens.RemoveAll(x => x == "");

      UpdateScope(production[0]);
      AddToSimTable(production[0]);
      checkMethodAttributes(production[0]);
      checkExpr(num);

      //quita la cantidad de elementos en la pila segun la cantidad de elementos en la produccion
      string type = "";
      for (int i = 0; i < elemens.Count; i++) {
        stack.Pop();
        if ((production[0] == "Type" && i == 1) || (production[0] == "ConstType"))
          type = (symbol.Peek() == "ident")
            ? StackSymbolTrack.First().aux
            : symbol.Peek();
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
                // VERBOSE.print( new Stack<TrackItem>(StackSymbolTrack),  new Stack<int>(stack), new Stack<string>(symbol), action, new List<Token>(tokensList));

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
        case "FunctionDecl":
        case "InterfaceDecl":
          Scope.RemoveAt(Scope.Count -1);
          break;
        default: return;
      }
    }

#region AddToSimTableFunctions

    private void AddToSimTable(string reductionType){
      switch (reductionType) {

        case "Variable": case "ConstDecl":
          AddToSimTableVariable(reductionType);
          break;

        case "FunctionType": 
          AddToSimTableFunction();
          break;

        case "ClassHeader":
          AddToSimTableClass();
          break;

        case "InterfaceHeader":
          AddToSimTableInterface();
          break;

        case "Prototype":
          AddToSimTablePrototype();
          break;

        case "Formals":{
          symbolTable.AddFormals();
          break;
        }

        default: return;
      }
    }
  

  private void AddToSimTableVariable(string type){
    Variable newVar = (type == "Variable") ? newVariable() : newConstant();

    if (!new string[]{ "int","double","bool","string"}.Contains(newVar.type))
      newVar = new Objeto( newVar, 
        symbolTable.getClass(newVar.type, Scope, tokensList.Skip(1).First().line) );

    CheckFatherForClass(
      (newVar is Objeto) 
        ? new Objeto((Objeto)newVar) 
        : new Variable(newVar)
    );

    symbolTable.Insert(newVar, tokensList.First().line);
  }

  private Variable newVariable(){
    return new Variable(){
      id = StackSymbolTrack.Peek().aux,
      isConstant = false,
      type = StackSymbolTrack.Skip(1).First().aux,
      Scope = string.Join('-', Scope),
      value = ""
    };
  }

  private Variable newConstant(){
    return new Variable(){
      id = StackSymbolTrack.Skip(1).Take(1).First().aux,
      isConstant = true,
      type = StackSymbolTrack.Skip(2).Take(1).First().aux,
      Scope = string.Join('-', Scope),
      value = ""
    };
  }


  private void CheckFatherForClass(Variable newVar){
    string scopeTop = Scope[Scope.Count() - 1];
    Scope.Remove(scopeTop);

    // TODO: Aqui por error se agregan los parametros de las funciones a las clases
    var father = symbolTable.Search(scopeTop, Scope);
    if (father is Class)
      ((Class)father).variables.Add(newVar);
    Scope.Add(scopeTop);
  }

  private void AddToSimTableFunction(){
    string id = StackSymbolTrack.Skip(3).Take(1).First().aux;
    Function newFucn = new Function(){
      id = id,
      type = StackSymbolTrack.Skip(4).Take(1).First().aux,
      Scope = string.Join('-', Scope),
      Return = ""
    };

    string scopeTop = Scope[Scope.Count() - 1];
    Scope.Remove(scopeTop);

    var father = symbolTable.Search(scopeTop, Scope);
    if (father is Class)
      ((Class)father).functions.Add(new Function(newFucn));
    
    Scope.Add(scopeTop);


    symbolTable.Insert(newFucn, tokensList.First().line);
    symbolTable.AddFormals(newFucn, string.Join("-", Scope) + "-" + id);
    Scope.Add(id);
  }

  private void AddToSimTableClass(){
    string id = StackSymbolTrack.Skip(1).Take(1).First().aux; 
    symbolTable.Insert(
      new Class(){ 
        id = id,
        Scope = string.Join('-', Scope) ,
        variables = new List<Variable>(),
        functions = new List<Function>()
      }
      , tokensList.First().line
    );
    Scope.Add(id);
  }

  private void AddToSimTableInterface(){
    string id = StackSymbolTrack.Peek().aux;
    symbolTable.Insert(
      new Interface(){
        id = id,
        Scope = string.Join('-', Scope) 
      }
      , tokensList.First().line
    );
    Scope.Add(id);
  }

  private void AddToSimTablePrototype(){
    Prototype newPr = new Prototype(){
      id = StackSymbolTrack.Skip(4).First().aux,
      type = StackSymbolTrack.Skip(5).First().aux,
      Scope = string.Join('-', Scope) 
    }; 

    symbolTable.Insert(newPr, tokensList.First().line);
    symbolTable.AddFormals(newPr, string.Join("-", Scope) + "-" + newPr.id);

  }


#endregion


    private void checkMethodAttributes(string reductionType) {
      switch (reductionType) {
        case "Actuals":
          /*Logica implementada en seccion de limpiar abajo*/
          // symbolTable.tempActuals.Insert(0, symbolTable.exprM.);
          /* Missing insert logic in Actuals */
          break;

        case "CallStmt":
          string functionName = StackSymbolTrack.Skip(3).First().aux;
          string line = tokensList.First().line;
          symbolTable.compareActuals(functionName, Scope, line);
          break;

        default: return;
      }
    }


    private void checkExpr(int reductionID){
      try {
        symbolTable.exprM.evaluateNewExpr(reductionID, Scope, StackSymbolTrack);
      }
      catch (System.Exception EX) {
        Console.WriteLine("Error en la linea :" + tokensList.First().line + " " + EX.Message);
        // symbolTable.exprM.cleanExpr();
      }
      checkCleanExpr(reductionID);
    }


    private void checkCleanExpr(int reductionID){
      if (reductionID == 60 || 
          reductionID == 66 || 
          reductionID == 70 || 
          reductionID == 73) {
          symbolTable.exprM.cleanExpr();
      }
      if (reductionID == 64) {
        string exprtype = symbolTable.exprM.ExpresionAcumulated.First().Type;
        symbolTable.tempActuals.Add(exprtype);
        symbolTable.exprM.ExpresionAcumulated.Pop();
      }
    }
  }
}