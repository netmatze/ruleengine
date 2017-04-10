using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExpressionEvaluator.Lexer
{
    // LL(k) Lexer
    public abstract class Lexer
    {
        protected const char EOF = char.MinValue;
        protected char[] lookahead;
        protected int lookaheadCount;

        private string input;

        public string Input
        {
            get { return input; }
            set { input = value; }
        }

        protected int index = -1;
        protected int savedIndex = -1;
        protected int lookaheadIndex = 0;

        public Lexer(string input, int lookaheadCount)
        {
            this.input = input;
            this.lookaheadCount = lookaheadCount;
            lookahead = new char[lookaheadCount];
            for (int i = 1; i <= lookaheadCount; i++)
            {
                ConsumeItem();
            }
        }        

        public void ConsumeItem()
        {
            index++;
            if (index >= input.Length)
                lookahead[lookaheadIndex] = EOF;
            else
                lookahead[lookaheadIndex] = input[index];
            lookaheadIndex = (lookaheadIndex + 1) % lookaheadCount;        
        }

        public void DonotConsumeItem()
        {            
            if (index >= input.Length)
                lookahead[lookaheadIndex] = EOF;
            else
                lookahead[lookaheadIndex] = input[index];
        }

        public void Match(char chr)
        {
            if (lookahead[lookaheadIndex] == chr)
            {
                ConsumeItem();
            }
            else
            {
                throw new Exception("expecting" + chr + "; found " + lookahead[lookaheadIndex]);
            }
        }

        public abstract void Rewind();

        public abstract Token NextToken();

        public abstract Token LookAhead();
    }
}
