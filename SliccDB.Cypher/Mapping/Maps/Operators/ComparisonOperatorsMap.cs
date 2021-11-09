using System;
using SliccDB.Cypher.Mapping.Base;
using SliccDB.Cypher.Model;

namespace SliccDB.Cypher.Mapping.Maps
{
    public class ComparisonOperatorsMap : Map<string, ComparisonOperators>
    {
        public override ComparisonOperators Project(string src)
        {
            switch (src)
            {
                case "=": return ComparisonOperators.EQUAL;
                case "<>": return ComparisonOperators.NOT_EQUAL;
                case ">": return ComparisonOperators.MORE_THAN;
                case "<": return ComparisonOperators.LESS_THAN;
                case ">=": return ComparisonOperators.MORE_THAN_OR_EQUAL;
                case "<=": return ComparisonOperators.LESS_THAN_OR_EQUAL;
                case "IS NULL": return ComparisonOperators.IS_NULL;
                case "IS NOT NULL": return ComparisonOperators.IS_NOT_NULL;
                default: throw new Exception("Requested ComparisonOperator is not defined. ComparisonOperator: " + src);
            }
        }
    }
}