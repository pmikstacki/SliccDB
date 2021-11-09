using System;
using System.Collections;
using System.Collections.Generic;
using Antlr4.Runtime.Tree;

namespace SliccDB.Cypher.Extensions
{
    public static class ExpressionContextExtensions
    {
        public static IList<IParseTree> GetChildContextsByType(this IList<IParseTree> treeList, Type requiredType)
        {
            var foundTypes = new List<IParseTree>();

            foreach (var parseTree in treeList)
            {
                if (parseTree.GetType() == requiredType)
                {
                    foundTypes.Add(parseTree);
                }
            }

            return foundTypes;  
        }
    }
}