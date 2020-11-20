using System;
using System.Collections.Generic;
using System.Linq;
using DataStructures;

namespace Clases {
  class SymbolTable {
    List<SymbolToken> table;
    List<Variable> tempFormals;
    public List<string> tempActuals;
    public ExprManager exprM;

    public SymbolTable(){
      table = new List<SymbolToken>();
      tempFormals = new List<Variable>();
      exprM = new ExprManager(table);
    }


    public SymbolToken Insert(SymbolToken newToken, List<string> scope, string type) {
      
      if (type == "Function" || type == "Prototype") {
        for (int i = 0; i < tempFormals.Count; i++)
          tempFormals[i].Scope = string.Join('-', scope);
        
        table.AddRange(tempFormals);
        tempFormals = new List<Variable>();

        if (type == "Function")  ((Function)newToken).arguments = tempFormals;
        if (type == "Prototype") ((Prototype)newToken).arguments = tempFormals;

        scope.RemoveAt(scope.Count - 1);
      }



      string newScope = string.Join('-', scope);
      newToken.Scope = newScope;
      bool exists = (table.Exists(x => x.id == newToken.id && x.Scope == newScope));

      if (!exists) {
        table.Add(newToken);
        return newToken;
      }
      else return null;
    }

    public SymbolToken Search(string ID, List<string> scope){
      string scopestrg = string.Join('-', scope);
      return table.FirstOrDefault(x => x.id == ID && x.Scope == scopestrg);
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
      table.RemoveAt(table.Count -1);
      tempFormals.Add(lastVar);
    }


  }

  class ExprManager{

    public readonly List<SymbolToken> table;
    private List<ExprNode> ExpresionAcumulated;
    public ExprManager(List<SymbolToken> table){
      this.table = table;
      ExpresionAcumulated = new List<ExprNode>();
    }
    //                                                 Symbol             Aux                 
    public void AddExpr(List<string> production, List<string> elemens, List<TrackItem> Track){
      List<ExprNode> toInsert = new List<ExprNode>();
      foreach(var item in elemens){
        if(item != "Ɛ" || !item.Contains("Expr"))
            toInsert.Add(new ExprNode(){val= Track[0].aux, type = elemens[0] });
        else break;
      }
      ExpresionAcumulated.AddRange(toInsert);
    }    
    /*ToDo*/
    List<string> getExpresionType (){
      return null;
    }









  }

  public class ExprNode {
    public string val { get; set; }//identificador,isgayandres=true
    public string type { get; set; }//int,bool
  }
}