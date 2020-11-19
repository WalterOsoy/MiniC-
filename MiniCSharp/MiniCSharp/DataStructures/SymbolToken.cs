using System.Collections.Generic;

namespace DataStructures {
  class SymbolToken {
    public string id { get; set; }
    public string Ambito { get; set; }
  }

  class Class : SymbolToken {

  }

  class Variable : SymbolToken {
    public string value { get; set; }
    public string type { get; set; }

    public bool isConstant = false;
  }

  class Funcion : SymbolToken {
    public string Return { get; set; }
    public string type { get; set; }

    public List<Variable> arguments { get; set; }
  }
}
