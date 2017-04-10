using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LikeParser
{
    public class State
    {
        private List<StatusTransition> statusTransitions_ = new List<StatusTransition>();

        public List<StatusTransition> StatusTransitions_
        {
            get { return statusTransitions_; }
            set { statusTransitions_ = value; }
        }

        private bool isErrorState;

        public bool IsErrorState
        {
            get { return isErrorState; }
            set { isErrorState = value; }
        }

        private bool isPreviewEndeState;

        public bool IsPreviewEndeState
        {
            get { return isPreviewEndeState; }
            set { isPreviewEndeState = value; }
        }

        private bool isFirstPosition;

        public bool IsFirstPosition
        {
            get { return isFirstPosition; }
            set { isFirstPosition = value; }
        }

        private int number;

        public int Number
        {
            get { return number; }
            set { number = value; }
        }

        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        
        public State(int number, string value, bool isErrorState = false, bool isFirstPosition = false)
        {
            this.value = value;
            this.isFirstPosition = isFirstPosition;
            this.number = number;
            this.isErrorState = isErrorState;
        }

        public State Check(char value, StateMaschine stateMaschine)
        {
            foreach(StatusTransition statusTransition in StatusTransitions_)
            {                               
                if(statusTransition.Status_.Value == value.ToString())
                {                 
                    return statusTransition.Status_;
                }
                else if (statusTransition.Status_.Value == LexerItems.QuestionMark.ToString())
                {                    
                    return statusTransition.Status_;
                }
                else if (statusTransition.Status_.Value == LexerItems.Percent.ToString())
                {
                    if (statusTransition.Status_.StatusTransitions_[0].Status_.Value != "Ende")
                    {
                        State returnState = statusTransition.Status_.Check(value, stateMaschine);
                        if (!returnState.IsErrorState)
                        {
                            return returnState;
                        }
                    }
                    else
                    {
                        return new State(-1, "End", false);
                    }
                }                
            }
            return new State(-1, "Error", true);
        }        
    }
}
