using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class IntegerNode : AbstractSyntaxTreeNode
    {
        public int Value { get; set; }
    }
}
