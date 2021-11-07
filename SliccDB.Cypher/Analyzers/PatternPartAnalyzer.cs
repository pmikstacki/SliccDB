using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using SliccDB.Cypher.Model;

namespace SliccDB.Cypher.Analyzers
{
    public class PatternPartAnalyzer : BaseAnalyzer<PatternPartModel>
    {
        public override PatternPartModel Analyze(IParseTree context)
        {
            CypherParser.OC_PatternPartContext partContext = context as CypherParser.OC_PatternPartContext;

            Console.WriteLine("Found Pattern Part" + partContext.GetText());
            var patternPartModel = new PatternPartModel();

            if (!partContext.oC_AnonymousPatternPart().IsEmpty)
            {
                Console.WriteLine("Found Anonymous Pattern Part" + partContext.oC_AnonymousPatternPart().ToString());
                if (!partContext.oC_AnonymousPatternPart().oC_PatternElement().IsEmpty)
                {
                    var patternElement = partContext.oC_AnonymousPatternPart().oC_PatternElement();



                    if (!patternElement.oC_NodePattern().IsEmpty)
                    {
                        Console.WriteLine("Found Node Pattern");

                        var nodePattern = patternElement.oC_NodePattern();


                        if (nodePattern.oC_Variable() != null)
                        {
                            var variable = nodePattern.oC_Variable();

                            Console.WriteLine("Found Variable Definition " + variable.GetText());
                            if (variable.oC_SymbolicName() != null)
                            {
                                Console.WriteLine(
                                    "Found Variable Symbolic Name " + variable.oC_SymbolicName().GetText());
                                patternPartModel.VariableName = variable.oC_SymbolicName().GetText();
                            }

                        }

                        if (nodePattern.oC_NodeLabels() != null && !nodePattern.oC_NodeLabels().IsEmpty)
                        {
                            Console.WriteLine("Found Node Labels");
                            nodePattern.oC_NodeLabels().oC_NodeLabel().ToList().ForEach(nodeLabel =>
                            {
                                Console.WriteLine("Found Node Label Definition: " + nodeLabel.GetText());
                                if (!nodeLabel.oC_LabelName().IsEmpty)
                                {
                                    Console.WriteLine("Found Node Label Name: " + nodeLabel.oC_LabelName().GetText());
                                    patternPartModel.Labels.Add(nodeLabel.oC_LabelName().GetText());
                                }
                            });
                        }

                        if (nodePattern.oC_Properties() != null)
                        {
                            Console.WriteLine("Found Properties");

                            var properties = nodePattern.oC_Properties();


                            if (properties.oC_MapLiteral() != null)
                            {
                                Console.WriteLine("Found Map Literals " + properties.oC_MapLiteral().GetText());
                                var propertyKeys = properties.oC_MapLiteral().oC_PropertyKeyName();
                                var propertyValues = properties.oC_MapLiteral().oC_Expression();
                                for (int i = 0; i < properties.oC_MapLiteral().oC_PropertyKeyName().Length; i++)
                                {
                                    patternPartModel.Properties.Add(propertyKeys[i].GetText(),
                                        propertyValues[i].GetText().Replace("'", ""));
                                }


                                if (properties.oC_MapLiteral().oC_PropertyKeyName() != null)
                                {
                                    foreach (var ocPropertyKeyNameContext in properties.oC_MapLiteral()
                                        .oC_PropertyKeyName())
                                    {
                                        Console.WriteLine("Found Map Literal " + ocPropertyKeyNameContext.GetText());

                                    }
                                }

                                if (properties.oC_MapLiteral().oC_Expression() != null)
                                {
                                    var expression = properties.oC_MapLiteral().oC_Expression();

                                    foreach (var ocExpressionContext in expression)
                                    {
                                        Console.WriteLine("Found Expression " + ocExpressionContext.GetText());
                                    }
                                }

                            }
                        }
                    }

                    if (partContext.oC_Variable() != null)
                    {
                        Console.WriteLine("Variable Definition " + partContext.oC_Variable().GetText());
                        if (!partContext.oC_Variable().oC_SymbolicName().IsEmpty)
                        {
                            Console.WriteLine("Symbolic Name " + partContext.oC_Variable().oC_SymbolicName().GetText());

                            patternPartModel.VariableName = partContext.oC_Variable().oC_SymbolicName().GetText();

                        }

                    }

                }
            }

            return patternPartModel;
        }
    }
}