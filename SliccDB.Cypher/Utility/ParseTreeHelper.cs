using System;
using Antlr4.Runtime.Tree;

namespace SliccDB.Cypher.Utility
{
    public static class ParseTreeHelper
    {
        public static void PrintContext(IParseTree tree, int indent)
        {
            string indentString = "";
            for (int i = 1; i <= indent; i++)
            {
                indentString += " ";
            }
            for(int i = 0;i< tree.ChildCount;i++)
            {
                var child = tree.GetChild(i);
                Console.WriteLine($"{indentString}Type: {child.GetType().Name}, Value: {child.GetText()}");
                PrintContext(child, indent++);
            }
        }
    }
}