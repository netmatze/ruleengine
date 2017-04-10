using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.AbstractSyntaxTree;
using SimpleExpressionEvaluator.AbstractSyntaxTree.AggregateFunctions;
using SimpleExpressionEvaluator.AbstractSyntaxTree.StringFunctions;

namespace SimpleExpressionEvaluator.Parser
{
    public class InfixToPostfix
    {
        //implementation with List and Operator Stack convert infix espression (5 + 3 * 2 - 1) to postfix expression and then calculate value ( 10 )
        //first convert infix to postfix expression and save in list (5 + 3 * 2 - 1 => 5 3 2 * 1 - +)
        //5 => to List ( 5 )
        //+ => to Operator Stack ( + )
        //3 => to List ( 5, 3 )
        //* => to Operator Stack ( +, * )
        //2 => to List ( 5, 3, 2 )
        //- => lower precedence than * on Stack => to List ( 5, 3, 2, * ) => Operator Stack ( +, - )
        //1 => to List ( 5, 3, 2, *, 1 )
        //pop Operator Stack ( +, - ) => to List ( 5, 3, 2, *, 1, -, + )
        //second calculate the result out of the genenerated list with the postfix expression with a Stack
        //5 => Stack ( 5 )
        //3 => Stack ( 5, 3 )
        //2 => Stack ( 5, 3, 2 )
        //* => 2 from Stack 3 from Stack change operators ( 5 ) => calculate 3 * 2 = 6 => Stack ( 5, 6 )
        //1 => Stack ( 5, 6, 1 )
        //- => 1 from Stack 6 from Stack change operators ( 5 ) => calculate 6 - 1 = 5 => Stack ( 5, 5 )
        //+ => 5 from Stack 5 from Stack change operators ( ) => calculate 5 + 5 => Stack ( 10 )
        public List<AbstractSyntaxTreeNode> ConvertInfixToPostfix(List<AbstractSyntaxTreeNode> postfixList)
        {
            List<AbstractSyntaxTreeNode> returnList = new List<AbstractSyntaxTreeNode>();
            Stack<AbstractSyntaxTreeNode> operatorStack = new Stack<AbstractSyntaxTreeNode>();
            var position = 0;
            var methodParameterMode = false;
            MethodCallNode methodCallNode = null;
            for (int i = position; i < postfixList.Count; i++)            
            {
                var item = postfixList[i];
                if (item is IntegerNode || item is DoubleNode || 
                    item is VariableNode || 
                    item is FieldNode ||
                    item is BooleanNode || item is StringNode || item is NullNode)
                {
                    if (methodParameterMode)
                    {
                        
                    }
                    else
                    {
                        returnList.Add(item);
                    }
                }
                else if(item is ObjectNode)
                {
                    var filter = ((ObjectNode)item).Filter;
                    if (filter != null)
                    {
                        ((ObjectNode)item).Filter.FilterItems = ConvertInfixToPostfix(filter.FilterItems);
                    }
                    returnList.Add(item);
                }
                else if(item is UpperNode || item is LowerNode || item is LengthNode 
                    || item is TrimNode || item is LeftTrimNode || item is RightTrimNode)
                {
                    returnList.Add(item);
                }
                else if(item is CollectionNode)
                {
                    if (methodParameterMode)
                    {

                    }
                    else
                    {
                        var filter = ((CollectionNode)item).Filter;
                        if(filter != null)
                        {
                            ((CollectionNode)item).Filter.FilterItems = ConvertInfixToPostfix(filter.FilterItems); 
                        }
                        returnList.Add(item);
                    }
                }
                else if (item is OpenBracketNode)
                {
                    if (methodParameterMode)
                    {

                    }
                    else
                    {
                        i = ConvertInfixToPostfixBracket(i, postfixList, returnList);
                    }
                }
                else if (item is CloseBracketNode)
                {
                    methodParameterMode = false;
                }
                else if (item is OrNode || item is AndNode)
                {
                    while (operatorStack.Count > 0)
                    {
                        var abstractSyntaxTreeNode = operatorStack.Pop();
                        returnList.Add(abstractSyntaxTreeNode);
                    }
                    operatorStack.Push(item);
                }
                else if (item is GreaterThenNode || item is GreaterThenOrEqualNode ||
                    item is SmallerThenNode || item is SmallerThenOrEqualNode ||
                    item is EqualNode || item is UnEqualNode || item is IsNode || item is LikeNode || 
                    item is AggregateInNode || item is AggregateNotInNode)
                {
                    if (operatorStack.Count() > 0 && (operatorStack.Peek() is MulNode || 
                        operatorStack.Peek() is DivNode ||
                        operatorStack.Peek() is ModuloNode))
                    {
                        AbstractSyntaxTreeNode node = operatorStack.Pop();
                        returnList.Add(node);
                    }
                    else if (operatorStack.Count() > 0 && (operatorStack.Peek() is AddNode || 
                        operatorStack.Peek() is SubNode))
                    {
                        AbstractSyntaxTreeNode node = operatorStack.Pop();
                        returnList.Add(node);
                    }
                    else if (operatorStack.Count() > 0 && (operatorStack.Peek() is AggregateInNode ||
                        operatorStack.Peek() is AggregateNotInNode))
                    {
                        AbstractSyntaxTreeNode node = operatorStack.Pop();
                        returnList.Add(node);
                    }
                    operatorStack.Push(item);
                }
                else if (item is AddNode || item is SubNode)
                {
                    if (operatorStack.Count() > 0 && (operatorStack.Peek() is MulNode || 
                        operatorStack.Peek() is DivNode ||
                        operatorStack.Peek() is ModuloNode))
                    {
                        AbstractSyntaxTreeNode node = operatorStack.Pop();
                        returnList.Add(node);
                    }
                    operatorStack.Push(item);
                }
                else if (item is MulNode || item is DivNode || item is ModuloNode)
                {
                    operatorStack.Push(item);
                }
                else if (item is SetNode || item is ThenNode || item is ElseNode)
                {
                    if (postfixList[i + 1] is MethodCallNode )
                    {
                        methodCallNode = (MethodCallNode)postfixList[i + 1];
                        i = ConvertMethodInfixToPostfixBracket(i + 1, postfixList, methodCallNode);
                        returnList.Add(item);
                        returnList.Add(methodCallNode);
                    }
                    else if (postfixList[i + 1] is OpenBracketNode)
                    {
                        returnList.Add(item);
                        i = ConvertInfixToPostfixBracket(i + 1, postfixList, returnList);
                        //returnList.Add(item);
                        //returnList.Add(methodCallNode);
                    }
                    else
                    {
                        operatorStack.Push(item);
                    }
                }
                else if (item is MethodCallNode)
                {
                    methodCallNode = (MethodCallNode)item;
                    i = ConvertMethodInfixToPostfixBracket(i, postfixList, methodCallNode);
                    returnList.Add(methodCallNode);
                }
                position++;
            }
            while (operatorStack.Count > 0)
            {
                var abstractSyntaxTreeNode = operatorStack.Pop();
                returnList.Add(abstractSyntaxTreeNode);
            }
            return returnList;
        }

        public int ConvertMethodInfixToPostfixBracket(int position,
           List<AbstractSyntaxTreeNode> postfixList, AbstractSyntaxTreeNode methodCallNode)
        {
            Stack<AbstractSyntaxTreeNode> operatorStack = new Stack<AbstractSyntaxTreeNode>();
            int i = 0;
            List<AbstractSyntaxTreeNode> parameterList = new List<AbstractSyntaxTreeNode>();
            for (i = position + 1; i < postfixList.Count; i++)
            {
                var item = postfixList[i];
                if (item is CloseBracketNode)
                {
                    break;
                }
                else if (item is IntegerNode || item is DoubleNode || 
                    item is VariableNode || item is FieldNode || item is CollectionNode || item is ObjectNode
                    || item is BooleanNode || item is StringNode)
                {
                    parameterList.Add(item);
                }                                       
                position++;
            }
            ((MethodCallNode)methodCallNode).parameterList.InsertRange(0, parameterList.ToArray());
            return i;
        }

        public int ConvertInfixToPostfixBracket(int position, 
            List<AbstractSyntaxTreeNode> postfixList, List<AbstractSyntaxTreeNode> returnList)
        {
            Stack<AbstractSyntaxTreeNode> operatorStack = new Stack<AbstractSyntaxTreeNode>();
            int i = 0;
            var methodParameterMode = false;
            MethodCallNode methodCallNode = null;
            for (i = position + 1; i < postfixList.Count; i++)
            {
                var item = postfixList[i];
                if (item is CloseBracketNode)
                {
                    break;
                }
                if (item is IntegerNode || item is DoubleNode || item is VariableNode 
                    || item is FieldNode || item is CollectionNode || item is ObjectNode
                    || item is BooleanNode || item is StringNode)
                {
                    if (methodParameterMode)
                    {

                    }
                    else
                    {
                        returnList.Add(item);
                    }
                }
                else if (item is UpperNode || item is LowerNode || item is LengthNode
                    || item is TrimNode || item is LeftTrimNode || item is RightTrimNode)
                {
                    returnList.Add(item);
                }
                else if (item is OpenBracketNode)
                {
                    if (methodParameterMode)
                    {

                    }
                    else
                    {                        
                        i = ConvertInfixToPostfixBracket(i, postfixList, returnList);
                    }
                }                
                else if (item is OrNode || item is AndNode)
                {
                    while (operatorStack.Count > 0)
                    {
                        var abstractSyntaxTreeNode = operatorStack.Pop();
                        returnList.Add(abstractSyntaxTreeNode);
                    }
                    operatorStack.Push(item);
                }
                else if (item is GreaterThenNode || item is GreaterThenOrEqualNode ||
                    item is SmallerThenNode || item is SmallerThenOrEqualNode ||
                    item is EqualNode || item is UnEqualNode)
                {
                    if (operatorStack.Count() > 0 && (operatorStack.Peek() is MulNode || operatorStack.Peek() is DivNode ||
                        operatorStack.Peek() is ModuloNode))
                    {
                        AbstractSyntaxTreeNode node = operatorStack.Pop();
                        returnList.Add(node);
                    }
                    if (operatorStack.Count() > 0 && (operatorStack.Peek() is AddNode || operatorStack.Peek() is SubNode))
                    {
                        AbstractSyntaxTreeNode node = operatorStack.Pop();
                        returnList.Add(node);
                    }
                    operatorStack.Push(item);
                }
                else if (item is AddNode || item is SubNode)
                {
                    if (operatorStack.Count() > 0 && (operatorStack.Peek() is MulNode || operatorStack.Peek() is DivNode ||
                        operatorStack.Peek() is ModuloNode))
                    {
                        AbstractSyntaxTreeNode node = operatorStack.Pop();
                        returnList.Add(node);
                    }
                    operatorStack.Push(item);
                }
                else if (item is MulNode || item is DivNode || item is ModuloNode)
                {
                    operatorStack.Push(item);
                }
                else if (item is MethodCallNode)
                {
                    methodParameterMode = true;
                    methodCallNode = (MethodCallNode)item;
                    i = ConvertMethodInfixToPostfixBracket(i, postfixList, methodCallNode);
                    returnList.Add(methodCallNode);
                    methodParameterMode = false;
                }
                position++;
            }
            while (operatorStack.Count > 0)
            {
                var abstractSyntaxTreeNode = operatorStack.Pop();
                returnList.Add(abstractSyntaxTreeNode);
            }
            return i;
        }
    }
}
