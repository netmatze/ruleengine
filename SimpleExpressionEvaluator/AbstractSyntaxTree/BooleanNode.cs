using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class BooleanNode : AbstractSyntaxTreeNode
    {
        public bool Value { get; set; }
    }
}
