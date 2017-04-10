using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace SimpleExpressionEvaluator
{
    public class DynamicBaseClass : DynamicObject
    {
        public string Name { get; set; }

        public string ReferenceName { get; set; }

        public ObjectType Type { get; set; }

        public List<dynamic> Fields = new List<dynamic>();
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return base.TryGetMember(binder, out result);
        }
    }
}
