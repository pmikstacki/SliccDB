using System;
using Antlr4.Runtime.Tree;
using SliccDB.Cypher.Model;

namespace SliccDB.Cypher.Analyzers
{
    public class LiteralAnalyzer : BaseAnalyzer<LiteralModel>
    {
        public override LiteralModel Analyze(IParseTree context)
        {
            Console.WriteLine("Entered Literal Analyzer: "+context);
            return new LiteralModel();
        }

        private LiteralType InfereLiteralType(string LiteralText)
        {
            return LiteralType.STRING;
        }
    }
}