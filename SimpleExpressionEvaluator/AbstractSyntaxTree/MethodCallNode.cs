using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator.AbstractSyntaxTree
{
    public class MethodCallNode : AbstractSyntaxTreeNode
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<AbstractSyntaxTreeNode> parameterList = 
            new List<AbstractSyntaxTreeNode>();

        private int parameterCount;

        public int ParameterCount
        {
            get
            {
                return parameterList.Count;
            }
        }
    }
}
