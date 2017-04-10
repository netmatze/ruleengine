using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class DateTimeNode : AbstractSyntaxTreeNode
    {
        public DateTime Value { get; set; }
    }
}
