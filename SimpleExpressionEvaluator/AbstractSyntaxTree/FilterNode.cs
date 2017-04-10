using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class FilterNode : AbstractSyntaxTreeNode
    {
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public List<AbstractSyntaxTreeNode> FilterItems = new List<AbstractSyntaxTreeNode>();
    }
}
