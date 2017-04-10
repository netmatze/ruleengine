using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleExpressionEvaluator.AbstractSyntaxTree;
using SimpleExpressionEvaluator.Lexer;

namespace SimpleExpressionEvaluator.Parser
{
    // LL(k) Parser
    public abstract class Parser
    {
        protected Lexer.Lexer input;
        protected Token[] lookahead;
        protected int lookaheadCount;
        protected int lookaheadIndex = 0;
        protected ParseTree parserTree = new ParseTree();
        protected AbstractSyntaxTreeNode currentTreeNode;

        public Parser(Lexer.Lexer input, int lookaheadCount)
        {
            this.input = input;
            this.lookaheadCount = lookaheadCount;
            lookahead = new Token[lookaheadCount];
            for (int i = 1; i <= lookaheadCount; i++)
            {
                ConsumeNextToken();
            }
        }

        public void ConsumeNextToken()
        {
            lookahead[lookaheadIndex] = input.NextToken();
            lookaheadIndex = (lookaheadIndex + 1) % lookaheadCount;
        }

        public void Match(TokenType token)
        {
            if (lookahead[lookaheadIndex].Type_ == token)
                ConsumeNextToken();
            else
                throw new Exception("expecting " +
                    Enum.GetName(typeof(TokenType), token) + "; " + lookahead.ToString());
        }

        public Token MatchToken(TokenType token)
        {
            var node = lookahead[lookaheadIndex];
            if (lookahead[lookaheadIndex].Type_ == token)
                ConsumeNextToken();
            else
                throw new Exception("expecting " +
                    Enum.GetName(typeof(TokenType), token) + "; " + lookahead.ToString());
            return node;
        }

        public Token NextToken()
        {
            return input.LookAhead();
        }

        public void Rewind()
        {
            input.Rewind();            
        }

        public bool CheckParserPath(TokenType token)
        {
            if (lookahead[lookaheadIndex].Type_ == token)
                return true;
            else
                return false;
        }

        public bool CheckParserPathLookAhead(TokenType token)
        {
            int tempLookaheadIndex = (lookaheadIndex + 1) % lookaheadCount;
            if (lookahead[tempLookaheadIndex].Type_ == token)
                return true;
            else
                return false;
        }

        public Token LookahreadToken(int index)
        {
            return lookahead[(lookaheadIndex + index + 1) % lookaheadCount];
        }

        public TokenType LookahreadTokenType(int index)
        {
            return LookahreadToken(index).Type_;
        }

        public Token CurrentToken()
        {
            return lookahead[lookaheadIndex];
        }

        public TokenType CurrentTokenType()
        {
            return lookahead[lookaheadIndex].Type_;
        }
    }
}
