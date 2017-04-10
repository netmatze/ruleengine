using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LikeParser
{
    public class RegularExpressionPart
    {
        private bool isAtEnd;

        public bool IsAtEnd
        {
            get 
            {
                if (regularExpressionPosition >= regularExpression.Length - 1)
                {
                    return true;
                }
                return false;
            }            
        }

        public bool IsAfterEnd
        {
            get
            {
                if (regularExpressionPosition >= regularExpression.Length)
                {
                    return true;
                }
                return false;
            }
        }

        public bool IsAtStart
        {
            get
            {
                if (regularExpressionPosition == 0)
                {
                    return true;
                }
                return false;
            }
        }

        private bool isStartExpression;

        public bool IsStartExpression
        {
            get { return isStartExpression; }
            set { isStartExpression = value; }
        }

        private string regularExpression;

        public string RegularExpression
        {
            get { return regularExpression; }
            set { regularExpression = value; }
        }

        private int regularExpressionPosition;

        public int RegularExpressionPosition
        {
            get { return regularExpressionPosition; }
            set { regularExpressionPosition = value; }
        }

        public char ActualPart
        {
            get { return regularExpression[regularExpressionPosition]; }
        }

        public char PreviousPart
        {
            get
            {
                if (regularExpressionPosition > 0)
                {
                    return regularExpression[regularExpressionPosition - 1];
                }
                return regularExpression[regularExpressionPosition];
            }
        }
    }
}
