using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleExpressionEvaluator.Lexer;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class VariableNode : AbstractSyntaxTreeNode
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private object value;

        public object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
