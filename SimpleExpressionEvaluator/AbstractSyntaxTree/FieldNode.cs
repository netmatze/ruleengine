using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class FieldNode : AbstractSyntaxTreeNode
    {

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private VariableNode value;

        public VariableNode Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
