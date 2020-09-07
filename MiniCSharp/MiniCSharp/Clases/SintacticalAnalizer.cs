using System.Reflection.Metadata.Ecma335;
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
    public SintacticalAnalizer(out Queue<Token> tokensQueue){
      tokensQueue = new Queue<Token>(this.tokensQueue);
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
      if (Matched) return MatchLiteral(new string[]{","});
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
      bool Matched = false;
      return Matched;
    }
    private bool ParseType(){
      bool Matched = ParseTypePrim();
      if(Matched) return ParseTypeBiPrim();
      return false;
    }
    private bool ParseTypePrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseTypeBiPrim(){
      bool Matched = MatchLiteral(new string[]{"[]"});
      if(Matched) return ParseTypeBiPrim();
      return true;//Returns true because this accepts nullable values Є      
    }
    private bool ParseFuncD(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseFrms(){
      bool Matched = ParseVar();
      if(ParseVarPrim()) return MatchLiteral(new string[]{","});
      else return true;//Returns true because this accepts nullable values Є      
    }
    private bool ParseSt(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseIst(){
      bool Matched = MatchLiteral(new string[]{"if"});
      if(Matched){
        if(MatchLiteral(new string[]{"("})){
          if(ParseExpr()){
            if(MatchLiteral(new string[]{")"})){                
              if(ParseSt()){
                return ParseIstPrim();
              }
            }          
          }
        }
      }
      return false;
    }
    private bool ParseIstPrim(){
      bool Matched = false;
      return Matched;
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
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr(){
      bool Matched = ParseExpr1();
      if(Matched){
        return ParseExprPrim();
      }
      return false;
    }
    private bool ParseExprPrim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr1(){
      bool Matched = ParseExpr2();
      if(Matched){
        return ParseExpr1Prim();
      }
      return false;
    }
    private bool ParseExpr1Prim(){
      bool Matched = false;
      return Matched;
    }
    private bool ParseExpr2(){
      bool Matched = ParseExpr3();
      if(Matched){
        return ParseExpr2Prim();
      }
      return false;
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