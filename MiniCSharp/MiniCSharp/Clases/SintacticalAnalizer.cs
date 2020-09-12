using DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Clases
{
    class SintacticalAnalizer
    {
        List<Token> tokensList;
        Stack<Token> tempStack = new Stack<Token>();

        public SintacticalAnalizer(ref List<Token> tokensList)
        {
            this.tokensList = new List<Token>(tokensList);
        }

        public bool Analize()
        {
            // int LastHash = tokensList[0].GetHashCode();
            // while (this.tokensList.Count != 0)
            // {
            //     ResultParse result = ParsePrg();
            //     if (LastHash == tokensList[0].GetHashCode())
            //     {
            //         logError(tokensList[0].Value);
            //         tokensList.RemoveAt(0);
            //     }
            // }
            // return true;


            int LastHash = tokensList[0].GetHashCode();
            while (this.tokensList.Count != 0)
            {
              ResultParse result = ParsePrg();
              if (!result.allok){
                //Hace un backtracking hasta el estado inicial
                reinsert(result.CountLevel);
                //Hace un log del error, remueve el primer dato he intanta con el siguente
                logError(tokensList[0].Value);
                tokensList.RemoveAt(0);
              }
            }
            return true;

        }

        #region Tokens
        private ResultParse ParsePrg()
        {
            int level = 0;
            ResultParse result;
            result = ParseVarD();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseD();
                if (result.allok)
                    return new ResultParse() { allok = true, CountLevel = level + 1 };
                return new ResultParse() { allok = false, CountLevel = level };
            }
            result = ParseFuncD();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseD();
                if (result.allok) return new ResultParse() { allok = true, CountLevel = level + 1 };
                new ResultParse() { allok = false, CountLevel = level  };
            }
            return new ResultParse() { allok = false, CountLevel = level }; ;
        }

        private ResultParse ParseD()
        {
            int level = 0;
            ResultParse result;
            result = ParseVarD();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseD();
                return new ResultParse() { allok = result.allok, CountLevel = level  };//------- revisar el return 
            }
            else
            {
                result = ParseFuncD();
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = ParseD();
                    if (result.allok)
                    {
                        return new ResultParse() { allok = result.allok, CountLevel = level + 1};
                    }
                    return new ResultParse() { allok = result.allok, CountLevel = level };
                }
                return new ResultParse() { allok = true, CountLevel = level + 1 }; //Returns true because this accepts nullable values Є
            }
        }


        private ResultParse ParseVarD()
        {
            int level = 0;
            ResultParse result;
            result = ParseVar();
            if (result.allok)
            {
                level += result.CountLevel;
                result = MatchLiteral(new string[] { ";" });
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = result.allok, CountLevel = level }; 
        }
        private ResultParse ParseVar()
        {
            int level = 0;
            ResultParse result;
            result = ParseType();
            if (result.allok)
            {
                level += result.CountLevel;
                result = MatchType("Identificador");
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
                return new ResultParse() { allok = result.allok, CountLevel = level };
            }
            else return new ResultParse() { allok = result.allok, CountLevel = level };
        }


        private ResultParse ParseVarPrim()
        {
            int level = 0;
            ResultParse result;
            result = ParseVar();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseVarPrim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
                return new ResultParse() { allok = result.allok, CountLevel = level };
            }
            return new ResultParse() { allok = true, CountLevel = level + 1 };
        }


        private ResultParse ParseType()
        {
            int level = 0;
            ResultParse result;
            result = ParseTypePrim();
            if (result.allok)
            {
                level = result.CountLevel;
                result = ParseTypeBiPrim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
                return new ResultParse() { allok = result.allok, CountLevel = level };
            }
            return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
        }


        private ResultParse ParseTypePrim()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "int", "double", "bool", "string" });
            if (result.allok)
            {
                level += result.CountLevel;
                return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
            }
            result = MatchType("Identificador");
            if (result.allok)
            {
                level += result.CountLevel;
                return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
            }
            return new ResultParse() { allok = result.allok, CountLevel = level };
        }


        private ResultParse ParseTypeBiPrim()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "[]" });
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseTypeBiPrim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = true, CountLevel = level + 1}; //Returns true because this accepts nullable values Є      
        }


        private ResultParse ParseFuncD()
        {
            if (ParseType())
            {
                if (MatchType("Identificador"))
                {
                    if (MatchLiteral(new string[]{ "("}))
                    {
                        if (ParseFrms())
                        {
                            if (MatchLiteral(new string[]{ ")"}))
                            {
                                if (ParseSt())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            else if (MatchLiteral(new string[] { "void" }))
            {
                if (MatchType("Identificador"))
                {
                    if (MatchLiteral(new string[] { "(" }))
                    {
                        if (ParseFrms())
                        {
                            if (MatchLiteral(new string[] { ")" }))
                            {
                                if (ParseSt())
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private ResultParse ParseFrms()
        {
            int level = 0;
            ResultParse result;
            result = ParseVar();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseVarPrim();
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = MatchLiteral(new string[] { "," });
                    if (result.allok)
                    {
                        return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                    }                    
                }
                return new ResultParse() { allok = result.allok, CountLevel = level };
            }
            return new ResultParse() { allok = true, CountLevel = level +1};//Returns true because this accepts nullable values Є      
        }


        private ResultParse ParseSt()
        {
            int level = 0;
            ResultParse result;
            result = ParseIst();
            if (result.allok)
            {
                return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
            }
            result = ParseRst();
            if (result.allok)
            {
                return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
            }
            result = ParseExpr();
            if (result.allok)
            {
                level += result.CountLevel;
                result = MatchLiteral(new string[] { ";" });
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                } 
            }
            return new ResultParse() { allok = false, CountLevel = level };
        }


        private ResultParse ParseIst()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "if" });
            if (result.allok)
            {
                level += result.CountLevel;
                result = MatchLiteral(new string[] { "(" });
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = ParseExpr();
                    if (result.allok)
                    {
                        level += result.CountLevel;
                        result = MatchLiteral(new string[] { ")" });
                        if (result.allok)
                        {
                            level += result.CountLevel;
                            result = ParseSt();
                            if (result.allok)
                            {
                                level += result.CountLevel;
                                result = ParseIstPrim();
                                if (result.allok)
                                {
                                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                                }
                            }
                        }
                    }
                }
            }
            return new ResultParse() { allok = false, CountLevel = level };
        }
        private ResultParse ParseIstPrim()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "else" });
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseSt();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
                return new ResultParse() { allok = result.allok, CountLevel = level };
            }
            return new ResultParse() { allok = true, CountLevel = level + 1 };
        }


        private ResultParse ParseRst()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "Return" });            
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseRstPrim();
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = MatchLiteral(new string[] { ";" });
                    if (result.allok)
                    {
                        return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                    }
                }
            }
            return new ResultParse() { allok = false, CountLevel = level };
        }
        private ResultParse ParseRstPrim()
        {
            int level = 0;
            ResultParse result;
            result = ParseExpr();
            if (result.allok)
            {
                return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
            }
            return new ResultParse() { allok = true, CountLevel = level + 1 };
        }


        private ResultParse ParseExpr()
        {
            int level = 0;
            ResultParse result;
            result = ParseExpr1();
            if (result.allok) 
            {
                level += result.CountLevel;
                result = ParseExprPrim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = result.allok, CountLevel = level };
        }


        private ResultParse ParseExprPrim()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "||" });
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr1();
                if (result.allok)
                {
                    level += result.CountLevel;
                    result =ParseExprPrim();
                    if (result.allok)
                    {
                        return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                    }
                }
                else return new ResultParse() { allok = result.allok, CountLevel = level };
            }
            return new ResultParse() { allok = true, CountLevel = level + 1 };
        }


        private ResultParse ParseExpr1()
        {
            int level = 0;
            ResultParse result;
            result = ParseExpr2();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr1Prim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = result.allok, CountLevel = level };
        }


        private ResultParse ParseExpr1Prim()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "&&" });
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr2();
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = ParseExpr1Prim();
                    if (result.allok)
                    {
                        return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                    }
                }
                else return new ResultParse() { allok = result.allok, CountLevel = level };
            }
            return new ResultParse() { allok = true, CountLevel = level + 1 };
        }


        private ResultParse ParseExpr2()
        {
            int level = 0;
            ResultParse result;
            result = ParseExpr3();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr2Prim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = result.allok, CountLevel = level };
        }


        private ResultParse ParseExpr2Prim()
        {
            int level = 0;
            ResultParse result;
            result = MatchLiteral(new string[] { "==" });
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr3();
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = ParseExpr2Prim();
                    if (result.allok) {
                        return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                    }
                }
                return new ResultParse() { allok = false, CountLevel = level };
            }
            result = MatchLiteral(new string[] { "!=" });
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr3();
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = ParseExpr2Prim();
                    if (result.allok)
                    {
                        return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                    }
                }
                else return new ResultParse() { allok = false, CountLevel = level };
            }
            else return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
        }


        private ResultParse ParseExpr3()
        {
            int level = 0;
            ResultParse result;
            result = ParseExpr4();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr3Prim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = result.allok, CountLevel = level };
        }


        private ResultParse ParseExpr3Prim()
        {
            if (MatchLiteral(new string[] { "<" }))
            {
                if (ParseExpr4())
                    return ParseExpr3Prim();
                else return false;
            }
            else if (MatchLiteral(new string[] { "<=" }))
            {
                if (ParseExpr4()) return ParseExpr3Prim();
                else return false;
            }
            if (MatchLiteral(new string[] { ">" }))
            {
                if (ParseExpr4()) return ParseExpr3Prim();
                else return false;
            }
            else if (MatchLiteral(new string[] { ">=" }))
            {
                if (ParseExpr4()) return ParseExpr3Prim();
                else return false;
            }
            else return true;
        }


        private ResultParse ParseExpr4()
        {
            int level = 0;
            ResultParse result;
            result = ParseExpr5();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr4Prim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = result.allok, CountLevel = level };
        }

        private ResultParse ParseExpr4Prim()
        {
            if (MatchLiteral(new string[] { "+" }))
            {
                if (ParseExpr5()) return ParseExpr4Prim();
                else return false;
            }
            else if (MatchLiteral(new string[] { "-" }))
            {
                if (ParseExpr5()) return ParseExpr4Prim();
                else return false;
            }
            else return true;
        }
        private ResultParse ParseExpr5()
        {
            int level = 0;
            ResultParse result;
            result = ParseExpr6();
            if (result.allok)
            {
                level += result.CountLevel;
                result = ParseExpr5Prim();
                if (result.allok)
                {
                    return new ResultParse() { allok = result.allok, CountLevel = level + 1 };
                }
            }
            return new ResultParse() { allok = result.allok, CountLevel = level };
        }


        private ResultParse ParseExpr5Prim()
        {
            if (MatchLiteral(new string[] { "*" }))
            {
                if (ParseExpr6()) return ParseExpr5Prim();
                else return false;
            }
            else if (MatchLiteral(new string[] { "/" }))
            {
                if (ParseExpr6()) return ParseExpr5Prim();
                else return false;
            }
            else return true;
        }


        private ResultParse ParseExpr6()
        {
            if (MatchLiteral(new string[] { "New" }))
            {
                if (MatchLiteral(new string[] { "(" }))
                {
                    if (MatchType("Identificador"))
                    {
                        if (MatchLiteral(new string[] { ")" }))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            else if (MatchLiteral(new string[] { "this" }))
            {
                return true;
            }
            else if (MatchLiteral(new string[] { "(" }))
            {
                if (ParseExpr())
                {
                    return MatchLiteral(new string[] { ")" });
                }
            }
            else if (MatchLiteral(new string[] { "-" }))
            {
                return ParseExpr();
            }
            else if (MatchLiteral(new string[] { "!" }))
            {
                return ParseExpr();
            }
            else if (ParseLval())
            {
                if (MatchLiteral(new string[] { "=" }))
                {
                    return ParseExpr();
                }
                return false;
            }
            else if (ParseConst())
            {
                return true;
            }
            else if (ParseLval())
            {
                return true;
            }
            return false;
        }


        private ResultParse ParseLval()
        {
            int level = 0;
            ResultParse result;

            result = MatchType("Identificador");
            if (result.allok)
            {
                level += result.CountLevel;
                return new ResultParse() { allok = true, CountLevel = level + 1 };
            }
            result = ParseExpr();
            if (result.allok)
            {
                level += result.CountLevel;
                result = MatchLiteral(new string[] { "." });
                if (result.allok)
                {
                    level += result.CountLevel; 
                    if (MatchType("Identificador").allok)
                        return new ResultParse() { allok = true, CountLevel = level + 1 };
                }
                return new ResultParse() { allok = false, CountLevel = level };
            }
            result = ParseExpr();
            if (result.allok)
            {
                level += result.CountLevel;
                result = MatchLiteral(new string[] { "[" });
                if (result.allok)
                {
                    level += result.CountLevel;
                    result = ParseExpr();
                    if (result.allok)
                    {
                        level += result.CountLevel;
                        result = MatchLiteral(new string[] { "]" });
                        if (result.allok)
                            return new ResultParse() { allok = true, CountLevel = level + 1 };
                    }
                }
                return new ResultParse() { allok = false, CountLevel = level };
            }
            return new ResultParse() { allok = false, CountLevel = 0 };
        }


        private ResultParse ParseConst()
        {            
            if (MatchType("Valor Hexadecimal").allok)
                return new ResultParse() { allok = true, CountLevel = 1 };
            else if (MatchType("Valor Hexadecimal").allok)
                return new ResultParse() { allok = true, CountLevel = 1 };
            else if (MatchType("Valor Exponencial").allok)
                return new ResultParse() { allok = true, CountLevel = 1 };
            else if (MatchType("Valor Decimal").allok)
                return new ResultParse() { allok = true, CountLevel = 1 };
            else if (MatchType("booleano").allok)
                return new ResultParse() { allok = true, CountLevel = 1 };
            else if (MatchType("Cadena de texto").allok)
                return new ResultParse() { allok = true, CountLevel = 1 };
            else if (MatchLiteral(new string[] { "null" }).allok)
                return new ResultParse() { allok = true, CountLevel = 1 };
            else 
                return new ResultParse() { allok = false, CountLevel = 0 };
        }

        #endregion

        private ResultParse MatchType(string tokenType)
        {
            if (tokensList[0].type == tokenType)
            {
                tempStack.Push(tokensList[0]);
                tokensList.RemoveAt(0);
                return new ResultParse() { allok = true, CountLevel = 1 };
            }
            else return new ResultParse() { allok = false, CountLevel = 0 };
        }

        private ResultParse MatchLiteral(string[] stringLiteral)
        {
            if (stringLiteral.Contains(tokensList[0].Value))
            {
                tempStack.Push(tokensList[0]);
                tokensList.RemoveAt(0);
                return new ResultParse() { allok = true, CountLevel = 1 };
            }
            else return new ResultParse() { allok = false, CountLevel = 0 };
        }


        /// <summary>Tries to parse the next function</summary>
        /// <param name="ParseFunc">Parse Function</param>
        /// <param name="reQueue">Add again in the list the last pulled out item</param>
        /// <returns>if parse was successfull</returns>
        //private ResultParse NextParse(Func<bool> ParseFunc, bool reQueue, int reintegros){
        //  if (ParseFunc()) return true; 
        //  else {

        //    if(reQueue) 
        //      

        //    logError(tokensList[0].Value);
        //    return false;
        //  }
        //}

        private void reinsert(int levels){
          for(int i = 0; i < levels; i++) 
            tokensList.Insert(0, tempStack.Pop());
        }

        private void logError(string Token)
        {
            Console.WriteLine("Se encontro un token inesperado {0}", Token);
        }
    }
}