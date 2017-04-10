using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class DoubleNode : AbstractSyntaxTreeNode
    {
        public double Value { get; set; }
    }
}
