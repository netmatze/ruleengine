using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleExpressionEvaluator.AbstractSyntaxTree;
using SimpleExpressionEvaluator.Lexer;
using SimpleExpressionEvaluator.AbstractSyntaxTree.AggregateFunctions;
using SimpleExpressionEvaluator.AbstractSyntaxTree.StringFunctions;

namespace SimpleExpressionEvaluator.Parser
{
    public class ExpressionEvaluatorParser : Parser
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
        //public Stack<AbstractSyntaxTreeNode> lastNodes = new Stack<AbstractSyntaxTreeNode>();
        //public Stack<AbstractSyntaxTreeNode> numberStack = new Stack<AbstractSyntaxTreeNode>();
        //public Stack<AbstractSyntaxTreeNode> operatorStack = new Stack<AbstractSyntaxTreeNode>();
        private Dictionary<string, AbstractSyntaxTreeNode> symbolTable = new Dictionary<string, AbstractSyntaxTreeNode>();

        public Dictionary<string, AbstractSyntaxTreeNode> SymbolTable
        {
            get { return symbolTable; }
            set { symbolTable = value; }
        }
        
        public ExpressionEvaluatorParser(SimpleExpressionEvaluator.Lexer.Lexer lexer)
            : base(lexer, 1)
        {

        }

        public List<AbstractSyntaxTreeNode> BuildParseTree()
        {
            if (parserTree == null)
            {
                parserTree = new ParseTree();
            }
            Stack<AbstractSyntaxTreeNode> operatorStack = new Stack<AbstractSyntaxTreeNode>();
            while (lookahead[lookaheadIndex].Type_ != TokenType.EOF)
            {
                parserTree.AddChild(BuildAST());
            }
            InfixToPostfix infixToPostfix = new InfixToPostfix();
            var infixToPostfixResult = infixToPostfix.ConvertInfixToPostfix(parserTree.RootNodes);
            return infixToPostfixResult;
        }

        public List<AbstractSyntaxTreeNode> BuildParseTree(DynamicBaseClass[] objectValues)
        {
            if (parserTree == null)
            {
                parserTree = new ParseTree();
            }
            Stack<AbstractSyntaxTreeNode> operatorStack = new Stack<AbstractSyntaxTreeNode>();
            while (lookahead[lookaheadIndex].Type_ != TokenType.EOF)
            {
                parserTree.AddChild(BuildAST(objectValues));
            }
            InfixToPostfix infixToPostfix = new InfixToPostfix();
            var infixToPostfixResult = infixToPostfix.ConvertInfixToPostfix(parserTree.RootNodes);
            return infixToPostfixResult;
        }

        public AbstractSyntaxTreeNode BuildAST()
        {                                    
            if (lookahead[lookaheadIndex].Type_ == TokenType.EQUAL)
            {
                return Equal();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.UNEQUAL)
            {
                return Unequal();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SMALLERTHEN)
            {
                return Smaller();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SMALLERTHENOREQUAL)
            {
                return Smallerequal();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.GREATERTHEN)
            {
                return Greater();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.GREATERTHENOREQUAL)
            {
                return Greaterequal();
            }            
            else if (lookahead[lookaheadIndex].Type_ == TokenType.OPENBRACKET)
            {
                return OpenBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.CLOSEBRACKET)
            {
                return CloseBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.OPENSQUAREBRACKET)
            {
                return OpenSquareBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.CLOSESQUAREBRACKET)
            {
                return CloseSquareBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.PLUS)
            {
                return Plus();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MINUS)
            {
                return Minus();
            }
            //else if (lookahead[lookaheadIndex].Type_ == TokenType.POINT)
            //{
            //    return Point();
            //}
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MUL)
            {
                return Mulibly();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.DIV)
            {
                return Divide();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MOD)
            {
                return Modulo();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.IDENT)
            {
                var methodCall = CheckMethodCall(lookahead[lookaheadIndex].Text);
                if (methodCall.Item1)
                {
                    return methodCall.Item2;
                }
                return Ident(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SET)
            {
                return Set(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.THEN)
            {
                return Then(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.ELSE)
            {
                return Else(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.IS)
            {
                return Is(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.IN)
            {
                return In(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.NOTIN)
            {
                return NotIn(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.AVERAGE)
            {
                return Average(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SUM)
            {
                return Sum(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.COUNT)
            {
                return Count(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MIN)
            {
                return Min(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MAX)
            {
                return Max(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.FIELD)
            {
                return Field(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.COLLECTION)
            {
                return Collection(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.FILTER)
            {
                return Collection(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LIKE)
            {
                return Like(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.NULL)
            {
                return Null(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.INTEGER)
            {
                return Integer(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.DOUBLE)
            {
                return Double(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.BOOL)
            {
                return Bool(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.STRING)
            {
                return String(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.OR)
            {
                return Or();
            }            
            else if (lookahead[lookaheadIndex].Type_ == TokenType.AND)
            {
                return And();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.COMMA)
            {
                return Comma();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LOWER)
            {
                return Lower();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.UPPER)
            {
                return Upper();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LENGTH)
            {
                return Lenght();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.TRIM)
            {
                return Trim();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LEFTTRIM)
            {
                return LeftTrim();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.RIGHTTRIM)
            {
                return RightTrim();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.POINT)
            {
                return Point();
            }
            return null;
        }

        public AbstractSyntaxTreeNode BuildSubAST(TokenType tokenType, string tokenText)
        {
            if (tokenType == TokenType.EQUAL)
            {
                return Equal();
            }
            else if (tokenType == TokenType.UNEQUAL)
            {
                return Unequal();
            }
            else if (tokenType == TokenType.SMALLERTHEN)
            {
                return Smaller();
            }
            else if (tokenType == TokenType.SMALLERTHENOREQUAL)
            {
                return Smallerequal();
            }
            else if (tokenType == TokenType.GREATERTHEN)
            {
                return Greater();
            }
            else if (tokenType == TokenType.GREATERTHENOREQUAL)
            {
                return Greaterequal();
            }
            else if (tokenType == TokenType.OPENBRACKET)
            {
                return OpenBracket();
            }
            else if (tokenType == TokenType.CLOSEBRACKET)
            {
                return CloseBracket();
            }
            else if (tokenType == TokenType.OPENSQUAREBRACKET)
            {
                return OpenSquareBracket();
            }
            else if (tokenType == TokenType.CLOSESQUAREBRACKET)
            {
                return CloseSquareBracket();
            }
            else if (tokenType == TokenType.PLUS)
            {
                return Plus();
            }
            else if (tokenType == TokenType.MINUS)
            {
                return Minus();
            }
            else if (tokenType == TokenType.POINT)
            {
                return Minus();
            }
            else if (tokenType == TokenType.MUL)
            {
                return Mulibly();
            }
            else if (tokenType == TokenType.DIV)
            {
                return Divide();
            }
            else if (tokenType == TokenType.MOD)
            {
                return Modulo();
            }
            else if (tokenType == TokenType.IDENT)
            {                
                return Ident(tokenText);
            }
            else if (tokenType == TokenType.SET)
            {
                return Set(tokenText);
            }
            else if (tokenType == TokenType.THEN)
            {
                return Then(tokenText);
            }
            else if (tokenType == TokenType.ELSE)
            {
                return Else(tokenText);
            }
            else if (tokenType == TokenType.IS)
            {
                return Is(tokenText);
            }
            else if (tokenType == TokenType.IN)
            {
                return In(tokenText);
            }
            else if (tokenType == TokenType.NOTIN)
            {
                return NotIn(tokenText);
            }
            else if (tokenType == TokenType.AVERAGE)
            {
                return Average(tokenText);
            }
            else if (tokenType == TokenType.SUM)
            {
                return Sum(tokenText);
            }
            else if (tokenType == TokenType.COUNT)
            {
                return Count(tokenText);
            }
            else if (tokenType == TokenType.MIN)
            {
                return Min(tokenText);
            }
            else if (tokenType == TokenType.MAX)
            {
                return Max(tokenText);
            }
            else if (tokenType == TokenType.FIELD)
            {
                return Field(tokenText);
            }
            else if (tokenType == TokenType.COLLECTION)
            {
                return Collection(tokenText);
            }
            else if (tokenType == TokenType.FILTER)
            {
                return Collection(tokenText);
            }
            else if (tokenType == TokenType.LIKE)
            {
                return Like(tokenText);
            }
            else if (tokenType == TokenType.NULL)
            {
                return Null(tokenText);
            }
            else if (tokenType == TokenType.INTEGER)
            {
                return Integer(tokenText);
            }
            else if (tokenType == TokenType.DOUBLE)
            {
                return Double(tokenText);
            }
            else if (tokenType == TokenType.BOOL)
            {
                return Bool(tokenText);
            }
            else if (tokenType == TokenType.STRING)
            {
                return String(tokenText);
            }
            else if (tokenType == TokenType.OR)
            {
                return Or();
            }
            else if (tokenType == TokenType.AND)
            {
                return And();
            }
            else if (tokenType == TokenType.COMMA)
            {
                return Comma();
            }
            else if (tokenType == TokenType.LOWER)
            {
                return Lower();
            }
            else if (tokenType == TokenType.UPPER)
            {
                return Upper();
            }
            else if (tokenType == TokenType.LENGTH)
            {
                return Lenght();
            }
            else if (tokenType == TokenType.TRIM)
            {
                return Trim();
            }
            else if (tokenType == TokenType.LEFTTRIM)
            {
                return LeftTrim();
            }
            else if (tokenType == TokenType.RIGHTTRIM)
            {
                return RightTrim();
            }
            else if (tokenType == TokenType.POINT)
            {
                return Point();
            }
            return null;
        }

        public AbstractSyntaxTreeNode BuildAST(DynamicBaseClass[] objectValues)
        {
            if (lookahead[lookaheadIndex].Type_ == TokenType.EQUAL)
            {
                return Equal();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.UNEQUAL)
            {
                return Unequal();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SMALLERTHEN)
            {
                return Smaller();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SMALLERTHENOREQUAL)
            {
                return Smallerequal();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.GREATERTHEN)
            {
                return Greater();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.GREATERTHENOREQUAL)
            {
                return Greaterequal();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.OPENBRACKET)
            {
                return OpenBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.CLOSEBRACKET)
            {
                return CloseBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.OPENSQUAREBRACKET)
            {
                return OpenSquareBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.CLOSESQUAREBRACKET)
            {
                return CloseSquareBracket();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.PLUS)
            {
                return Plus();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MINUS)
            {
                return Minus();
            }
            //else if (lookahead[lookaheadIndex].Type_ == TokenType.POINT)
            //{
            //    return Point();
            //}
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MUL)
            {
                return Mulibly();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.DIV)
            {
                return Divide();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MOD)
            {
                return Modulo();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.IDENT)
            {
                var methodCall = CheckMethodCall(lookahead[lookaheadIndex].Text);
                if (methodCall.Item1)
                {
                    return methodCall.Item2;
                }
                var foundObject = false;
                ObjectNode objectNode = null;
                foreach (var objectValue in objectValues)
                {
                    if (objectValue.ReferenceName == lookahead[lookaheadIndex].Text)
                    {
                        objectNode = new ObjectNode() { Name = lookahead[lookaheadIndex].Text, ObjectValue = objectValue };
                        foundObject = true;
                        break;
                    }
                }
                if(foundObject)
                {
                    return IdentObject(objectNode);
                }
                return Ident(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SET)
            {
                return Set(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.THEN)
            {
                return Then(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.ELSE)
            {
                return Else(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.IS)
            {
                return Is(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.IN)
            {
                return In(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.NOTIN)
            {
                return NotIn(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.AVERAGE)
            {
                return Average(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.SUM)
            {
                return Sum(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.COUNT)
            {
                return Count(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MIN)
            {
                return Min(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.MAX)
            {
                return Max(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.FIELD)
            {
                return Field(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.COLLECTION)
            {
                return Collection(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.FILTER)
            {
                return Collection(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LIKE)
            {
                return Like(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.NULL)
            {
                return Null(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.INTEGER)
            {
                return Integer(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.DOUBLE)
            {
                return Double(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.BOOL)
            {
                return Bool(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.STRING)
            {
                return String(lookahead[lookaheadIndex].Text);
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.OR)
            {
                return Or();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.AND)
            {
                return And();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.COMMA)
            {
                return Comma();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LOWER)
            {
                return Lower();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.UPPER)
            {
                return Upper();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LENGTH)
            {
                return Lenght();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.TRIM)
            {
                return Trim();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.LEFTTRIM)
            {
                return LeftTrim();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.RIGHTTRIM)
            {
                return RightTrim();
            }
            else if (lookahead[lookaheadIndex].Type_ == TokenType.POINT)
            {
                return Point();
            }
            return null;
        }

        public AbstractSyntaxTreeNode BuildSubAST(TokenType tokenType, string tokenText, DynamicBaseClass[] values)
        {
            if (tokenType == TokenType.EQUAL)
            {
                return Equal();
            }
            else if (tokenType == TokenType.UNEQUAL)
            {
                return Unequal();
            }
            else if (tokenType == TokenType.SMALLERTHEN)
            {
                return Smaller();
            }
            else if (tokenType == TokenType.SMALLERTHENOREQUAL)
            {
                return Smallerequal();
            }
            else if (tokenType == TokenType.GREATERTHEN)
            {
                return Greater();
            }
            else if (tokenType == TokenType.GREATERTHENOREQUAL)
            {
                return Greaterequal();
            }
            else if (tokenType == TokenType.OPENBRACKET)
            {
                return OpenBracket();
            }
            else if (tokenType == TokenType.CLOSEBRACKET)
            {
                return CloseBracket();
            }
            else if (tokenType == TokenType.OPENSQUAREBRACKET)
            {
                return OpenSquareBracket();
            }
            else if (tokenType == TokenType.CLOSESQUAREBRACKET)
            {
                return CloseSquareBracket();
            }
            else if (tokenType == TokenType.PLUS)
            {
                return Plus();
            }
            else if (tokenType == TokenType.MINUS)
            {
                return Minus();
            }
            else if (tokenType == TokenType.POINT)
            {
                return Minus();
            }
            else if (tokenType == TokenType.MUL)
            {
                return Mulibly();
            }
            else if (tokenType == TokenType.DIV)
            {
                return Divide();
            }
            else if (tokenType == TokenType.MOD)
            {
                return Modulo();
            }
            else if (tokenType == TokenType.IDENT)
            {
                return Ident(tokenText);
            }
            else if (tokenType == TokenType.SET)
            {
                return Set(tokenText);
            }
            else if (tokenType == TokenType.THEN)
            {
                return Then(tokenText);
            }
            else if (tokenType == TokenType.ELSE)
            {
                return Else(tokenText);
            }
            else if (tokenType == TokenType.IS)
            {
                return Is(tokenText);
            }
            else if (tokenType == TokenType.IN)
            {
                return In(tokenText);
            }
            else if (tokenType == TokenType.NOTIN)
            {
                return NotIn(tokenText);
            }
            else if (tokenType == TokenType.AVERAGE)
            {
                return Average(tokenText);
            }
            else if (tokenType == TokenType.SUM)
            {
                return Sum(tokenText);
            }
            else if (tokenType == TokenType.COUNT)
            {
                return Count(tokenText);
            }
            else if (tokenType == TokenType.MIN)
            {
                return Min(tokenText);
            }
            else if (tokenType == TokenType.MAX)
            {
                return Max(tokenText);
            }
            else if (tokenType == TokenType.FIELD)
            {
                return Field(tokenText);
            }
            else if (tokenType == TokenType.COLLECTION)
            {
                return Collection(tokenText);
            }
            else if (tokenType == TokenType.FILTER)
            {
                return Collection(tokenText);
            }
            else if (tokenType == TokenType.LIKE)
            {
                return Like(tokenText);
            }
            else if (tokenType == TokenType.NULL)
            {
                return Null(tokenText);
            }
            else if (tokenType == TokenType.INTEGER)
            {
                return Integer(tokenText);
            }
            else if (tokenType == TokenType.DOUBLE)
            {
                return Double(tokenText);
            }
            else if (tokenType == TokenType.BOOL)
            {
                return Bool(tokenText);
            }
            else if (tokenType == TokenType.STRING)
            {
                return String(tokenText);
            }
            else if (tokenType == TokenType.OR)
            {
                return Or();
            }
            else if (tokenType == TokenType.AND)
            {
                return And();
            }
            else if (tokenType == TokenType.COMMA)
            {
                return Comma();
            }
            else if (tokenType == TokenType.LOWER)
            {
                return Lower();
            }
            else if (tokenType == TokenType.UPPER)
            {
                return Upper();
            }
            else if (tokenType == TokenType.LENGTH)
            {
                return Lenght();
            }
            else if (tokenType == TokenType.TRIM)
            {
                return Trim();
            }
            else if (tokenType == TokenType.LEFTTRIM)
            {
                return LeftTrim();
            }
            else if (tokenType == TokenType.RIGHTTRIM)
            {
                return RightTrim();
            }
            else if (tokenType == TokenType.POINT)
            {
                return Point();
            }
            return null;
        }

        public Tuple<bool, MethodCallNode> CheckMethodCall(string value)
        {
            MethodCallNode methodCallNode = new MethodCallNode();
            methodCallNode.Name = value;
            var currentToken = NextToken();
            if (currentToken.Type_ == TokenType.OPENBRACKET)
            {
                Match(TokenType.IDENT);
                return new Tuple<bool, MethodCallNode>(true, methodCallNode);
            }
            else if(currentToken.Type_ == TokenType.LIKE)
            {
                Rewind();
                return new Tuple<bool, MethodCallNode>(false, null);
            }
            else
            {                
                return new Tuple<bool, MethodCallNode>(false, null);
            }         
        }

        public VariableNode Ident(string value)
        {
            VariableNode identNode = new VariableNode();
            identNode.Name = value;
            Match(TokenType.IDENT);
            var currentToken = CurrentToken();            
            if (!symbolTable.ContainsKey(value))
                symbolTable.Add(value, identNode);
            return identNode;
        }

        public ObjectNode IdentObject(ObjectNode objectNode)
        {
            Match(TokenType.IDENT);
            var currentToken = CurrentToken();
            if (!symbolTable.ContainsKey(objectNode.Name))
                symbolTable.Add(objectNode.Name, objectNode);            
            currentToken = CurrentToken();
            if (currentToken.Type_ == TokenType.IDENT)
            {
                objectNode.PropertyName = currentToken.Text;
                Match(TokenType.IDENT);
            }
            else if (currentToken.Type_ == TokenType.FILTER)
            {
                FilterNode filterNode = Filter(currentToken.Text);
                objectNode.Filter = filterNode;
            }
            ConsumeNextToken();
            currentToken = CurrentToken();
            if (currentToken.Type_ == TokenType.POINT)
            {
                Match(TokenType.POINT);
                currentToken = CurrentToken();
            }
            if (currentToken.Type_ == TokenType.FILTER)
            {
                FilterNode filterNode = Filter(currentToken.Text);
                objectNode.Filter = filterNode;
                currentToken = CurrentToken();
            }
            if (currentToken.Type_ == TokenType.POINT)
            {
                Match(TokenType.POINT);
                currentToken = CurrentToken();
            }
            if (currentToken.Type_ == TokenType.AVERAGE)
            {
                objectNode.AggregateFunction = new AggregateAverageNode();
                Match(TokenType.AVERAGE);
            }
            else if (currentToken.Type_ == TokenType.SUM)
            {
                objectNode.AggregateFunction = new AggregateSumNode();
                Match(TokenType.SUM);
            }
            else if (currentToken.Type_ == TokenType.COUNT)
            {
                objectNode.AggregateFunction = new AggregateCountNode();
                Match(TokenType.COUNT);
            }
            else if (currentToken.Type_ == TokenType.MIN)
            {
                objectNode.AggregateFunction = new AggregateMinNode();
                Match(TokenType.MIN);
            }
            else if (currentToken.Type_ == TokenType.MAX)
            {
                objectNode.AggregateFunction = new AggregateMaxNode();
                Match(TokenType.MAX);
            }            
            return objectNode;
        }

        public VariableNode FieldIdent(string value)
        {
            VariableNode identNode = new VariableNode();
            identNode.Name = value;
            var fieldValue = "Field." + value;
            if (!symbolTable.ContainsKey(fieldValue))
                symbolTable.Add(fieldValue, identNode);
            return identNode;
        }
        public VariableNode CollectionIdent(string value)
        {
            VariableNode identNode = new VariableNode();
            identNode.Name = value;
            var collectionValue = "Collection." + value;
            if (!symbolTable.ContainsKey(collectionValue))
                symbolTable.Add(collectionValue, identNode);
            return identNode;
        }

        public FilterNode Filter(string value)
        {
            FilterNode filterNode = new FilterNode();
            filterNode.Text = value;
            Match(TokenType.FILTER);
            Token nextToken = CurrentToken();
            do
            {
                if (nextToken.Type_ != TokenType.OPENSQUAREBRACKET)
                {
                    var abstractSyntaxTreeNode = BuildSubAST(nextToken.Type_, nextToken.Text);
                    filterNode.FilterItems.Add(abstractSyntaxTreeNode);
                }
                else
                {
                    Match(TokenType.OPENSQUAREBRACKET);
                }
                nextToken = CurrentToken();
            }
            while (nextToken.Type_ != TokenType.CLOSESQUAREBRACKET);
            Match(TokenType.CLOSESQUAREBRACKET);
            return filterNode;
        }

        public FilterNode Filter(string value, DynamicBaseClass[] objectValues)
        {
            FilterNode filterNode = new FilterNode();
            filterNode.Text = value;
            Match(TokenType.FILTER);
            Token nextToken = CurrentToken();
            do
            {
                if (nextToken.Type_ != TokenType.OPENSQUAREBRACKET)
                {
                    var abstractSyntaxTreeNode = BuildSubAST(nextToken.Type_, nextToken.Text, objectValues);
                    filterNode.FilterItems.Add(abstractSyntaxTreeNode);
                }
                else
                {
                    Match(TokenType.OPENSQUAREBRACKET);
                }
                nextToken = CurrentToken();
            }
            while (nextToken.Type_ != TokenType.CLOSESQUAREBRACKET);
            Match(TokenType.CLOSESQUAREBRACKET);
            return filterNode;
        }

        public SetNode Set(string value)
        {
            SetNode setNode = new SetNode();
            Match(TokenType.SET);
            return setNode;
        }

        public ThenNode Then(string value)
        {
            ThenNode thenNode = new ThenNode();
            Match(TokenType.THEN);
            return thenNode;
        }

        public ElseNode Else(string value)
        {
            ElseNode elseNode = new ElseNode();
            Match(TokenType.ELSE);
            return elseNode;
        }

        public IsNode Is(string value)
        {
            IsNode isNode = new IsNode();
            Match(TokenType.IS);
            return isNode;
        }

        public AggregateInNode In(string value)
        {
            AggregateInNode inNode = new AggregateInNode();
            Match(TokenType.IN);
            return inNode;
        }

        public AggregateNotInNode NotIn(string value)
        {
            AggregateNotInNode notInNode = new AggregateNotInNode();
            Match(TokenType.NOTIN);
            return notInNode;
        }

        public AggregateAverageNode Average(string value)
        {
            AggregateAverageNode averageNode = new AggregateAverageNode();
            Match(TokenType.AVERAGE);
            return averageNode;
        }

        public AggregateCountNode Count(string value)
        {
            AggregateCountNode countNode = new AggregateCountNode();
            Match(TokenType.COUNT);
            return countNode;
        }

        public AggregateSumNode Sum(string value)
        {
            AggregateSumNode sumNode = new AggregateSumNode();
            Match(TokenType.SUM);
            return sumNode;
        }

        public AggregateMinNode Min(string value)
        {
            AggregateMinNode minNode = new AggregateMinNode();
            Match(TokenType.MIN);
            return minNode;
        }

        public AggregateMaxNode Max(string value)
        {
            AggregateMaxNode maxNode = new AggregateMaxNode();
            Match(TokenType.MAX);
            return maxNode;
        }
        
        public FieldNode Field(string value)
        {
            FieldNode fieldNode = new FieldNode();            
            Match(TokenType.FIELD);
            var currentToken = NextToken();            
            if(currentToken.Type_ == TokenType.IDENT)
            {
                VariableNode variableNode = FieldIdent(currentToken.Text);
                fieldNode.Value = variableNode;
                Match(TokenType.OPENSQUAREBRACKET);               
            }           
            return fieldNode;
        }        

        public CollectionNode Collection(string value)
        {
            CollectionNode collectionNode = new CollectionNode();
            Match(TokenType.COLLECTION);
            ConsumeNextToken();
            var currentToken = CurrentToken();
            if (currentToken.Type_ == TokenType.IDENT)
            {
                VariableNode variableNode = CollectionIdent(currentToken.Text);
                collectionNode.Value = variableNode;
                Match(TokenType.IDENT);
            }
            else if(currentToken.Type_ == TokenType.FILTER)
            {
                FilterNode filterNode = Filter(currentToken.Text);
                collectionNode.Filter = filterNode;
            }
            ConsumeNextToken();
            currentToken = CurrentToken();
            if(currentToken.Type_ == TokenType.POINT)
            {
                Match(TokenType.POINT);
                currentToken = CurrentToken();
            }
            if (currentToken.Type_ == TokenType.FILTER)
            {
                FilterNode filterNode = Filter(currentToken.Text);
                collectionNode.Filter = filterNode;                
                currentToken = CurrentToken();
            }
            if (currentToken.Type_ == TokenType.POINT)
            {
                Match(TokenType.POINT);
                currentToken = CurrentToken();
            }
            if (currentToken.Type_ == TokenType.AVERAGE)
            {
                collectionNode.AggregateFunction = new AggregateAverageNode();
                Match(TokenType.AVERAGE);
            }
            else if(currentToken.Type_ == TokenType.SUM)
            {
                collectionNode.AggregateFunction = new AggregateSumNode();
                Match(TokenType.SUM);
            }
            else if (currentToken.Type_ == TokenType.COUNT)
            {
                collectionNode.AggregateFunction = new AggregateCountNode();
                Match(TokenType.COUNT);
            }
            else if (currentToken.Type_ == TokenType.MIN)
            {
                collectionNode.AggregateFunction = new AggregateMinNode();
                Match(TokenType.MIN);
            }
            else if (currentToken.Type_ == TokenType.MAX)
            {
                collectionNode.AggregateFunction = new AggregateMaxNode();
                Match(TokenType.MAX);
            }            
            else
            {
                Rewind();
            }               
            return collectionNode;
        }        

        public LikeNode Like(string value)
        {
            LikeNode likeNode = new LikeNode();
            Match(TokenType.LIKE);
            return likeNode;
        }

        public NullNode Null(string value)
        {
            NullNode nullNode = new NullNode();
            Match(TokenType.NULL);
            return nullNode;
        }

        public IntegerNode Integer(string value)
        {
            IntegerNode integerNode = new IntegerNode();
            Match(TokenType.INTEGER);            
            integerNode.Value = int.Parse(value);
            return integerNode;
        }

        public BooleanNode Bool(string value)
        {
            BooleanNode booleanNode = new BooleanNode();
            Match(TokenType.BOOL);
            booleanNode.Value = bool.Parse(value);
            return booleanNode;
        }

        public StringNode String(string value)
        {
            StringNode stringNode = new StringNode();
            Match(TokenType.STRING);
            stringNode.Value = value;
            return stringNode;
        }

        public DoubleNode Double(string value)
        {
            DoubleNode doubleNode = new DoubleNode();
            Match(TokenType.DOUBLE);            
            doubleNode.Value = double.Parse(value);
            return doubleNode;
        }

        public AddNode Plus()
        {
            AddNode addNode = new AddNode();
            Match(TokenType.PLUS);                      
            return addNode;
        }
        
        public SubNode Minus()
        {
            SubNode subNode = new SubNode();
            Match(TokenType.MINUS);            
            return subNode;
        }

        public PointNode Point()
        {
            PointNode pointNode = new PointNode();
            Match(TokenType.POINT);
            return pointNode;
        }

        //public PointNode Minus()
        //{
        //    SubNode subNode = new SubNode();
        //    Match(TokenType.MINUS);
        //    return subNode;
        //}

        public MulNode Mulibly()
        {
            MulNode mulNode = new MulNode();
            Match(TokenType.MUL);            
            return mulNode;
        }

        public DivNode Divide()
        {
            DivNode divNode = new DivNode();
            Match(TokenType.DIV);           
            return divNode;
        }

        public ModuloNode Modulo()
        {
            ModuloNode modNode = new ModuloNode();
            Match(TokenType.MOD);
            return modNode;
        }

        public OpenBracketNode OpenBracket()
        {
            OpenBracketNode bracketNode = new OpenBracketNode();
            Match(TokenType.OPENBRACKET);
            return bracketNode;
        }

        public CloseBracketNode CloseBracket()
        {
            CloseBracketNode closeBracketNode = new CloseBracketNode();            
            Match(TokenType.CLOSEBRACKET);
            return closeBracketNode;
        }        

        public OpenSquareBracketNode OpenSquareBracket()
        {
            OpenSquareBracketNode openSquareBracket = new OpenSquareBracketNode();
            Match(TokenType.OPENSQUAREBRACKET);
            return openSquareBracket;
        }

        public CloseSquareBracketNode CloseSquareBracket()
        {
            CloseSquareBracketNode closeSquareBracket = new CloseSquareBracketNode();
            Match(TokenType.CLOSESQUAREBRACKET);
            return closeSquareBracket;
        }

        public EqualNode Equal()
        {
            EqualNode equalNode = new EqualNode();
            Match(TokenType.EQUAL);
            return equalNode;
        }

        public UnEqualNode Unequal()
        {
            UnEqualNode unequalNode = new UnEqualNode();
            Match(TokenType.UNEQUAL);
            return unequalNode;
        }

        public SmallerThenNode Smaller()
        {
            SmallerThenNode smallerThenNode = new SmallerThenNode();
            Match(TokenType.SMALLERTHEN);
            return smallerThenNode;
        }

        public CommaNode Comma()
        {
            var commaNode = new CommaNode();
            Match(TokenType.COMMA);
            return commaNode;
        }

        public LowerNode Lower()
        {
            var lowerNode = new LowerNode();
            Match(TokenType.LOWER);
            return lowerNode;
        }

        public UpperNode Upper()
        {
            var upperNode = new UpperNode();
            Match(TokenType.UPPER);
            return upperNode;
        }

        public LengthNode Lenght()
        {
            var lengthNode = new LengthNode();
            Match(TokenType.LENGTH);
            return lengthNode;
        }

        public TrimNode Trim()
        {
            var trimNode = new TrimNode();
            Match(TokenType.TRIM);
            return trimNode;
        }
       
        public LeftTrimNode LeftTrim()
        {
            var leftTrimNode = new LeftTrimNode();
            Match(TokenType.LEFTTRIM);
            return leftTrimNode;
        }

        public RightTrimNode RightTrim()
        {
            var rightTrimNode = new RightTrimNode();
            Match(TokenType.RIGHTTRIM);
            return rightTrimNode;
        }

        public SmallerThenOrEqualNode Smallerequal()
        {
            SmallerThenOrEqualNode smallerThenOrEqualNode = new SmallerThenOrEqualNode();
            Match(TokenType.SMALLERTHENOREQUAL);
            return smallerThenOrEqualNode;
        }

        public GreaterThenNode Greater()
        {
            GreaterThenNode greaterThenNode = new GreaterThenNode();
            Match(TokenType.GREATERTHEN);
            return greaterThenNode;
        }

        public GreaterThenOrEqualNode Greaterequal()
        {
            GreaterThenOrEqualNode greaterThenOrEqualNode = new GreaterThenOrEqualNode();
            Match(TokenType.GREATERTHENOREQUAL);
            return greaterThenOrEqualNode;
        }

        public OrNode Or()
        {
            OrNode orNode = new OrNode();
            Match(TokenType.OR);            
            return orNode;
        }

        public AndNode And()
        {
            AndNode andNode = new AndNode();
            Match(TokenType.AND);
            return andNode;
        }

        //public AndNode And()
        //{
        //    AndNode andNode = new AndNode();
        //    Match(TokenType.COMMA);
        //    return andNode;
        //}
    }
}
