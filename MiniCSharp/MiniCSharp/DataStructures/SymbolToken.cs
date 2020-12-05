using System.Collections.Generic;

namespace DataStructures {
  class SymbolToken {
    public string id { get; set; }
    public string Scope { get; set; }
  }

  class Class : SymbolToken {
    List<Variable> variables { get; set; }
    List<Function> functions { get; set; }

    public override string ToString() {
      string idStrg = "id: " + id.PadRight(15);
      string scopeStrg = "in scope: " + Scope.PadRight(35);
      return string.Format("| Class     || {0} | {1} |", idStrg, scopeStrg);
    }
  }
  
  class Interface : SymbolToken {
    public override string ToString() {
      string idStrg = "id: " + id.PadRight(15);
      string scopeStrg = "in scope: " + Scope.PadRight(35);
      return string.Format("| Interface || {0} | {1} |", idStrg, scopeStrg);
    }
  }

  class Variable : SymbolToken {
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
    public string Return { get; set; }
    public override string ToString() {
      string idStrg     = "id: "         + id.PadRight(15);
      string scopeStrg  = "in scope: "   + Scope.PadRight(35);
      string typeStrg   = "of type:  "   + type.PadRight(10);
      return string.Format("| Function  || {0} | {1} | {2} |", idStrg, scopeStrg, typeStrg);
    }
  }
}