using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class StringNode : AbstractSyntaxTreeNode
    {
        public string Value { get; set; }
    }
}
