using System.Diagnostics.SymbolStore;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataStructures;

namespace Clases {

    class TableParser {
        List<Token> tokensList;
        Stack<int> stack;
        Stack<string> symbol;
        string action;
        Dictionary<int, Dictionary<string, string>> table;
        Dictionary<int, Dictionary<string, List<string>>> grammar;
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
                        action = table[stack.Peek ()]["Ɛ"];
                        switch (action[0]) {
                            case 's':
                                Displacement (true);
                                break;
                            case 'r':
                                Reduction ();
                                break;
                            case 'e':
                                end = true;
                                okey = false;
                                break;
                        }
                        break;
                }
            } while (end != true);
            if (okey) { //all okey
                Console.WriteLine ("Parseo exitoso");
            } else { //error
                Console.WriteLine ("Error en el parse en: " + tokensList[0].Value + " - > " + fila + ", " + entrada.type + ", " + entrada.Value);
            }
        }
        private void Displacement (bool epsilon) {            
            stack.Push (Convert.ToInt32 (Regex.Match (action, getnumbre).ToString ()));
            if (!epsilon) {
                symbol.Push (tokensList[0].type);
                tokensList.RemoveAt (0);
            } else {
                symbol.Push ("Ɛ");
            }
        }
        private void Reduction () {
            int num = Convert.ToInt32 (Regex.Match (action, getnumbre).ToString ());
            List<string> production = new List<string> (grammar[num].Keys);
            List<string> elemens = new List<string> (grammar[num][production[0]]);
            //quita la cantidad de elementos en la pila segun la cantidad de elementos en la produccion 
            for (int i = 0; i < elemens.Count; i++) {
                stack.Pop ();
            }
            //quita la cantidad de elementos en los simbolos segun la cantidad de elementos en la produccion 
            for (int i = 0; i < elemens.Count; i++) {
                symbol.Pop ();
            }
            //inserta la produccion 
            symbol.Push (production[0]);
            GoTo ();
        }
        private void GoTo () {
            int fila = stack.Peek ();
            string entrada = symbol.Peek ();
            action = table[fila][entrada];
            stack.Push (Convert.ToInt32 (action));
        }
    }
}