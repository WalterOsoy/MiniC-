using System.Collections.Generic;

namespace DataStructures {
  class SymbolToken {
    public string id { get; set; }
    public string Scope { get; set; }
  }

  class Class : SymbolToken {

  
  }
  
  class Interface : SymbolToken {

  }

  class Variable : SymbolToken {
    public string type { get; set; }
    public string value { get; set; }
    public bool isConstant = false;
  }

  class Prototype : SymbolToken {
    public string type { get; set; }
    public List<Variable> arguments { get; set; }
  }

  class Function : Prototype {
    public string Return { get; set; }
  }
}