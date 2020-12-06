using System;
using System.Collections.Generic;
using System.Linq;

namespace DataStructures {
  class SymbolToken {
    public string id { get; set; }
    public string Scope { get; set; }

    public override bool Equals(Object obj) {
      //Check for null and compare run-time types.
      if ((obj == null) || ! this.GetType().Equals(obj.GetType())) {
        return false;
      }
      else {
        SymbolToken otherObj = (SymbolToken) obj;
        return (id == otherObj.id) && (Scope == otherObj.Scope);
      }
    }

    public override int GetHashCode() {
      return (id + Scope).GetHashCode();
    }
  }


  class Class : SymbolToken {

    public Class(){
      variables = new List<Variable>();
      functions = new List<Function>();
    }
    public Class(Class newItem){
      id = newItem.id;
      Scope = newItem.Scope;
      variables = new List<Variable>(newItem.variables);
      functions = new List<Function>(newItem.functions);
    }

    public List<Variable> variables { get; set; }
    public List<Function> functions { get; set; }

    public override string ToString() {
      string idStrg = "id: " + id.PadRight(20);
      string scopeStrg = "in scope: " + Scope.PadRight(40);
      return string.Format("| Class     || {0} | {1} |", idStrg, scopeStrg);
    }
  }
  

  class Interface : SymbolToken {
    public Interface(){}
    
    public Interface(Interface newItem){
      id = newItem.id;
      Scope = newItem.Scope;
    }

    public override string ToString() {
      string idStrg = "id: " + id.PadRight(20);
      string scopeStrg = "in scope: " + Scope.PadRight(40);
      return string.Format("| Interface || {0} | {1} |", idStrg, scopeStrg);
    }
  }


  class Variable : SymbolToken {

    public Variable(){}
    public Variable(Variable newItem){
      id = newItem.id;
      Scope = newItem.Scope;
      type = newItem.type;
      value = newItem.value;
      isConstant = newItem.isConstant;
    }


    public string type { get; set; }
    public string value { get; set; }
    public bool isConstant = false;

    public override string ToString() {
      string varOrConst = (isConstant) ? "Constant" : "Variable";
      string idStrg     = "id: "         + id.PadRight(20);
      string scopeStrg  = "in scope: "   + Scope.PadRight(40);
      string typeStrg   = "of type:  "   + type.PadRight(10);
      string valueStrg  = "with value: " + ((value != "") ? value.PadRight(25) : "Undefined".PadRight(25));
      return string.Format("| {0}  || {1} | {2} | {3} | {4} |", varOrConst, idStrg, scopeStrg, typeStrg, valueStrg);
    }
  }


  class Objeto : Variable {

    public Objeto(){}
    public Objeto(Objeto newItem){
      id         = newItem.id;
      Scope      = newItem.Scope;
      type       = newItem.type;
      value      = newItem.value;
      isConstant = newItem.isConstant;
      variables  = newItem.variables.Select(x => (x is Objeto) ? new Objeto((Objeto)x) : new Variable(x)).ToList();
      functions  = newItem.functions.Select(x => new Function(x)).ToList();
    }
    public Objeto(Variable baseVar, Class baseClass){
      id         = baseVar.id;
      Scope      = baseVar.Scope;
      type       = baseVar.type;
      value      = (baseClass.id != "undefinedClass") ? baseClass.id : baseVar.value;
      isConstant = baseVar.isConstant;
      variables  = baseClass.variables.Select(x => (x is Objeto) ? new Objeto((Objeto)x) : new Variable(x)).ToList();
      functions  = baseClass.functions.Select(x => new Function(x)).ToList();

      string objectScope = Scope + '-' + id;
      variables = variables.Select(x => {
        x.Scope = objectScope;
        return x;
      }).ToList();

      functions = functions.Select(x => {
        x.Scope = objectScope;
        return x;
      }).ToList();
    }

    public List<Variable> variables { get; set; }
    public List<Function> functions { get; set; }

    public override string ToString() {
      string idStrg     = "id: " + id.PadRight(20);
      string scopeStrg  = "in scope: " + Scope.PadRight(40);
      string typeStrg   = "of type:  "  + type.PadRight(10);
      string valueStrg  = "with value: " + ((value != "") ? value.PadRight(25) : "Undefined".PadRight(25));
      string obj        = string.Format("| Object    || {0} | {1} | {2} | {3} |", idStrg, scopeStrg, typeStrg, valueStrg);
      string vars       = string.Join("\r\n", variables);
      string funcs      = string.Join("\r\n", functions);
      
      return obj + 
        ((variables.Count > 0) ? "\r\n" + vars : "") +
        ((functions.Count > 0) ? "\r\n" + funcs : "");
    }
  }


  class Prototype : SymbolToken {

    public Prototype(){}
    public Prototype(Prototype newItem){
      id = newItem.id;
      Scope = newItem.Scope;
      type = newItem.type;
      arguments = new List<Variable>(newItem.arguments);
    }


    public string type { get; set; }
    public List<Variable> arguments { get; set; }

    public override string ToString() {
      string idStrg     = "id: "         + id.PadRight(20);
      string scopeStrg  = "in scope: "   + Scope.PadRight(40);
      string typeStrg   = "of type:  "   + type.PadRight(10);
      return string.Format("| Prototype || {0} | {1} | {2} |", idStrg, scopeStrg, typeStrg);
    }
  }


  class Function : Prototype {

    public Function(){
      arguments = new List<Variable>();
    }
    public Function(Function newItem){
      id = newItem.id;
      Scope = newItem.Scope;
      type = newItem.type;
      arguments = new List<Variable>(newItem.arguments);
      Return = newItem.Return;
    }

    public string Return { get; set; }
    public override string ToString() {
      string idStrg     = "id: "         + id.PadRight(20);
      string scopeStrg  = "in scope: "   + Scope.PadRight(40);
      string typeStrg   = "of type:  "   + type.PadRight(10);
      return string.Format("| Function  || {0} | {1} | {2} |", idStrg, scopeStrg, typeStrg);
    }
  }
}