using System;
using SliccDB.Cypher.Mapping.Base;
using SliccDB.Cypher.Model;

namespace SliccDB.Cypher.Mapping.Maps
{
    public class StringOperatorsMap : Map<string, StringOperators>
    {
        public override StringOperators Project(string src)
        {
            switch (src)
            {
                case "STARTS WITH": return StringOperators.STARTS_WITH;
                case "ENDS WITH": return StringOperators.ENDS_WITH;
                case "CONTAINS": return StringOperators.CONTAINS;
                default: throw new Exception("Requested StringOperator is not defined. StringOperator: " + src);
            }
        }
    }
}