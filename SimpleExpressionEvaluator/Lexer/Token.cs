using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleExpressionEvaluator.Lexer
{
    public class Token
    {
        private TokenType type_;

        public TokenType Type_
        {
            get { return type_; }
            set { type_ = value; }
        }
        private string text;

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public Token(TokenType type, string text)
        {
            this.type_ = type;
            this.text = text;
        }

        public override string ToString()
        {
            return string.Format("<'{0}'>,{1}", text, Enum.GetName(typeof(TokenType), type_));
        }
    }
}
