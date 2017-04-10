using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LikeParser
{
    public class StateMaschineBuilder
    {
        private Stack<State> bracketOperations = new Stack<State>();        

        public StateMaschine Match(string regularExpression)
        {
            RegularExpressionPart regularExpressionPart = new RegularExpressionPart()
            {
                RegularExpression = regularExpression,
                RegularExpressionPosition = 0
            };            
            StateMaschine stateMaschine = new StateMaschine();
            var result = LoopMatch(regularExpressionPart, stateMaschine);
            return stateMaschine;
        }

        public bool Recognize(string text, StateMaschine stateMaschine)
        {
            stateMaschine.RecognizeMode();
            var length = text.Length;
            stateMaschine.TextLength = length;
            for (int i = 0; i < length; i++)
            {
                var character = text.ToArray()[i];
                var recognized = stateMaschine.Recognize(character);
                if(recognized.Item2)
                {
                    return stateMaschine.ReachedEndeState;
                }
                if (!recognized.Item1)
                {
                    return false;
                }
            }
            return stateMaschine.ReachedEndeState;
        }
        
        public StateMaschine LoopMatch(RegularExpressionPart regularExpressionPart, StateMaschine stateMaschine)
        {
            var length = regularExpressionPart.RegularExpression.ToArray().Length;
            State previousState = null;
            State jumpState = null;
            var minLength = 0;
            for(int i = 0; i < length; i++)
            {
                var actualIndex = i;
                var character = regularExpressionPart.RegularExpression.ToArray()[i];                
                if (character == LexerItems.QuestionMark)
                {
                    previousState = stateMaschine.AddState(actualIndex, character.ToString(), previousState);
                    minLength++;
                    jumpState = null;
                }
                else if (character == LexerItems.Percent)
                {
                    if (i == 0)
                    {
                        State tempPreviousState = previousState;
                        previousState = stateMaschine.AddState(actualIndex, character.ToString(), previousState, true);                        
                    }
                    else
                    {
                        State tempPreviousState = previousState;
                        previousState = stateMaschine.AddState(actualIndex, character.ToString(), previousState);                       
                    }
                }                
                //else if (character == LexerItems.QuestionMark)
                //{
                //    previousState = stateMaschine.AddState(actualIndex, character.ToString(), previousState);
                //}
                //else if (character == LexerItems.OpenBracket)
                //{
                //    State state = new State(actualIndex, character.ToString());
                //    bracketOperations.Push(state);
                //    previousState = stateMaschine.AddState(state, previousState);
                //}
                //else if (character == LexerItems.CloseBracket)
                //{
                //    State state = new State(actualIndex, character.ToString());
                //    previousState = stateMaschine.AddState(state, previousState);
                //    var tempState = bracketOperations.Pop();
                //    if (tempState.Value == LexerItems.Pipe.ToString())
                //    {
                //        tempState.StatusTransitions_.Add(new StatusTransition(state));
                //        tempState = bracketOperations.Pop();
                //        if (tempState.Value != LexerItems.Pipe.ToString())
                //        {
                //            jumpState = tempState;
                //        }
                //    }
                //    else
                //    {
                //        jumpState = tempState;
                //    }
                //}
                //else if (character == LexerItems.Pipe)
                //{
                //    State state = new State(actualIndex, character.ToString());
                //    State bracketOperation = bracketOperations.Peek();                    
                //    bracketOperations.Push(state);
                //    previousState = stateMaschine.AddState(state, previousState);
                //    var previewCharacter = regularExpressionPart.RegularExpression.ToArray()[i + 1];
                //    State nextCharacter = new State(actualIndex + 1, previewCharacter.ToString());
                //    bracketOperation.StatusTransitions_.Add(new StatusTransition(nextCharacter));
                //    if (nextCharacter.Value == LexerItems.OpenBracket.ToString())
                //    {
                //        bracketOperations.Push(nextCharacter);
                //    }
                //    stateMaschine.ActualState = nextCharacter;
                //    previousState = nextCharacter;
                //    i++;
                //    jumpState = null;
                //    minLength = 0;
                //}
                else
                {
                    previousState = stateMaschine.AddState(actualIndex, character.ToString(), previousState);
                    minLength++;
                    jumpState = null;
                }
                if (stateMaschine.MinLength < minLength)
                    stateMaschine.MinLength = minLength;
            }
            previousState.IsPreviewEndeState = true;
            stateMaschine.AddState(length, "Ende", previousState);
            return stateMaschine;
        }
    }
}
