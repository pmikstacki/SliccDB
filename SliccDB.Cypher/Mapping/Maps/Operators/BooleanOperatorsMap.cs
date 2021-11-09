using System;
using SliccDB.Cypher.Mapping.Base;
using SliccDB.Cypher.Model;

namespace SliccDB.Cypher.Mapping.Maps
{
    public class BooleanOperatorsMap : Map<string, BooleanOperators>
    {
        public override BooleanOperators Project(string src)
        {
            switch (src)
            {
                case "XOR": return BooleanOperators.XOR;
                case "AND": return BooleanOperators.AND;
                case "NOT": return BooleanOperators.NOT;
                case "OR": return BooleanOperators.OR;
                default: throw new Exception("Requested BooleanOperator is not defined. BooleanOperator: " + src);
            }
        }
    }
}