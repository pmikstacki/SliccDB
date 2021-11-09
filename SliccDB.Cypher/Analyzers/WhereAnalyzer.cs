using System;
using Antlr4.Runtime.Tree;
using SliccDB.Cypher.Model;

namespace SliccDB.Cypher.Analyzers
{
    public class WhereAnalyzer : BaseAnalyzer<WhereModel>
    {
        public override WhereModel Analyze(IParseTree context)
        {
            Console.WriteLine("Entered Where Analyzer");
            var whereContext = context as CypherParser.OC_WhereContext;
            if (whereContext.oC_Expression() != null)
            {
                var expressionData = new ExpressionAnalyzer().Analyze(whereContext.oC_Expression());


            }

            return new WhereModel();
        }
    }
}