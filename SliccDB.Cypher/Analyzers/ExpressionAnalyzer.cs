using System;
using System.Linq;
using Antlr4.Runtime.Tree;
using SliccDB.Cypher.Extensions;
using SliccDB.Cypher.Model;
using SliccDB.Cypher.Utility;

namespace SliccDB.Cypher.Analyzers
{
    public class ExpressionAnalyzer : BaseAnalyzer<ExpressionModel>
    {
        public override ExpressionModel Analyze(IParseTree context)
        {

            Console.WriteLine("Entered Where Analyzer");

            var expressionContext = context as CypherParser.OC_ExpressionContext;
            if (expressionContext.oC_OrExpression() != null)
            {
                //...
                if (expressionContext.oC_OrExpression().oC_XorExpression() != null)
                {
                    if (expressionContext.oC_OrExpression().oC_XorExpression() != null)
                    {
                        //....
                    }
                }
            }

            if (expressionContext != null)
            {
                var orExpressionChildren =
                    expressionContext.children.GetChildContextsByType(typeof(CypherParser.OC_OrExpressionContext));
                if (orExpressionChildren.Count > 0)
                {
                    Console.WriteLine("Entered or expression. ");
                }

                var xorExpressionChildren =
                    expressionContext.children.GetChildContextsByType(typeof(CypherParser.OC_XorExpressionContext));
                if (xorExpressionChildren.Count > 0)
                {
                    Console.WriteLine("Entered xor expression. ");
                }
                
                var andExpressionChildren =
                    expressionContext.children.GetChildContextsByType(typeof(CypherParser.OC_AndExpressionContext));
                if (andExpressionChildren.Count > 0)
                {
                    Console.WriteLine("Entered xor expression. ");
                }

                var notExpressionChildren =
                    expressionContext.children.GetChildContextsByType(typeof(CypherParser.OC_NotExpressionContext));
                if (notExpressionChildren.Count > 0)
                {
                    Console.WriteLine("Entered not expression. ");
                }
            }

            return new ExpressionModel();
        }
    }
}