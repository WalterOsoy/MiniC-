using System.Collections.Generic;
using System.Linq;
using DataStructures;

namespace MiniCSharp {
  class SymbolTable {
    List<SymbolToken> table = new List<SymbolToken>();


    public SymbolToken Insert(SymbolToken newToken, List<string> ambito) {
      string newAmbito = string.Join('-', ambito);
      newToken.Ambito = newAmbito;
      bool exists = (table.Exists(x => x.id == newToken.id && x.Ambito == newAmbito));

      if (exists) {
        table.Add(newToken);
        return newToken;
      }
      else return null;
    }

    public SymbolToken Search(string ID, List<string> ambito){
      string ambitostrg = string.Join('-', ambito);
      return table.FirstOrDefault(x => x.id == ID && x.Ambito == ambitostrg);
    }


    public int Delete(SymbolToken newToken, List<string> ambito) {
      string newAmbito = string.Join('-', ambito);
      newToken.Ambito = newAmbito;
      bool exists = (table.Exists(x => x.id == newToken.id && x.Ambito == newAmbito));

      if (exists) {
        return table.RemoveAll(x => x.id == newToken.id && x.Ambito == newAmbito);
      } else return 0; 
    }
  }
}