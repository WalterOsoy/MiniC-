using System.Collections.Generic;

namespace DataStructures {
  class SymbolToken {
    public string id { get; set; }
    public string Scope { get; set; }
  }

  class Class : SymbolToken {
    public override string ToString() {
      return "type: class || id: " + id + " in scope: " + Scope + ";";
    }
  }
  
  class Interface : SymbolToken {
    public override string ToString() {
      return "type: interface || id: " + id + " in scope: " + Scope + ";";
    }
  }

  class Variable : SymbolToken {
    public string type { get; set; }
    public string value { get; set; }
    public bool isConstant = false;

    public override string ToString() {
      return (isConstant)
        ? "type: Constant || id: " + id +" of type: " + type + " with Value: " + value + " in scope: " + Scope + ";"
        : "type: Variable || id: " + id +" of type: " + type + " with Value: " + value + " in scope: " + Scope + ";";
    }
  }

  class Prototype : SymbolToken {
    public string type { get; set; }
    public List<Variable> arguments { get; set; }

    public override string ToString() {
      return "type: prototype || id: " + id +" of type: " + type + /*" with args: \r\n    -" + string.Join("\r\n    -", arguments) +*/ " in scope: " + Scope + ";";
    }
  }

  class Function : Prototype {
    public string Return { get; set; }
    public override string ToString() {
      return "type: Function || id: " + id +" of type: " + type + /*" with args: \r\n    -" + string.Join("\r\n    -", arguments) +*/ " in scope: " + Scope + ";";
    }
  }
}