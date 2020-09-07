using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using DataStructures;
using System.Linq;

namespace Clases
{
  class SintacticalAnalizer
  {

    Queue<Token> tokensQueue;
    public SintacticalAnalizer(ref Queue<Token> tokensQueue){
      this.tokensQueue = tokensQueue;
    }


    public bool Analize(){

      return true;
    }    
    #region Tokens

    private bool ParsePrg(){
      if (ParseVarD()) return ParseD();
      else if (ParseFuncD()) return ParseD();
      else return false;
    }

    private bool ParseD(){
      bool Matched = ParseVarD();

      if (Matched) {
        return ParseD();
      }
      else{
        Matched = ParseFuncD();
        if (Matched) return ParseD();
        else return true;//Returns true because this accepts nullable values Є
      }
    }
    

    private bool ParseVarD(){
      if (ParseVar()) return MatchLiteral(new string[]{";"});
      else return false;
    }


    private bool ParseVar(){
      bool Matched = ParseType();
      if (Matched) 
        return MatchType("Identificador");
      else
        return true;//Returns true because this accepts nullable values Є
    }
    
    
    private bool ParseVarPrim(){
      if (ParseVar()) return ParseVarPrim();
      else return true;
    }


    private bool ParseType(){
      bool Matched = ParseTypePrim();
      if(Matched) return ParseTypeBiPrim();
      return false;
    }
    
    
    private bool ParseTypePrim(){
      if (MatchLiteral(new string[]{"int", "double", "bool", "string"})) return true;
      else return MatchType("Identificador");
    }


    private bool ParseTypeBiPrim(){
      bool Matched = MatchLiteral(new string[]{"[]"});
      if(Matched) return ParseTypeBiPrim();
      return true;//Returns true because this accepts nullable values Є      
    }
    
    
    private bool ParseFuncD(){
      if (ParseType()){
        if (MatchType("Identificador"))
          if (ParseFrms())
            if(ParseSt())
              return true;
      } else if (MatchLiteral(new string[]{"void"})){
        if (MatchType("Identificador"))
          if (ParseFrms())
            if(ParseSt())
              return true;
      }
      return false;
    }

    #warning Ver por que Matched esta asignado y en la validacion del if se utiliza otro metodo
    private bool ParseFrms(){      
      if(ParseVar()){
        if(ParseVarPrim()){
          return MatchLiteral(new string[]{","});
        }
        return false;
      }
      return true;//Returns true because this accepts nullable values Є      
    }
    
    
    private bool ParseSt(){
      if (ParseIst()) return true;
      else if (ParseRst()) return true;
      else if (ParseExpr()) return MatchLiteral(new string[]{";"});
      else return false;
    }


    private bool ParseIst(){
      if(MatchLiteral(new string[]{"if"})){
        if(MatchLiteral(new string[]{"("})){
          if(ParseExpr()){
            if(MatchLiteral(new string[]{")"})){                
              if(ParseSt()){
                return ParseIstPrim();
              }
              return false;
            }
            return false;
          }
          return false;
        }
        return false;
      }
      return false;
    }    
    private bool ParseIstPrim(){
      if (MatchLiteral(new string[]{"else"})) return ParseSt();
      else return true;
    }


    private bool ParseRst(){
      bool Matched = MatchLiteral(new string[]{"Return"});
      if(Matched){ 
        if(ParseRstPrim())
          return MatchLiteral(new string[]{";"});
      }
      return false;
    }
    
    
    private bool ParseRstPrim(){
      ParseExpr();
      return true;
    }


    private bool ParseExpr(){
      bool Matched = ParseExpr1();
      if(Matched){
        return ParseExprPrim();
      }
      return false;
    }
    
    
    private bool ParseExprPrim(){
      if (MatchLiteral(new string[]{"||"})){
        if (ParseExpr1()) return ParseExprPrim();
        else return false;
      }
      return true;
    }
  
  
    private bool ParseExpr1(){
      bool Matched = ParseExpr2();
      if(Matched){
        return ParseExpr1Prim();
      }
      return false;
    }
    
    
    private bool ParseExpr1Prim(){
      if (MatchLiteral(new string[]{"&&"})){
        if (ParseExpr2()) return ParseExpr1Prim();
        else return false;
      }
      return true;
    }
    
    
    private bool ParseExpr2(){
      bool Matched = ParseExpr3();
      if(Matched){
        return ParseExpr2Prim();
      }
      return false;
    }
    
    
    private bool ParseExpr2Prim(){
      if (MatchLiteral(new string[]{"=="})){
        if (ParseExpr3()) return ParseExpr2Prim();
        else return false;
      } else if (MatchLiteral(new string[]{"!="})){
        if (ParseExpr3()) return ParseExpr2Prim();
        else return false;
      } else return true;
    }
    
    
    private bool ParseExpr3(){
      bool Matched = ParseExpr4();
      if(Matched) 
        return ParseExpr3Prim();
      return false;
    }
    
    
    private bool ParseExpr3Prim(){
      if (MatchLiteral(new string[]{"<"})){
        if (ParseExpr4()) 
          return ParseExpr3Prim();
        else return false;
      } else if (MatchLiteral(new string[]{"<="})){
        if (ParseExpr4()) return ParseExpr3Prim();
        else return false;
      } if (MatchLiteral(new string[]{">"})){
        if (ParseExpr4()) return ParseExpr3Prim();
        else return false;
      } else if (MatchLiteral(new string[]{">="})){
        if (ParseExpr4()) return ParseExpr3Prim();
        else return false;
      } else return true;
    }
    
    
    private bool ParseExpr4(){
      bool Matched = ParseExpr5();
      if(Matched)
        return ParseExpr4Prim();
      return false;
    }
    private bool ParseExpr4Prim(){
      if (MatchLiteral(new string[]{"+"})){
        if (ParseExpr5()) return ParseExpr4Prim();
        else return false;
      } else if (MatchLiteral(new string[]{"-"})){
        if (ParseExpr5()) return ParseExpr4Prim();
        else return false;
      } else return true;
    }
    private bool ParseExpr5(){
      bool Matched = ParseExpr6();
      if(Matched)
        return ParseExpr5Prim();
      return false;
    }
    private bool ParseExpr5Prim(){
      if (MatchLiteral(new string[]{"*"})){
        if (ParseExpr6()) return ParseExpr5Prim();
        else return false;
      } else if (MatchLiteral(new string[]{"/"})){
        if (ParseExpr6()) return ParseExpr5Prim();
        else return false;
      } else return true;
    }
    private bool ParseExpr6(){
      if(ParseExpr()){
        return true;
      }else if(MatchLiteral(new string[]{"New"})) {
        if(MatchLiteral(new string[]{"("})){
          if(MatchType("Identificador")){
            if(MatchLiteral(new string[]{")"})){
              return true;
            }
          }
        }
        return false;
      }else if(MatchLiteral(new string[]{"this"})){
        return true;
      }else if(MatchLiteral(new string[]{"("})){
        if(ParseExpr()){
          return MatchLiteral(new string[]{")"});
        }
      }else if(MatchLiteral(new string[]{"-"})){
        return ParseExpr();
      }else if(MatchLiteral(new string[]{"!"})){
        return ParseExpr();
      }else if(ParseLval()){
        if(MatchLiteral(new string[]{"="})){
          return ParseExpr();
        }
        return false;
      }else if(ParseConst()){
        return true;
      }else if(ParseLval()){
        return true;
      }
      return false;
    }
    
    
    private bool ParseLval(){
      if (MatchType("Identificador")) return true;
      else if (ParseExpr()){ 
        if (MatchLiteral(new string[]{"."})) 
          if(MatchType("Identificador"))
            return true;
      } else if (ParseExpr()){
        if (MatchLiteral(new string[]{"["}))
          if (ParseExpr())
            if(MatchLiteral(new string[]{"]"}))
              return true;
      }
      return false;
    }
    
    
    private bool ParseConst(){
      if(MatchType("Valor Hexadecimal")){
        return true;
      } else if(MatchType("Valor Hexadecimal")){
        return true;
      }else if(MatchType("Valor Exponencial")){
        return true;
      }else if(MatchType("Valor Decimal")){
        return true;
      }else if(MatchType("booleano")){
        return true;
      }else if(MatchType("Cadena de texto")){
        return true;
      }else if(MatchLiteral(new string[]{"null"})){
        return true;
      }
      return false;
    }

    #endregion
    

    private bool MatchType(string tokenType){
      if(tokensQueue.Peek().type == tokenType){
        tokensQueue.Dequeue();
        return true;
      }
      else {
        Console.WriteLine("Error, ya no se que hacer, no se con cual quedarme");
        return false;
      }
    }

    private bool MatchLiteral(string[] stringLiteral){
      if(stringLiteral.Contains(tokensQueue.Peek().Value)){
        tokensQueue.Dequeue();
        return true;
      }
      else {
        Console.WriteLine("Error, ya no se que hacer, no se con cual quedarme");
        return false;
      }
    }
  }
}