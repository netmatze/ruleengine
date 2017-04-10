using SimpleExpressionEvaluator.AbstractSyntaxTree.AggregateFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class CollectionNode : AbstractSyntaxTreeNode
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

        public FilterNode filter;

        public FilterNode Filter
        {
            get { return filter; }
            set { filter = value; }
        }

        private AggregateNode aggregateFunction;
        public AggregateNode AggregateFunction
        {
            get { return aggregateFunction; }
            set { aggregateFunction = value; }
        }
    }
}
