using LikeParser;
using SimpleExpressionEvaluator.AbstractSyntaxTree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.Parser
{ 
    public abstract class EvaluatorBase
    {
        public Stack<AbstractSyntaxTreeNode> valueStack = new Stack<AbstractSyntaxTreeNode>();
        protected Dictionary<string, AbstractSyntaxTreeNode> setList = new Dictionary<string, AbstractSyntaxTreeNode>();
        protected Dictionary<Tuple<string, string>, List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>> thenList =
            new Dictionary<Tuple<string, string>, List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>>();
        protected Dictionary<Tuple<string, string>, List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>> elseList =
            new Dictionary<Tuple<string, string>, List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>>>();

        protected void MethodCallNodeExecuteCall<T>(T objectValue, AbstractSyntaxTreeNode item)
        {
            var method = objectValue.GetType().GetMethod(((MethodCallNode)item).Name);
            if (method != null)
            {
                var objectList = new List<object>();
                for (int parameterCount = 0; parameterCount < ((MethodCallNode)item).parameterList.Count; parameterCount++)
                {
                    var parameter = ((MethodCallNode)item).parameterList[parameterCount];
                    if (parameter is StringNode)
                    {
                        var value = ((StringNode)parameter).Value;
                        DateTime dateTime = new DateTime();
                        if (DateTime.TryParse(value, out dateTime))
                        {
                            objectList.Add(dateTime);
                        }
                        else
                        {
                            objectList.Add(value);
                        }
                    }
                    else if (parameter is DateTimeNode)
                    {
                        var value = ((DateTimeNode)parameter).Value;
                        objectList.Add(value);
                    }
                    else if (parameter is BooleanNode)
                    {
                        var value = ((BooleanNode)parameter).Value;
                        objectList.Add(value);
                    }
                    else if (parameter is DoubleNode)
                    {
                        var value = ((DoubleNode)parameter).Value;
                        objectList.Add(value);
                    }
                    else if (parameter is IntegerNode)
                    {
                        var value = ((IntegerNode)parameter).Value;
                        objectList.Add(value);
                    }
                }
                var result = method.Invoke(objectValue, objectList.ToArray());
                int intOutValue;
                double doubleOutValue;
                bool boolOutValue;
                DateTime dateTimeOutValue;
                if (result == null)
                {
                    NullNode nullNode = new NullNode();
                    valueStack.Push(nullNode);
                }
                else if (int.TryParse(result.ToString(), out intOutValue))
                {
                    IntegerNode integerNode = new IntegerNode();
                    integerNode.Value = intOutValue;
                    valueStack.Push(integerNode);
                }
                else if (double.TryParse(result.ToString(), out doubleOutValue))
                {
                    DoubleNode doubleNode = new DoubleNode();
                    doubleNode.Value = doubleOutValue;
                    valueStack.Push(doubleNode);
                }
                else if (bool.TryParse(result.ToString(), out boolOutValue))
                {
                    BooleanNode booleanNode = new BooleanNode();
                    booleanNode.Value = boolOutValue;
                    valueStack.Push(booleanNode);
                }
                else if (DateTime.TryParse(result.ToString(), out dateTimeOutValue))
                {
                    DateTimeNode dateTimeNode = new DateTimeNode();
                    dateTimeNode.Value = dateTimeOutValue;
                    valueStack.Push(dateTimeNode);
                }
                else
                {
                    StringNode stringNode = new StringNode();
                    stringNode.Value = result.ToString();
                    valueStack.Push(stringNode);
                }
            }
        }

        protected bool Like(string firstoperatorString, string secondperatorString)
        {
            StateMaschineBuilder stateMaschineBuilder = new StateMaschineBuilder();
            StateMaschine stateMaschine = stateMaschineBuilder.Match(secondperatorString);
            var result = stateMaschineBuilder.Recognize(firstoperatorString, stateMaschine);
            return result;
        }

        protected string FindParameterList(List<Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>> list,
            List<AbstractSyntaxTreeNode> postfixList,
            int actualPosition, Stack<AbstractSyntaxTreeNode> valueStack, bool firstCall)
        {
            var actualParameter = postfixList[actualPosition];
            if (actualParameter is ThenNode)
            {
                return FindParameterList(list, postfixList, actualPosition - 1, valueStack, true);
            }
            else if (actualParameter is ElseNode)
            {
                var pop = valueStack.Pop();
                return FindParameterList(list, postfixList, actualPosition - 1, valueStack, true);
            }
            else if (actualParameter is IntegerNode || actualParameter is StringNode || actualParameter is BooleanNode ||
                                actualParameter is DateTimeNode || actualParameter is DoubleNode)
            {
                AbstractSyntaxTreeNode pop = null;
                if (!firstCall)
                    pop = valueStack.Pop();
                list.Add(new Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>(actualParameter, null));
                return FindParameterList(list, postfixList, actualPosition - 1, valueStack, false);
            }
            else
            {
                if (actualParameter is VariableNode)
                    return ((VariableNode)actualParameter).Name;
                else if (actualParameter is MethodCallNode)
                {
                    foreach (var parameter in ((MethodCallNode)actualParameter).parameterList)
                    {
                        list.Add(new Tuple<AbstractSyntaxTreeNode, AbstractSyntaxTreeNode>(parameter, null));
                    }
                    return ((MethodCallNode)actualParameter).Name;
                }
                else
                {
                    return actualParameter.GetType().Name;
                }
            }
        }
    }
}
