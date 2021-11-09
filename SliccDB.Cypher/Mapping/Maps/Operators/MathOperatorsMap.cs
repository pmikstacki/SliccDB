using System;
using SliccDB.Cypher.Mapping.Base;
using SliccDB.Cypher.Model;

namespace SliccDB.Cypher.Mapping.Maps
{
    public class MathOperatorsMap : Map<string, MathOperators>
    {
        public override MathOperators Project(string src)
        {
            switch (src)
            {
                case "^": return MathOperators.POWER;
                case "*": return MathOperators.MULTIPLICATION;
                case "/": return MathOperators.DIVISION;
                case "%": return MathOperators.MODULO;
                case "+": return MathOperators.ADDITION;
                case "-": return MathOperators.SUBSTRACTION;
                default: throw new Exception("Requested MathOperator is not defined. MathOperator: "+src);
            }
        }
    }
}