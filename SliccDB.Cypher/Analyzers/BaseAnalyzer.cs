using System;
using Antlr4.Runtime.Tree;

namespace SliccDB.Cypher.Analyzers
{
    public abstract class BaseAnalyzer<T> where T : class
    {
        public abstract T Analyze(IParseTree context);

    }
}