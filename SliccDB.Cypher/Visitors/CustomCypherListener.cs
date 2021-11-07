using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using SliccDB.Core;
using SliccDB.Cypher.Model;
using SliccDB.Serialization;

namespace SliccDB.Cypher.Visitors
{
    public class CustomCypherListener : CypherBaseListener
    {

        public DatabaseConnection CurrentDatabaseConnection;
        private Dictionary<string, object> _variableBuffer = new Dictionary<string, object>();
        public QueryResult CypherQueryResult { get; set; }
        public override void EnterOC_Cypher(CypherParser.OC_CypherContext context)
        {
            Console.WriteLine("Entered Cypher");
            base.EnterOC_Cypher(context);
        }

        public override void ExitOC_Cypher(CypherParser.OC_CypherContext context)
        {
            Console.WriteLine("Exited Cypher");
            _variableBuffer.Clear();
            base.ExitOC_Cypher(context);
        }

        public override void EnterOC_Statement(CypherParser.OC_StatementContext context)
        {
            base.EnterOC_Statement(context);
        }

        public override void ExitOC_Statement(CypherParser.OC_StatementContext context)
        {
            base.ExitOC_Statement(context);
        }

        public override void EnterOC_Query(CypherParser.OC_QueryContext context)
        {
            base.EnterOC_Query(context);
        }

        public override void ExitOC_Query(CypherParser.OC_QueryContext context)
        {
            base.ExitOC_Query(context);
        }

        public override void EnterOC_RegularQuery(CypherParser.OC_RegularQueryContext context)
        {
            base.EnterOC_RegularQuery(context);
        }

        public override void ExitOC_RegularQuery(CypherParser.OC_RegularQueryContext context)
        {
            base.ExitOC_RegularQuery(context);
        }

        public override void EnterOC_Union(CypherParser.OC_UnionContext context)
        {
            base.EnterOC_Union(context);
        }

        public override void ExitOC_Union(CypherParser.OC_UnionContext context)
        {
            base.ExitOC_Union(context);
        }

        public override void EnterOC_SingleQuery(CypherParser.OC_SingleQueryContext context)
        {
            base.EnterOC_SingleQuery(context);
        }

        public override void ExitOC_SingleQuery(CypherParser.OC_SingleQueryContext context)
        {
            base.ExitOC_SingleQuery(context);
        }

        public override void EnterOC_SinglePartQuery(CypherParser.OC_SinglePartQueryContext context)
        {
            base.EnterOC_SinglePartQuery(context);
        }

        public override void ExitOC_SinglePartQuery(CypherParser.OC_SinglePartQueryContext context)
        {
            base.ExitOC_SinglePartQuery(context);
        }

        public override void EnterOC_MultiPartQuery(CypherParser.OC_MultiPartQueryContext context)
        {
            base.EnterOC_MultiPartQuery(context);
        }

        public override void ExitOC_MultiPartQuery(CypherParser.OC_MultiPartQueryContext context)
        {
            base.ExitOC_MultiPartQuery(context);
        }

        public override void EnterOC_UpdatingClause(CypherParser.OC_UpdatingClauseContext context)
        {
            base.EnterOC_UpdatingClause(context);
        }

        public override void ExitOC_UpdatingClause(CypherParser.OC_UpdatingClauseContext context)
        {
            base.ExitOC_UpdatingClause(context);
        }

        public override void EnterOC_ReadingClause(CypherParser.OC_ReadingClauseContext context)
        {
            base.EnterOC_ReadingClause(context);
        }

        public override void ExitOC_ReadingClause(CypherParser.OC_ReadingClauseContext context)
        {
            base.ExitOC_ReadingClause(context);
        }

        public override void EnterOC_Match(CypherParser.OC_MatchContext context)
        {
            base.EnterOC_Match(context);
        }

        public override void ExitOC_Match(CypherParser.OC_MatchContext context)
        {
            base.ExitOC_Match(context);
        }

        public override void EnterOC_Unwind(CypherParser.OC_UnwindContext context)
        {
            base.EnterOC_Unwind(context);
        }

        public override void ExitOC_Unwind(CypherParser.OC_UnwindContext context)
        {
            base.ExitOC_Unwind(context);
        }

        public override void EnterOC_Merge(CypherParser.OC_MergeContext context)
        {
            base.EnterOC_Merge(context);
        }

        public override void ExitOC_Merge(CypherParser.OC_MergeContext context)
        {
            base.ExitOC_Merge(context);
        }

        public override void EnterOC_MergeAction(CypherParser.OC_MergeActionContext context)
        {
            base.EnterOC_MergeAction(context);
        }

        public override void ExitOC_MergeAction(CypherParser.OC_MergeActionContext context)
        {
            base.ExitOC_MergeAction(context);
        }

        public override void EnterOC_Create(CypherParser.OC_CreateContext context)
        {
            Console.WriteLine("Entered Create");

            if(context.oC_Pattern().oC_PatternPart().Length > 0) Console.WriteLine("Found Pattern Parts");

            context.oC_Pattern().oC_PatternPart().ToList().ForEach(a =>
            {
                Console.WriteLine("Found Pattern Part" + a.GetText());
                var nodeProperties = new Dictionary<string, string>();
                var nodeLabels = new HashSet<string>();
                var nodeBufferName = "";

                if (!a.oC_AnonymousPatternPart().IsEmpty)
                {
                    Console.WriteLine("Found Anonymous Pattern Part" + a.oC_AnonymousPatternPart().ToString());
                    if (!a.oC_AnonymousPatternPart().oC_PatternElement().IsEmpty)
                    {
                        var patternElement = a.oC_AnonymousPatternPart().oC_PatternElement();



                        if (!patternElement.oC_NodePattern().IsEmpty)
                        {
                            Console.WriteLine("Found Node Pattern");

                            var nodePattern = patternElement.oC_NodePattern();


                            if (nodePattern.oC_Variable() != null)
                            {
                                var variable = nodePattern.oC_Variable();

                                Console.WriteLine("Found Variable Definition "+variable.GetText());
                                if (variable.oC_SymbolicName() != null)
                                {
                                    Console.WriteLine("Found Variable Symbolic Name " + variable.oC_SymbolicName().GetText());
                                    nodeBufferName = variable.oC_SymbolicName().GetText();
                                }

                            }

                            if (!nodePattern.oC_NodeLabels().IsEmpty)
                            {
                                Console.WriteLine("Found Node Labels");
                                nodePattern.oC_NodeLabels().oC_NodeLabel().ToList().ForEach(nodeLabel =>
                                {
                                    Console.WriteLine("Found Node Label Definition: "+nodeLabel.GetText());
                                    if (!nodeLabel.oC_LabelName().IsEmpty)
                                    {
                                        Console.WriteLine("Found Node Label Name: " + nodeLabel.oC_LabelName().GetText());
                                        nodeLabels.Add(nodeLabel.oC_LabelName().GetText());
                                    }
                                });
                            }

                            if (nodePattern.oC_Properties() != null)
                            {
                                Console.WriteLine("Found Properties");

                                var properties = nodePattern.oC_Properties();


                                if (properties.oC_MapLiteral() != null)
                                {
                                    Console.WriteLine("Found Map Literals "+ properties.oC_MapLiteral().GetText());
                                    var propertyKeys = properties.oC_MapLiteral().oC_PropertyKeyName();
                                    var propertyValues = properties.oC_MapLiteral().oC_Expression();
                                    for (int i = 0; i < properties.oC_MapLiteral().oC_PropertyKeyName().Length; i++)
                                    {
                                        nodeProperties.Add(propertyKeys[i].GetText(), propertyValues[i].GetText().Replace("'", ""));
                                    }


                                    if (properties.oC_MapLiteral().oC_PropertyKeyName() != null)
                                    {
                                        foreach (var ocPropertyKeyNameContext in properties.oC_MapLiteral().oC_PropertyKeyName())
                                        {
                                            Console.WriteLine("Found Map Literal "+ocPropertyKeyNameContext.GetText());
                                            
                                        }
                                    }

                                    if (properties.oC_MapLiteral().oC_Expression() != null)
                                    {
                                        var expression = properties.oC_MapLiteral().oC_Expression();

                                        foreach (var ocExpressionContext in expression)
                                        {
                                            Console.WriteLine("Found Expression "+ocExpressionContext.GetText());
                                        }
                                    }

                                }
                            }


                           
                        }
                        if (a.oC_Variable() != null)
                        {
                            Console.WriteLine("Variable Definition " + a.oC_Variable().GetText());
                            if (!a.oC_Variable().oC_SymbolicName().IsEmpty)
                            {
                                Console.WriteLine("Symbolic Name " + a.oC_Variable().oC_SymbolicName().GetText());

                                //var.Name = a.oC_Variable().oC_SymbolicName().GetText();

                            }

                        }

                    }
                }
                if (a.oC_Variable() != null)
                {
                    if (a.oC_Variable().oC_SymbolicName() != null)
                    {
                        Console.WriteLine("Variable Definition " + a.oC_Variable().oC_SymbolicName().GetText());
                       // var.Name = a.oC_Variable().oC_SymbolicName().GetText(); 
                    }
                 
                }

                var node = CurrentDatabaseConnection.CreateNode(nodeProperties, nodeLabels);
                CypherQueryResult.Nodes.Add(node);
                if (!string.IsNullOrEmpty(nodeBufferName))
                {
                    _variableBuffer.Add(nodeBufferName, node);
                }
            });

            base.EnterOC_Create(context);
        }

        public override void ExitOC_Create(CypherParser.OC_CreateContext context)
        {
            Console.WriteLine("Exited Create");

            base.ExitOC_Create(context);
        }

        public override void EnterOC_Set(CypherParser.OC_SetContext context)
        {
            base.EnterOC_Set(context);
        }

        public override void ExitOC_Set(CypherParser.OC_SetContext context)
        {
            base.ExitOC_Set(context);
        }

        public override void EnterOC_SetItem(CypherParser.OC_SetItemContext context)
        {
            base.EnterOC_SetItem(context);
        }

        public override void ExitOC_SetItem(CypherParser.OC_SetItemContext context)
        {
            base.ExitOC_SetItem(context);
        }

        public override void EnterOC_Delete(CypherParser.OC_DeleteContext context)
        {
            base.EnterOC_Delete(context);
        }

        public override void ExitOC_Delete(CypherParser.OC_DeleteContext context)
        {
            base.ExitOC_Delete(context);
        }

        public override void EnterOC_Remove(CypherParser.OC_RemoveContext context)
        {
            base.EnterOC_Remove(context);
        }

        public override void ExitOC_Remove(CypherParser.OC_RemoveContext context)
        {
            base.ExitOC_Remove(context);
        }

        public override void EnterOC_RemoveItem(CypherParser.OC_RemoveItemContext context)
        {
            base.EnterOC_RemoveItem(context);
        }

        public override void ExitOC_RemoveItem(CypherParser.OC_RemoveItemContext context)
        {
            base.ExitOC_RemoveItem(context);
        }

        public override void EnterOC_InQueryCall(CypherParser.OC_InQueryCallContext context)
        {
            base.EnterOC_InQueryCall(context);
        }

        public override void ExitOC_InQueryCall(CypherParser.OC_InQueryCallContext context)
        {
            base.ExitOC_InQueryCall(context);
        }

        public override void EnterOC_StandaloneCall(CypherParser.OC_StandaloneCallContext context)
        {
            base.EnterOC_StandaloneCall(context);
        }

        public override void ExitOC_StandaloneCall(CypherParser.OC_StandaloneCallContext context)
        {
            base.ExitOC_StandaloneCall(context);
        }

        public override void EnterOC_YieldItems(CypherParser.OC_YieldItemsContext context)
        {
            base.EnterOC_YieldItems(context);
        }

        public override void ExitOC_YieldItems(CypherParser.OC_YieldItemsContext context)
        {
            base.ExitOC_YieldItems(context);
        }

        public override void EnterOC_YieldItem(CypherParser.OC_YieldItemContext context)
        {
            base.EnterOC_YieldItem(context);
        }

        public override void ExitOC_YieldItem(CypherParser.OC_YieldItemContext context)
        {
            base.ExitOC_YieldItem(context);
        }

        public override void EnterOC_With(CypherParser.OC_WithContext context)
        {
            base.EnterOC_With(context);
        }

        public override void ExitOC_With(CypherParser.OC_WithContext context)
        {
            base.ExitOC_With(context);
        }

        public override void EnterOC_Return(CypherParser.OC_ReturnContext context)
        {
            base.EnterOC_Return(context);
        }

        public override void ExitOC_Return(CypherParser.OC_ReturnContext context)
        {
            base.ExitOC_Return(context);
        }

        public override void EnterOC_ProjectionBody(CypherParser.OC_ProjectionBodyContext context)
        {
            base.EnterOC_ProjectionBody(context);
        }

        public override void ExitOC_ProjectionBody(CypherParser.OC_ProjectionBodyContext context)
        {
            base.ExitOC_ProjectionBody(context);
        }

        public override void EnterOC_ProjectionItems(CypherParser.OC_ProjectionItemsContext context)
        {
            base.EnterOC_ProjectionItems(context);
        }

        public override void ExitOC_ProjectionItems(CypherParser.OC_ProjectionItemsContext context)
        {
            base.ExitOC_ProjectionItems(context);
        }

        public override void EnterOC_ProjectionItem(CypherParser.OC_ProjectionItemContext context)
        {
            base.EnterOC_ProjectionItem(context);
        }

        public override void ExitOC_ProjectionItem(CypherParser.OC_ProjectionItemContext context)
        {
            base.ExitOC_ProjectionItem(context);
        }

        public override void EnterOC_Order(CypherParser.OC_OrderContext context)
        {
            base.EnterOC_Order(context);
        }

        public override void ExitOC_Order(CypherParser.OC_OrderContext context)
        {
            base.ExitOC_Order(context);
        }

        public override void EnterOC_Skip(CypherParser.OC_SkipContext context)
        {
            base.EnterOC_Skip(context);
        }

        public override void ExitOC_Skip(CypherParser.OC_SkipContext context)
        {
            base.ExitOC_Skip(context);
        }

        public override void EnterOC_Limit(CypherParser.OC_LimitContext context)
        {
            base.EnterOC_Limit(context);
        }

        public override void ExitOC_Limit(CypherParser.OC_LimitContext context)
        {
            base.ExitOC_Limit(context);
        }

        public override void EnterOC_SortItem(CypherParser.OC_SortItemContext context)
        {
            base.EnterOC_SortItem(context);
        }

        public override void ExitOC_SortItem(CypherParser.OC_SortItemContext context)
        {
            base.ExitOC_SortItem(context);
        }

        public override void EnterOC_Where(CypherParser.OC_WhereContext context)
        {
            base.EnterOC_Where(context);
        }

        public override void ExitOC_Where(CypherParser.OC_WhereContext context)
        {
            base.ExitOC_Where(context);
        }

        public override void EnterOC_Pattern(CypherParser.OC_PatternContext context)
        {
            base.EnterOC_Pattern(context);
        }

        public override void ExitOC_Pattern(CypherParser.OC_PatternContext context)
        {
            base.ExitOC_Pattern(context);
        }

        public override void EnterOC_PatternPart(CypherParser.OC_PatternPartContext context)
        {
            base.EnterOC_PatternPart(context);
        }

        public override void ExitOC_PatternPart(CypherParser.OC_PatternPartContext context)
        {
            base.ExitOC_PatternPart(context);
        }

        public override void EnterOC_AnonymousPatternPart(CypherParser.OC_AnonymousPatternPartContext context)
        {
            base.EnterOC_AnonymousPatternPart(context);
        }

        public override void ExitOC_AnonymousPatternPart(CypherParser.OC_AnonymousPatternPartContext context)
        {
            base.ExitOC_AnonymousPatternPart(context);
        }

        public override void EnterOC_PatternElement(CypherParser.OC_PatternElementContext context)
        {
            base.EnterOC_PatternElement(context);
        }

        public override void ExitOC_PatternElement(CypherParser.OC_PatternElementContext context)
        {
            base.ExitOC_PatternElement(context);
        }

        public override void EnterOC_NodePattern(CypherParser.OC_NodePatternContext context)
        {
            base.EnterOC_NodePattern(context);
        }

        public override void ExitOC_NodePattern(CypherParser.OC_NodePatternContext context)
        {
            base.ExitOC_NodePattern(context);
        }

        public override void EnterOC_PatternElementChain(CypherParser.OC_PatternElementChainContext context)
        {
            base.EnterOC_PatternElementChain(context);
        }

        public override void ExitOC_PatternElementChain(CypherParser.OC_PatternElementChainContext context)
        {
            base.ExitOC_PatternElementChain(context);
        }

        public override void EnterOC_RelationshipPattern(CypherParser.OC_RelationshipPatternContext context)
        {
            base.EnterOC_RelationshipPattern(context);
        }

        public override void ExitOC_RelationshipPattern(CypherParser.OC_RelationshipPatternContext context)
        {
            base.ExitOC_RelationshipPattern(context);
        }

        public override void EnterOC_RelationshipDetail(CypherParser.OC_RelationshipDetailContext context)
        {
            base.EnterOC_RelationshipDetail(context);
        }

        public override void ExitOC_RelationshipDetail(CypherParser.OC_RelationshipDetailContext context)
        {
            base.ExitOC_RelationshipDetail(context);
        }

        public override void EnterOC_Properties(CypherParser.OC_PropertiesContext context)
        {
            base.EnterOC_Properties(context);
        }

        public override void ExitOC_Properties(CypherParser.OC_PropertiesContext context)
        {
            base.ExitOC_Properties(context);
        }

        public override void EnterOC_RelationshipTypes(CypherParser.OC_RelationshipTypesContext context)
        {
            base.EnterOC_RelationshipTypes(context);
        }

        public override void ExitOC_RelationshipTypes(CypherParser.OC_RelationshipTypesContext context)
        {
            base.ExitOC_RelationshipTypes(context);
        }

        public override void EnterOC_NodeLabels(CypherParser.OC_NodeLabelsContext context)
        {
            base.EnterOC_NodeLabels(context);
        }

        public override void ExitOC_NodeLabels(CypherParser.OC_NodeLabelsContext context)
        {
            base.ExitOC_NodeLabels(context);
        }

        public override void EnterOC_NodeLabel(CypherParser.OC_NodeLabelContext context)
        {
            base.EnterOC_NodeLabel(context);
        }

        public override void ExitOC_NodeLabel(CypherParser.OC_NodeLabelContext context)
        {
            base.ExitOC_NodeLabel(context);
        }

        public override void EnterOC_RangeLiteral(CypherParser.OC_RangeLiteralContext context)
        {
            base.EnterOC_RangeLiteral(context);
        }

        public override void ExitOC_RangeLiteral(CypherParser.OC_RangeLiteralContext context)
        {
            base.ExitOC_RangeLiteral(context);
        }

        public override void EnterOC_LabelName(CypherParser.OC_LabelNameContext context)
        {
            base.EnterOC_LabelName(context);
        }

        public override void ExitOC_LabelName(CypherParser.OC_LabelNameContext context)
        {
            base.ExitOC_LabelName(context);
        }

        public override void EnterOC_RelTypeName(CypherParser.OC_RelTypeNameContext context)
        {
            base.EnterOC_RelTypeName(context);
        }

        public override void ExitOC_RelTypeName(CypherParser.OC_RelTypeNameContext context)
        {
            base.ExitOC_RelTypeName(context);
        }

        public override void EnterOC_Expression(CypherParser.OC_ExpressionContext context)
        {
            base.EnterOC_Expression(context);
        }

        public override void ExitOC_Expression(CypherParser.OC_ExpressionContext context)
        {
            base.ExitOC_Expression(context);
        }

        public override void EnterOC_OrExpression(CypherParser.OC_OrExpressionContext context)
        {
            base.EnterOC_OrExpression(context);
        }

        public override void ExitOC_OrExpression(CypherParser.OC_OrExpressionContext context)
        {
            base.ExitOC_OrExpression(context);
        }

        public override void EnterOC_XorExpression(CypherParser.OC_XorExpressionContext context)
        {
            base.EnterOC_XorExpression(context);
        }

        public override void ExitOC_XorExpression(CypherParser.OC_XorExpressionContext context)
        {
            base.ExitOC_XorExpression(context);
        }

        public override void EnterOC_AndExpression(CypherParser.OC_AndExpressionContext context)
        {
            base.EnterOC_AndExpression(context);
        }

        public override void ExitOC_AndExpression(CypherParser.OC_AndExpressionContext context)
        {
            base.ExitOC_AndExpression(context);
        }

        public override void EnterOC_NotExpression(CypherParser.OC_NotExpressionContext context)
        {
            base.EnterOC_NotExpression(context);
        }

        public override void ExitOC_NotExpression(CypherParser.OC_NotExpressionContext context)
        {
            base.ExitOC_NotExpression(context);
        }

        public override void EnterOC_ComparisonExpression(CypherParser.OC_ComparisonExpressionContext context)
        {
            base.EnterOC_ComparisonExpression(context);
        }

        public override void ExitOC_ComparisonExpression(CypherParser.OC_ComparisonExpressionContext context)
        {
            base.ExitOC_ComparisonExpression(context);
        }

        public override void EnterOC_AddOrSubtractExpression(CypherParser.OC_AddOrSubtractExpressionContext context)
        {
            base.EnterOC_AddOrSubtractExpression(context);
        }

        public override void ExitOC_AddOrSubtractExpression(CypherParser.OC_AddOrSubtractExpressionContext context)
        {
            base.ExitOC_AddOrSubtractExpression(context);
        }

        public override void EnterOC_MultiplyDivideModuloExpression(CypherParser.OC_MultiplyDivideModuloExpressionContext context)
        {
            base.EnterOC_MultiplyDivideModuloExpression(context);
        }

        public override void ExitOC_MultiplyDivideModuloExpression(CypherParser.OC_MultiplyDivideModuloExpressionContext context)
        {
            base.ExitOC_MultiplyDivideModuloExpression(context);
        }

        public override void EnterOC_PowerOfExpression(CypherParser.OC_PowerOfExpressionContext context)
        {
            base.EnterOC_PowerOfExpression(context);
        }

        public override void ExitOC_PowerOfExpression(CypherParser.OC_PowerOfExpressionContext context)
        {
            base.ExitOC_PowerOfExpression(context);
        }

        public override void EnterOC_UnaryAddOrSubtractExpression(CypherParser.OC_UnaryAddOrSubtractExpressionContext context)
        {
            base.EnterOC_UnaryAddOrSubtractExpression(context);
        }

        public override void ExitOC_UnaryAddOrSubtractExpression(CypherParser.OC_UnaryAddOrSubtractExpressionContext context)
        {
            base.ExitOC_UnaryAddOrSubtractExpression(context);
        }

        public override void EnterOC_StringListNullOperatorExpression(CypherParser.OC_StringListNullOperatorExpressionContext context)
        {
            base.EnterOC_StringListNullOperatorExpression(context);
        }

        public override void ExitOC_StringListNullOperatorExpression(CypherParser.OC_StringListNullOperatorExpressionContext context)
        {
            base.ExitOC_StringListNullOperatorExpression(context);
        }

        public override void EnterOC_ListOperatorExpression(CypherParser.OC_ListOperatorExpressionContext context)
        {
            base.EnterOC_ListOperatorExpression(context);
        }

        public override void ExitOC_ListOperatorExpression(CypherParser.OC_ListOperatorExpressionContext context)
        {
            base.ExitOC_ListOperatorExpression(context);
        }

        public override void EnterOC_StringOperatorExpression(CypherParser.OC_StringOperatorExpressionContext context)
        {
            base.EnterOC_StringOperatorExpression(context);
        }

        public override void ExitOC_StringOperatorExpression(CypherParser.OC_StringOperatorExpressionContext context)
        {
            base.ExitOC_StringOperatorExpression(context);
        }

        public override void EnterOC_NullOperatorExpression(CypherParser.OC_NullOperatorExpressionContext context)
        {
            base.EnterOC_NullOperatorExpression(context);
        }

        public override void ExitOC_NullOperatorExpression(CypherParser.OC_NullOperatorExpressionContext context)
        {
            base.ExitOC_NullOperatorExpression(context);
        }

        public override void EnterOC_PropertyOrLabelsExpression(CypherParser.OC_PropertyOrLabelsExpressionContext context)
        {
            base.EnterOC_PropertyOrLabelsExpression(context);
        }

        public override void ExitOC_PropertyOrLabelsExpression(CypherParser.OC_PropertyOrLabelsExpressionContext context)
        {
            base.ExitOC_PropertyOrLabelsExpression(context);
        }

        public override void EnterOC_Atom(CypherParser.OC_AtomContext context)
        {
            base.EnterOC_Atom(context);
        }

        public override void ExitOC_Atom(CypherParser.OC_AtomContext context)
        {
            base.ExitOC_Atom(context);
        }

        public override void EnterOC_Literal(CypherParser.OC_LiteralContext context)
        {
            base.EnterOC_Literal(context);
        }

        public override void ExitOC_Literal(CypherParser.OC_LiteralContext context)
        {
            base.ExitOC_Literal(context);
        }

        public override void EnterOC_BooleanLiteral(CypherParser.OC_BooleanLiteralContext context)
        {
            base.EnterOC_BooleanLiteral(context);
        }

        public override void ExitOC_BooleanLiteral(CypherParser.OC_BooleanLiteralContext context)
        {
            base.ExitOC_BooleanLiteral(context);
        }

        public override void EnterOC_ListLiteral(CypherParser.OC_ListLiteralContext context)
        {
            base.EnterOC_ListLiteral(context);
        }

        public override void ExitOC_ListLiteral(CypherParser.OC_ListLiteralContext context)
        {
            base.ExitOC_ListLiteral(context);
        }

        public override void EnterOC_PartialComparisonExpression(CypherParser.OC_PartialComparisonExpressionContext context)
        {
            base.EnterOC_PartialComparisonExpression(context);
        }

        public override void ExitOC_PartialComparisonExpression(CypherParser.OC_PartialComparisonExpressionContext context)
        {
            base.ExitOC_PartialComparisonExpression(context);
        }

        public override void EnterOC_ParenthesizedExpression(CypherParser.OC_ParenthesizedExpressionContext context)
        {
            base.EnterOC_ParenthesizedExpression(context);
        }

        public override void ExitOC_ParenthesizedExpression(CypherParser.OC_ParenthesizedExpressionContext context)
        {
            base.ExitOC_ParenthesizedExpression(context);
        }

        public override void EnterOC_RelationshipsPattern(CypherParser.OC_RelationshipsPatternContext context)
        {
            base.EnterOC_RelationshipsPattern(context);
        }

        public override void ExitOC_RelationshipsPattern(CypherParser.OC_RelationshipsPatternContext context)
        {
            base.ExitOC_RelationshipsPattern(context);
        }

        public override void EnterOC_FilterExpression(CypherParser.OC_FilterExpressionContext context)
        {
            base.EnterOC_FilterExpression(context);
        }

        public override void ExitOC_FilterExpression(CypherParser.OC_FilterExpressionContext context)
        {
            base.ExitOC_FilterExpression(context);
        }

        public override void EnterOC_IdInColl(CypherParser.OC_IdInCollContext context)
        {
            base.EnterOC_IdInColl(context);
        }

        public override void ExitOC_IdInColl(CypherParser.OC_IdInCollContext context)
        {
            base.ExitOC_IdInColl(context);
        }

        public override void EnterOC_FunctionInvocation(CypherParser.OC_FunctionInvocationContext context)
        {
            base.EnterOC_FunctionInvocation(context);
        }

        public override void ExitOC_FunctionInvocation(CypherParser.OC_FunctionInvocationContext context)
        {
            base.ExitOC_FunctionInvocation(context);
        }

        public override void EnterOC_FunctionName(CypherParser.OC_FunctionNameContext context)
        {
            base.EnterOC_FunctionName(context);
        }

        public override void ExitOC_FunctionName(CypherParser.OC_FunctionNameContext context)
        {
            base.ExitOC_FunctionName(context);
        }

        public override void EnterOC_ExplicitProcedureInvocation(CypherParser.OC_ExplicitProcedureInvocationContext context)
        {
            base.EnterOC_ExplicitProcedureInvocation(context);
        }

        public override void ExitOC_ExplicitProcedureInvocation(CypherParser.OC_ExplicitProcedureInvocationContext context)
        {
            base.ExitOC_ExplicitProcedureInvocation(context);
        }

        public override void EnterOC_ImplicitProcedureInvocation(CypherParser.OC_ImplicitProcedureInvocationContext context)
        {
            base.EnterOC_ImplicitProcedureInvocation(context);
        }

        public override void ExitOC_ImplicitProcedureInvocation(CypherParser.OC_ImplicitProcedureInvocationContext context)
        {
            base.ExitOC_ImplicitProcedureInvocation(context);
        }

        public override void EnterOC_ProcedureResultField(CypherParser.OC_ProcedureResultFieldContext context)
        {
            base.EnterOC_ProcedureResultField(context);
        }

        public override void ExitOC_ProcedureResultField(CypherParser.OC_ProcedureResultFieldContext context)
        {
            base.ExitOC_ProcedureResultField(context);
        }

        public override void EnterOC_ProcedureName(CypherParser.OC_ProcedureNameContext context)
        {
            base.EnterOC_ProcedureName(context);
        }

        public override void ExitOC_ProcedureName(CypherParser.OC_ProcedureNameContext context)
        {
            base.ExitOC_ProcedureName(context);
        }

        public override void EnterOC_Namespace(CypherParser.OC_NamespaceContext context)
        {
            base.EnterOC_Namespace(context);
        }

        public override void ExitOC_Namespace(CypherParser.OC_NamespaceContext context)
        {
            base.ExitOC_Namespace(context);
        }

        public override void EnterOC_ListComprehension(CypherParser.OC_ListComprehensionContext context)
        {
            base.EnterOC_ListComprehension(context);
        }

        public override void ExitOC_ListComprehension(CypherParser.OC_ListComprehensionContext context)
        {
            base.ExitOC_ListComprehension(context);
        }

        public override void EnterOC_PatternComprehension(CypherParser.OC_PatternComprehensionContext context)
        {
            base.EnterOC_PatternComprehension(context);
        }

        public override void ExitOC_PatternComprehension(CypherParser.OC_PatternComprehensionContext context)
        {
            base.ExitOC_PatternComprehension(context);
        }

        public override void EnterOC_PropertyLookup(CypherParser.OC_PropertyLookupContext context)
        {
            base.EnterOC_PropertyLookup(context);
        }

        public override void ExitOC_PropertyLookup(CypherParser.OC_PropertyLookupContext context)
        {
            base.ExitOC_PropertyLookup(context);
        }

        public override void EnterOC_CaseExpression(CypherParser.OC_CaseExpressionContext context)
        {
            base.EnterOC_CaseExpression(context);
        }

        public override void ExitOC_CaseExpression(CypherParser.OC_CaseExpressionContext context)
        {
            base.ExitOC_CaseExpression(context);
        }

        public override void EnterOC_CaseAlternatives(CypherParser.OC_CaseAlternativesContext context)
        {
            base.EnterOC_CaseAlternatives(context);
        }

        public override void ExitOC_CaseAlternatives(CypherParser.OC_CaseAlternativesContext context)
        {
            base.ExitOC_CaseAlternatives(context);
        }

        public override void EnterOC_Variable(CypherParser.OC_VariableContext context)
        {
            base.EnterOC_Variable(context);
        }

        public override void ExitOC_Variable(CypherParser.OC_VariableContext context)
        {
            base.ExitOC_Variable(context);
        }

        public override void EnterOC_NumberLiteral(CypherParser.OC_NumberLiteralContext context)
        {
            base.EnterOC_NumberLiteral(context);
        }

        public override void ExitOC_NumberLiteral(CypherParser.OC_NumberLiteralContext context)
        {
            base.ExitOC_NumberLiteral(context);
        }

        public override void EnterOC_MapLiteral(CypherParser.OC_MapLiteralContext context)
        {
            base.EnterOC_MapLiteral(context);
        }

        public override void ExitOC_MapLiteral(CypherParser.OC_MapLiteralContext context)
        {
            base.ExitOC_MapLiteral(context);
        }

        public override void EnterOC_Parameter(CypherParser.OC_ParameterContext context)
        {
            base.EnterOC_Parameter(context);
        }

        public override void ExitOC_Parameter(CypherParser.OC_ParameterContext context)
        {
            base.ExitOC_Parameter(context);
        }

        public override void EnterOC_PropertyExpression(CypherParser.OC_PropertyExpressionContext context)
        {
            base.EnterOC_PropertyExpression(context);
        }

        public override void ExitOC_PropertyExpression(CypherParser.OC_PropertyExpressionContext context)
        {
            base.ExitOC_PropertyExpression(context);
        }

        public override void EnterOC_PropertyKeyName(CypherParser.OC_PropertyKeyNameContext context)
        {
            base.EnterOC_PropertyKeyName(context);
        }

        public override void ExitOC_PropertyKeyName(CypherParser.OC_PropertyKeyNameContext context)
        {
            base.ExitOC_PropertyKeyName(context);
        }

        public override void EnterOC_IntegerLiteral(CypherParser.OC_IntegerLiteralContext context)
        {
            base.EnterOC_IntegerLiteral(context);
        }

        public override void ExitOC_IntegerLiteral(CypherParser.OC_IntegerLiteralContext context)
        {
            base.ExitOC_IntegerLiteral(context);
        }

        public override void EnterOC_DoubleLiteral(CypherParser.OC_DoubleLiteralContext context)
        {
            base.EnterOC_DoubleLiteral(context);
        }

        public override void ExitOC_DoubleLiteral(CypherParser.OC_DoubleLiteralContext context)
        {
            base.ExitOC_DoubleLiteral(context);
        }

        public override void EnterOC_SchemaName(CypherParser.OC_SchemaNameContext context)
        {
            base.EnterOC_SchemaName(context);
        }

        public override void ExitOC_SchemaName(CypherParser.OC_SchemaNameContext context)
        {
            base.ExitOC_SchemaName(context);
        }

        public override void EnterOC_ReservedWord(CypherParser.OC_ReservedWordContext context)
        {
            base.EnterOC_ReservedWord(context);
        }

        public override void ExitOC_ReservedWord(CypherParser.OC_ReservedWordContext context)
        {
            base.ExitOC_ReservedWord(context);
        }

        public override void EnterOC_SymbolicName(CypherParser.OC_SymbolicNameContext context)
        {
            base.EnterOC_SymbolicName(context);
        }

        public override void ExitOC_SymbolicName(CypherParser.OC_SymbolicNameContext context)
        {
            base.ExitOC_SymbolicName(context);
        }

        public override void EnterOC_LeftArrowHead(CypherParser.OC_LeftArrowHeadContext context)
        {
            base.EnterOC_LeftArrowHead(context);
        }

        public override void ExitOC_LeftArrowHead(CypherParser.OC_LeftArrowHeadContext context)
        {
            base.ExitOC_LeftArrowHead(context);
        }

        public override void EnterOC_RightArrowHead(CypherParser.OC_RightArrowHeadContext context)
        {
            base.EnterOC_RightArrowHead(context);
        }

        public override void ExitOC_RightArrowHead(CypherParser.OC_RightArrowHeadContext context)
        {
            base.ExitOC_RightArrowHead(context);
        }

        public override void EnterOC_Dash(CypherParser.OC_DashContext context)
        {
            base.EnterOC_Dash(context);
        }

        public override void ExitOC_Dash(CypherParser.OC_DashContext context)
        {
            base.ExitOC_Dash(context);
        }

        public override void VisitErrorNode(IErrorNode node)
        {
            base.VisitErrorNode(node);
        }

        public override void EnterEveryRule(ParserRuleContext context)
        {
            base.EnterEveryRule(context);
        }

        public override void ExitEveryRule(ParserRuleContext context)
        {
            base.ExitEveryRule(context);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
        }
    }
}