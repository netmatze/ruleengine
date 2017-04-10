using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LikeParser
{
    public class StatusTransition
    {
        private State transitionStatus_;

        public State Status_
        {
            get { return transitionStatus_; }
            set { transitionStatus_ = value; }
        }

        public bool Marked
        {
            get;
            set;
        }

        public StatusTransition(State transitionStatus)
        {
            this.transitionStatus_ = transitionStatus;
        }
    }
}
