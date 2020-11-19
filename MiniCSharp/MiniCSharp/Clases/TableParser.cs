using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DataStructures;

namespace Clases {
    class TableParser {
        List<Token> tokensList;
        Stack<int> stack;
        Stack<string> symbol;
        string action;
        Dictionary<int, Dictionary<string, string>> table;
        Dictionary<int, Dictionary<string, List<string>>> grammar;
        Stack<TrackItem> StackSymbolTrack = new Stack<TrackItem>();
        List<string> Scope = new List<string>();
        List<SymbolToken> symbolTable = new List<SymbolToken>();

        dataPrinter VERBOSE = new dataPrinter();

        
        string getnumbre = @"[0-9]+";
        public TableParser (ref List<Token> tokensList/*, ref List<SymbolToken> SimTable*/) {
            this.tokensList = tokensList;
            table = new Dictionary<int, Dictionary<string, string>> ();
            grammar = new Dictionary<int, Dictionary<string, List<string>>> ();
            new DataLoader (ref table, ref grammar);
            stack = new Stack<int> ();
            symbol = new Stack<string> ();
            stack.Push (0);
            action = "";
            StackSymbolTrack.Push(new TrackItem(){ symbol = "Inicio Lista", stackNumber = 0, accepted = true });
            tokensList.Add (new Token { type = "$", Value = "$" });

            Scope = new List<string>(){ "Program" };
            // symbolTable = SimTable;
        }
        public void parse () {
            bool end = false;
            bool okey = false;
            int fila;
            Token entrada;
            do {
                fila = stack.Peek ();
                entrada = tokensList[0];
                action = table[fila][entrada.type];


                VERBOSE.print(
                  new Stack<TrackItem>(StackSymbolTrack), 
                  new Stack<int>(stack), 
                  new Stack<string>(symbol), 
                  action, 
                  new List<Token>(tokensList));


                switch (action[0]) {
                    case 's': //desplazamiento                        
                        Displacement (false);
                        break;
                    case 'r': //reduccion 
                        Reduction ();
                        break;
                    case 'a': //Aceptar
                        okey = true;
                        end = true;
                        break;
                    case 'e': //error 
                        action = table[stack.Peek()]["Ɛ"];

                                VERBOSE.print(
                                  new Stack<TrackItem>(StackSymbolTrack), 
                                  new Stack<int>(stack), 
                                  new Stack<string>(symbol), 
                                  action, 
                                  new List<Token>(tokensList));
                  
                        switch (action[0]) {
                            case 's':
                                Displacement (true);
                                break;
                            case 'r':
                                Reduction ();
                                break;
                            case 'e':
                                if(tokensList[0].type.Equals("$")){
                                  end = true;
                                  break;
                                }

                                Console.WriteLine("Error en el parse en: \n Token: '" + tokensList[0].Value + "' en la linea: " + tokensList[0].line + " columnas: " + tokensList[0].column);
                                while (!StackSymbolTrack.Peek().accepted)
                                {
                                  stack.Pop();
                                  symbol.Pop();
                                  StackSymbolTrack.Pop();
                                }
                                
                                tokensList.RemoveAt(0);
                                Console.WriteLine("empezando otra vez, intentaremos parsear el siguiente metodo");
                                break;
                        }
                        break;
                }
            } while (end != true);
            if (okey) { //all okey
                Console.WriteLine ("Parseo exitoso");
            } else { //error
                Console.WriteLine ("Error en el parse en: \n Token:" + tokensList[0].Value + " en la linea: "+tokensList[0].line +" columnas: " +tokensList[0].column);
            }
        }
        private void Displacement (bool epsilon) {
            int newStackNumber = Convert.ToInt32 (Regex.Match (action, getnumbre).ToString ());
            string newSymbol = "";
            string aux = "";

            stack.Push (newStackNumber);
            if (!epsilon) {
              newSymbol = tokensList[0].type;
              aux =  (newSymbol == "ident" || 
                      newSymbol == "intConstant" ||
                      newSymbol == "doubleConstant" ||
                      newSymbol == "boolConstant" ||
                      newSymbol == "stringConstant" ||
                      newSymbol == "null"
                      ) ? tokensList[0].Value : "";
              symbol.Push(newSymbol);
              tokensList.RemoveAt (0);
            } else {
              newSymbol = "Ɛ";
              symbol.Push (newSymbol);
            }

            StackSymbolTrack.Push(new TrackItem(){
              stackNumber = newStackNumber,
              symbol = newSymbol,
              accepted = (newSymbol == "{" || newSymbol == "}"),
              aux = aux
            });
        }
        private void Reduction () {
            int num = Convert.ToInt32 (Regex.Match (action, getnumbre).ToString ());
            List<string> production = new List<string> (grammar[num].Keys);
            List<string> elemens = new List<string> (grammar[num][production[0]]);
            elemens.RemoveAll(x => x == "");

            // UpdateScope(production[0], symbol.Peek());
            // AddToSimTable(production[0]);


            //quita la cantidad de elementos en la pila segun la cantidad de elementos en la produccion
            string type = "";
            for (int i = 0; i < elemens.Count; i++) {
                stack.Pop();
                if ((production[0] == "Type" && i == 1) || (production[0] == "ConstType"))
                  type = symbol.Peek();
                symbol.Pop();
                StackSymbolTrack.Pop();
            }
            //inserta la produccion 
            symbol.Push(production[0]);
            GoTo (production[0], type);
        }
        private void GoTo (string newTrackSymbol, string type) {

          

          List<string> checkpoints = new List<string>(){"Program","Decl","VariableDecl","ConstDecl","FunctionDecl","FunctionType","StmtBlock","ClassDecl","ClassHeader","Field","InterfaceDecl","InterfaceHeader","Prototype","Stmt","IfHeader","WhileHeader","ForHeader" };

            int fila = stack.Peek ();
            string entrada = symbol.Peek ();
            action = table[fila][entrada];

                    VERBOSE.print(
                      new Stack<TrackItem>(StackSymbolTrack), 
                      new Stack<int>(stack), 
                      new Stack<string>(symbol), 
                      action, 
                      new List<Token>(tokensList));

            int newStackItem = Convert.ToInt32 (action);
            stack.Push (newStackItem);

            StackSymbolTrack.Push(new TrackItem(){
              stackNumber = newStackItem,
              symbol = newTrackSymbol,
              accepted = checkpoints.Contains(newTrackSymbol),
              aux = type
            });
        }

        private void UpdateScope(string reductionType, string ID){
          switch (reductionType) {
            case "ClassHeader":
              Scope.Add(ID);
              break;
            case "FunctionType":
              Scope.Add(ID);
              break;
            case "InterfaceHeader":
              Scope.Add(ID);
              break;
            case "ClassDecl":
              Scope.RemoveAt(Scope.Count -1);
              break;
            case "FunctionDecl":
              Scope.RemoveAt(Scope.Count -1);
              break;
            case "InterfaceDecl":
              Scope.RemoveAt(Scope.Count -1);
              break;
            
              default:
                return;
          }
        }
        private void AddToSimTable(string reduction){
            switch (reduction) {
              case "VariableDecl":
                string id = symbol.Skip(1).First();
                Scope.Add("");

                break;
              case "ConstDecl":
                break;
              case "FunctionType":
                break;
              case "ClassHeader":
                break;
              case "InterfaceHeader":
                break;
              case "Prototype":
                break;
              
                default:
                  return;
            }
        }
    }
}