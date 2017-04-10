using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleEngine
{
    public class ExpressionRuleLoader
    {
        public Rule Load(int id)
        {
            switch (id)
            {
                case 1: return new Rule(" Name = 'Mathias' ");
                case 2: return new Rule(" Age = 35 ");
                case 3: return new Rule(" Age = 100 % 64 ");
                case 4: return new Rule(" Children * 2.0 + 5.0 = 9.0 && Married = true && Age = Children * 18 ");                
                default:
                    return null;
            }
        }        
    }
}
