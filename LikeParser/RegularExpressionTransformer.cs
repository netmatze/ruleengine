using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LikeParser
{
    public class RegularExpressionTransformer
    {
        private string regularExpression;

        public RegularExpressionTransformer(string regularExpression)
        {
            this.regularExpression = regularExpression;
            RegularExpressionPart regularExpressionPart = new RegularExpressionPart()
            {
                RegularExpression = regularExpression,
                RegularExpressionPosition = 0
            };
        }

        public bool Match(string text)
        {
            return true;
        }
    }
}
