using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DataStructures;

namespace Clases {
  class SymbolTable {
    List<SymbolToken> table;
    List<Variable> tempFormals;
    public List<Variable> tempActuals;
    public ExprManager exprM;

    public SymbolTable(){
      table = new List<SymbolToken>();
      tempFormals = new List<Variable>();
      exprM = new ExprManager(this);
      tempActuals = new List<Variable>();
    }


    public void Insert(SymbolToken newToken, string line) {
      bool exists = (table.Exists(x => x.id == newToken.id && x.Scope == newToken.Scope));
      if (exists) Console.WriteLine("Error en la linea :{0} Uso de elemento ya existente \"{1}\"", line, newToken.id);
      else table.Add(newToken);
    }

    public SymbolToken Search(string ID, List<string> scope){
      List<string> editableList = new List<string>(scope);
      List<string> baseScope = new List<string>();
      SymbolToken baseItem = new SymbolToken();

      //Creates the base scope that exists on the table ex (class.method.objectInstance)
      Func<SymbolToken, string, bool> searchCondition = (x, id) => x.id == id && x.Scope == string.Join("-", baseScope);
      
      for(int i = 0; i < editableList.Count; i++ ) {
        baseScope.Add(editableList[i]);
        string tempId = ( i < editableList.Count - 1) ? editableList[i + 1] : ID;  
        
        if (table.Count(x => searchCondition(x, tempId)) > 0)
          baseItem = table.First(x => searchCondition(x, tempId));
        else { 
          if (tempId == ID)
            baseScope.Remove(editableList[i]);
          break;
        }
      }

      foreach (var item in baseScope) 
        editableList.Remove(item);
      
      if (editableList.Count == 0) return baseItem;

      baseScope.Add(editableList[0]);
      editableList.RemoveAt(0);
      editableList.Add(ID);


      while (editableList.Count != 0) {
        string tempID = editableList[0];

        if (baseItem is Class) {
          Class classItem = (Class)baseItem;
          if (classItem.variables.Count(x => searchCondition(x, tempID)) > 0)
            baseItem = classItem.variables.First(x => searchCondition(x, tempID));
          else if (classItem.functions.Count(x => searchCondition(x, tempID)) > 0)
            baseItem = classItem.functions.First(x => searchCondition(x, tempID));
          else return null;
        }

        else if (baseItem is Objeto){
          Objeto ObjetoItem = (Objeto)baseItem;
          if (ObjetoItem.variables.Count(x => searchCondition(x, tempID)) > 0)
            baseItem = ObjetoItem.variables.First(x => searchCondition(x, tempID));
          else if (ObjetoItem.functions.Count(x => searchCondition(x, tempID)) > 0)
            baseItem = ObjetoItem.functions.First(x => searchCondition(x, tempID));
          else return null;
        }

        else if (baseItem is Function){
          Function functionItem = (Function)baseItem;
          if (functionItem.arguments.Count(x => searchCondition(x, tempID)) > 0)
            baseItem = functionItem.arguments.First(x => searchCondition(x, tempID));
          else return null;
        }

        else if (baseItem is Prototype){
          Prototype prototypeItem = (Prototype)baseItem;
          if (prototypeItem.arguments.Count(x => searchCondition(x, tempID)) > 0)
            baseItem = prototypeItem.arguments.First(x => searchCondition(x, tempID));
          else return null;
        }

        baseScope.Add(editableList[0]);
        editableList.RemoveAt(0);
      }
      return baseItem;
    }

    public Class getClass(string name, List<string> scope, string line){
      if ( table.Count(x => x.id == name && x.Scope == scope[0] && x is Class) > 0 ) 
        return (Class)table.First(x => x.id == name && x.Scope == scope[0] );
      else {
        Console.WriteLine("Error en la linea :" + line + ". No existe deficinicon para la clase " + name);
        return new Class(){ id = "undefinedClass"};
      }
    }

    public int Delete(SymbolToken newToken, List<string> scope) {
      string newScope = string.Join('-', scope);
      newToken.Scope = newScope;
      bool exists = (table.Exists(x => x.id == newToken.id && x.Scope == newScope));

      if (exists) {
        return table.RemoveAll(x => x.id == newToken.id && x.Scope == newScope);
      } else return 0; 
    }

    public void AddFormals(){
      Variable lastVar = (Variable)table[table.Count -1];
      string id = lastVar.id;
      List<string> scope = lastVar.Scope.Split('-').ToList();

      string fatherClass = scope[scope.Count() - 1];
      scope.Remove(fatherClass);

      var father = Search(fatherClass, scope);
      if (father is Class)
        ((Class)father).variables.RemoveAll(x => x.id == id && x.Scope == string.Join("-", scope) + "-" + fatherClass);

      table.RemoveAt(table.Count -1);
      tempFormals.Add(lastVar);
    }

    public void AddFormals(SymbolToken newToken, string scope){
      for (int i = 0; i < tempFormals.Count; i++)
        tempFormals[i].Scope = scope;
      
      table.AddRange(tempFormals);

      if (newToken is Function)  ((Function)newToken ).arguments = new List<Variable>(tempFormals);
      if (newToken is Prototype) ((Prototype)newToken).arguments = new List<Variable>(tempFormals);
      tempFormals = new List<Variable>();
    }

    public void compareActuals(string functName, List<string> scopeList, string line){
      Function function = (Function)this.Search(functName, scopeList);
      if (function != null) {
        for (int i = 0; i < tempActuals.Count(); i++) {
          if (tempActuals[i] != function.arguments[0].type) {
            Console.WriteLine("Error en la linea :" + line + "Tipo de argumento invalido en la funcion" +
            " " + function.id + " Argumento #" + (i + 1) + " no corresponde al tipo");
          }
        }
      }
      tempActuals = new List<string>();
    }

    public void printTable(){
      string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      path += @"\Tabla de simbolos.txt";
      System.Console.WriteLine(path);
      string text = "".PadLeft(157, '-') + "\r\n";
      foreach (var item in table) text += item + "\r\n";
      text += "".PadLeft(157, '-');
      File.WriteAllText(path, text);
    }
  }

  class ExprManager{
    public readonly SymbolTable table;
    public Stack<ExprNode> ExpresionAcumulated = new Stack<ExprNode>();

    public ExprManager(SymbolTable table){
      this.table = table;
    }

    public void evaluateNewExpr(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      switch (exprid) {

        /*Ignore case*/
        case 80: case 83: case 87: case 90: case 94: case 98:
          ignoreExpr(exprid, scopelist, StackSymbolTrack);
          return;

        /*heritage case*/
        case 102: case 103: case 104: case 105: case 106:
          heritage(exprid, scopelist, StackSymbolTrack);
          return;

        /*SaveConstant case*/
        case 107: case 108: case 109: case 110: case 111:
          saveConstant(exprid, scopelist, StackSymbolTrack);
          return;

        /*getValAndConc case*/
        case 96: case 97:  //Expr5 -> Constant || Expr5 -> LValue
          getValAndConcExpr(exprid, scopelist, StackSymbolTrack);
          return;

        /*concat case*/
        case 99: case 100: case 101:  //Expr5 -> ( Expr ) || Expr5 -> - Expr || Expr5 -> ! Expr
          concatExpr(exprid, scopelist, StackSymbolTrack);
          return;

        /*evaluatePrimas case*/
        case 79: case 82: case 85: case 86: case 89: case 92: case 93:
          evaluatePrimas(exprid, scopelist, StackSymbolTrack);
          return;

        /*evaluateDobles case*/
        case 78: case 81: case 84: case 88: case 91:
          evaluateDobles(exprid, scopelist, StackSymbolTrack);
          return;

        /*evaluate case*/
        case 95:
          evaluateExpr(exprid, scopelist, StackSymbolTrack);
          return;

          default: return;
      }
    }

    private void ignoreExpr(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      ExpresionAcumulated.Push(new ExprNode(){
        varName = "eps",
        Scope = string.Join("-", scopelist),
        Type = "eps",
        Value = ""
      });
    }

    private void heritage(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      string ID;

      if (exprid == 104 || exprid == 106) { //LValue -> Expr . IDENT || IDENT -> ident . IDENT
        ID = ExpresionAcumulated.Peek().varName;
        scopelist.Add(StackSymbolTrack.Skip(2).First().aux);

        ExpresionAcumulated.Pop();

      } else { // (103) Expr5 -> New ( ident ) || (103) LValue -> IDENT || (105) IDENT -> ident
        if (exprid == 103) return;
        ID = StackSymbolTrack.Peek().aux;
      }

      //Fix search method
      Variable tempVar = (Variable)table.Search(ID, scopelist);
      if (tempVar == null) {
        ExpresionAcumulated.Push(new ExprNode(){
          varName = ID,
          Scope = string.Join('-', scopelist),
          Type = "",
          Value = "" 
        });
        if (StackSymbolTrack.Skip(1).First().symbol != ".")
          throw new Exception("Uso de variable sin declarar \"" + ID + "\"");
        return;
      }

      ExpresionAcumulated.Push(new ExprNode(){
        varName = ID,
        Scope   = tempVar.Scope,
        Type    = tempVar.type ,
        Value   = tempVar.value
      });
    }

    private void saveConstant(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack) {
      var Value = StackSymbolTrack.Peek();
      ExpresionAcumulated.Push(new ExprNode(){
        varName = "",
        Scope = string.Join("-", scopelist),
        Type = Value.symbol,
        Value = Value.aux
      });
    }
    private void getValAndConcExpr(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      return;
    }

    private void concatExpr(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      ExprNode tmp  = ExpresionAcumulated.First();
      ExpresionAcumulated.Pop();
      
      string val = 
      (exprid == 99) 
        //   Expr5 -> ( Expr )
        ? "(" + tmp.Value + ")"
        // Expr5 -> - Expr  || Expr5 -> ! Expr
        : ((exprid == 100) ? "-" : "!") + tmp.Value;
        
      ExpresionAcumulated.Push(new ExprNode(){
        varName = "",
        Scope = string.Join("-", scopelist),
        Type = tmp.Type,
        Value = val
      });
    }

    private void evaluateExpr(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      string ID = ExpresionAcumulated.Skip(1).First().varName;
      List<string> scope = ExpresionAcumulated.Skip(1).First().Scope.Split("-").ToList();
      Variable tempVar = (Variable)table.Search(ID, scope);
      ExprNode tempNode = ExpresionAcumulated.Peek();


      //Logica gruesa de validacion de tipos
      if ( tempVar.type != tempNode.Type )
        if ( !(tempVar.type == "double" && tempNode.Type == "int") )
          if ( !(tempNode.Type.Contains(tempVar.type) || tempVar.type.Contains(tempNode.Type)) )
            throw new Exception("Los tipos no son compatibles");
      
      try {
        if (tempVar.type == "string") tempVar.value = concatStrings(tempNode.Value);
        else tempVar.value = new DataTable().Compute(tempNode.Value, null).ToString();
      } catch (System.Exception EX) {
        tempVar.value = "Undefined";
        System.Console.WriteLine("**** Just for dev **** " + EX.Message); 
      }
    }




    /// <summary>
    /// Recives a string concatenation in just one string and returns the concatenation
    ///   Example:
    ///     srtg = "Hello"+" World!"
    ///     returns "Hello World!"
    /// </summary>
    /// <param name="strg">String to concatenate</param>
    /// <returns>Simulation of string concatenation</returns>
    string concatStrings(string strg){
      List<string> strings = Regex.Split(strg, @"\s*[+]\s*").ToList();

      strings = strings.Select(x => {
        if (x[0] == '\"') x = x.Substring(1);
        if (x[x.Length -1] == '\"') x = x.Remove(x.Length -1);
        return x;
      }).ToList();
      return '\"' + string.Join("", strings) + '\"';
    }



    private void evaluatePrimas(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      var item = ExpresionAcumulated.Skip(1).First();
      var itemPrim = ExpresionAcumulated.Peek();
      ExpresionAcumulated.Pop();
      ExpresionAcumulated.Pop();
      
      string newValue = StackSymbolTrack.Skip(2).First().symbol;
      newValue += item.Value + itemPrim.Value;


      ExpresionAcumulated.Push(new ExprNode(){
        varName = "",
        Scope = string.Join("-", scopelist),
        Type = item.Type,
        Value = newValue
      });
    }

    private void evaluateDobles(int exprid, List<string> scopelist, Stack<TrackItem> StackSymbolTrack){
      var item = ExpresionAcumulated.Skip(1).First();
      var itemPrim = ExpresionAcumulated.Peek();
      ExpresionAcumulated.Pop();
      ExpresionAcumulated.Pop();

      string newValue = item.Value + itemPrim.Value;

      // Just allows auto casting from (double = [ double | int ] [+ - * /] [ double | int ])
      List<string> compatibleTypes = new List<string>(){ "double", "doubleConstant", "int", "intConstant" };
      bool isOperable = ( 
        (  item.Type == itemPrim.Type )               ||
        (  item.Type.Contains(itemPrim.Type))         ||
        (  itemPrim.Type.Contains(item.Type))         ||
        (  itemPrim.Type == "eps" )                   ||
        (  compatibleTypes.Contains(item.Type)        &&
        (  compatibleTypes.Contains(itemPrim.Type) ) )
      );

      bool isDouble = item.Type.Contains("double") || itemPrim.Type.Contains("double");



      ExpresionAcumulated.Push(new ExprNode(){
        varName = "",
        Scope = string.Join("-", scopelist),
        Type = (isOperable) 
          ? ((isDouble) ? "double" : item.Type)  
          : "Error en tipos",
        Value = newValue
      });
    }

    public void cleanExpr(){
      ExpresionAcumulated = new Stack<ExprNode>();
    }
  }

  public class ExprNode {
    public string Scope { get; set; }
    public string varName { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
  }
}