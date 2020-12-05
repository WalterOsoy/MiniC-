using System.Collections.Generic;
using System.Linq;

namespace DataStructures {
  class SymbolToken {
    public string id { get; set; }
    public string Scope { get; set; }
  }

  class Class : SymbolToken {

    public Class(){}
    public Class(Class newItem){
      id = newItem.id;
      Scope = newItem.Scope;
      variables = new List<Variable>(newItem.variables);
      functions = new List<Function>(newItem.functions);
    }

    public List<Variable> variables { get; set; }
    public List<Function> functions { get; set; }

    public override string ToString() {
      string idStrg = "id: " + id.PadRight(15);
      string scopeStrg = "in scope: " + Scope.PadRight(35);
      return string.Format("| Class     || {0} | {1} |", idStrg, scopeStrg);
    }
  }

  class Object : Class {

    public Object(){}
    public Object(Object newItem){
      id = newItem.id;
      Scope = newItem.Scope;
      variables = new List<Variable>(newItem.variables);
      functions = new List<Function>(newItem.functions);
    }

    public Object(string id, string scope, Class baseClass){
      this.id = id;
      this.Scope = Scope;
      variables = new List<Variable>(baseClass.variables);
      functions = new List<Function>(baseClass.functions);

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

    public override string ToString() {
      string idStrg = "id: " + id.PadRight(15);
      string scopeStrg = "in scope: " + Scope.PadRight(35);
      return string.Format("| Object    || {0} | {1} |", idStrg, scopeStrg);
    }
  }
  
  class Interface : SymbolToken {
    public Interface(){}
    
    public Interface(Interface newItem){
      id = newItem.id;
      Scope = newItem.Scope;
    }

    public override string ToString() {
      string idStrg = "id: " + id.PadRight(15);
      string scopeStrg = "in scope: " + Scope.PadRight(35);
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
      string idStrg     = "id: "         + id.PadRight(15);
      string scopeStrg  = "in scope: "   + Scope.PadRight(35);
      string typeStrg   = "of type:  "   + type.PadRight(10);
      string valueStrg  = "with value: " + ((value != "") ? value.PadRight(25) : "Undefined".PadRight(25));
      return string.Format("| {0}  || {1} | {2} | {3} | {4} |", varOrConst, idStrg, scopeStrg, typeStrg, valueStrg);
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
      string idStrg     = "id: "         + id.PadRight(15);
      string scopeStrg  = "in scope: "   + Scope.PadRight(35);
      string typeStrg   = "of type:  "   + type.PadRight(10);
      return string.Format("| Prototype || {0} | {1} | {2} |", idStrg, scopeStrg, typeStrg);
    }
  }

  class Function : Prototype {

    public Function(){}
    public Function(Function newItem){
      id = newItem.id;
      Scope = newItem.Scope;
      type = newItem.type;
      arguments = new List<Variable>(newItem.arguments);
      Return = newItem.Return;
    }

    public string Return { get; set; }
    public override string ToString() {
      string idStrg     = "id: "         + id.PadRight(15);
      string scopeStrg  = "in scope: "   + Scope.PadRight(35);
      string typeStrg   = "of type:  "   + type.PadRight(10);
      return string.Format("| Function  || {0} | {1} | {2} |", idStrg, scopeStrg, typeStrg);
    }
  }
}