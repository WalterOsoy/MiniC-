using System;
using System.Collections.Generic;
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

        
        string getnumbre = @"[0-9]+";
        public TableParser (ref List<Token> tokensList) {
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

            stack.Push (newStackNumber);
            if (!epsilon) {
              newSymbol = tokensList[0].type;
                symbol.Push(newSymbol);
                tokensList.RemoveAt (0);
            } else {
              newSymbol = "Ɛ";
              symbol.Push (newSymbol);
            }

            StackSymbolTrack.Push(new TrackItem(){
              stackNumber = newStackNumber,
              symbol = newSymbol,
              accepted = (newSymbol == "{" || newSymbol == "}")
            });
        }
        private void Reduction () {
            int num = Convert.ToInt32 (Regex.Match (action, getnumbre).ToString ());
            List<string> production = new List<string> (grammar[num].Keys);
            List<string> elemens = new List<string> (grammar[num][production[0]]);
            elemens.RemoveAll(x => x == "");
            //quita la cantidad de elementos en la pila segun la cantidad de elementos en la produccion 
            for (int i = 0; i < elemens.Count; i++) {
                stack.Pop();
                symbol.Pop();
                StackSymbolTrack.Pop();
            }
            //inserta la produccion 
            symbol.Push (production[0]);
            GoTo (production[0]);
        }
        private void GoTo (string newTrackSymbol) {
          List<string> checkpoints = new List<string>(){"Program","Decl","VariableDecl","Variable","ConstDecl","FunctionDecl","FunctionType","FunctionType","StmtBlock","ClassDecl","ClassHeader","Field","InterfaceDecl","InterfaceHeader","Prototype","Stmt","IfHeader","WhileHeader","ForHeader" };

            int fila = stack.Peek ();
            string entrada = symbol.Peek ();
            action = table[fila][entrada];

            int newStackItem = Convert.ToInt32 (action);
            stack.Push (newStackItem);

            StackSymbolTrack.Push(new TrackItem(){
              stackNumber = newStackItem,
              symbol = newTrackSymbol,
              accepted = checkpoints.Contains(newTrackSymbol)
            });
        }
    }
}