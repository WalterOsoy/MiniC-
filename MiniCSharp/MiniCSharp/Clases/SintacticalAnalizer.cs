using System;
using System.Collections.Generic;
using DataStructures;

namespace Clases
{
  class SintacticalAnalizer
  {

    Queue<Token> tokensQueue;
    public SintacticalAnalizer(out Queue<Token> tokensQueue){
      this.tokensQueue = tokensQueue;
    }


    public bool Analize(){

      return true;
    }    
    #region Tokens

    private bool ParsePrg(){
      bool Matched = ParseVarD();

      if (Matched) {
        return ParseD();
      }
      else{
        Matched = ParseFuncD();
        if (Matched) return ParseD();
        else return false;
      }
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
      bool Matched = ParseVar();
      if (Matched) return Match(";");
      else return false;
    }

    private bool ParseVar(){
      bool Matched = ParseType();
      if (Matched) return Match('');
      {
          
      }

    }
    private bool ParseVarPrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseType(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseTypePrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseTypeBiPrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseFuncD(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseFrms(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseSt(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseIst(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseIstPrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseRst(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseRstPrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExprPrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr1(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr1Prim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr2(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr2Prim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr3(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr3Prim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr4(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr4Prim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr5(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr5Prim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr6(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseLval(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseConst(){
      bool Matched = false;
      return Matched;
    }

    #endregion
    

    private bool MatchType(string tokenType){
      if(queue.peek().Type == tokenType){
        queue.dequeue();
        return true;
      }
      else {
        Console.WriteLine("Error, ya no se que hacer, no se con cual quedarme");
        return false;
      }
    }

    private bool MatchLiteral(string[] stringLiteral){
      if(stringLiteral.contains(queue.peek().Value)){
        queue.dequeue();
        return true;
      }
      else {
        Console.WriteLine("Error, ya no se que hacer, no se con cual quedarme");
        return false;
      }
    }
  }
}