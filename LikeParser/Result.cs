using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LikeParser
{
    public class Result
    {
        private ResultEnum result_;
        private int position;
        private int errorPosition;

        public int ErrorPosition
        {
            get { return errorPosition; }
            set { errorPosition = value; }
        }

        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        public ResultEnum Result_
        {
            get { return result_; }
            set { result_ = value; }
        }
    }

    public enum ResultEnum
    {
        OK,
        Error
    }
}
