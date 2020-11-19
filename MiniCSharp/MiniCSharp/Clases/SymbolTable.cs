using System.Collections.Generic;
using System.Linq;
using DataStructures;

namespace Clases {
  class SymbolTable {
    List<SymbolToken> table = new List<SymbolToken>();
    List<Variable> tempFormals = new List<Variable>();
    string nextExpr = "";


    public SymbolToken Insert(SymbolToken newToken, List<string> scope, string type) {
      
      if (type == "Function" || type == "Prototype") {
        for (int i = 0; i < tempFormals.Count; i++)
          tempFormals[i].Scope = string.Join('-', scope);
        
        table.AddRange(tempFormals);
        tempFormals = new List<Variable>();
        // TODO: Limpear la lista
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


    public int SaveToFile() {
      return 1;
    }
  }
}