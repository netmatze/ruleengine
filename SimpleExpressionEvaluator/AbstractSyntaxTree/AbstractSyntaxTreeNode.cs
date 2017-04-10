using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleExpressionEvaluator.Lexer;
//using StringTemplateEngine.TreeBasedInterpreter;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public abstract class AbstractSyntaxTreeNode
    {
        private Token token_;

        protected Token Token_
        {
            get { return token_; }
            set { token_ = value; }
        }

        public AbstractSyntaxTreeNode()
        {
        
        }

        public AbstractSyntaxTreeNode(Token token)
        {
            this.Token_ = token;
        }
    }
}
