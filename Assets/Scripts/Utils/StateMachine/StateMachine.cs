using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PokeGenshinUtils.StateMachine
{
    public class StateMachine<T>
    {
        public State<T> CurrentState { get; private set; }
        public Stack<State<T>> StateStack { get; private set; }
        private T _owner;
        public StateMachine(T owner) 
        {
            _owner = owner;
            StateStack = new Stack<State<T>>();
        }

        public void Execute()
        {
            CurrentState?.Execute();
        }

        public void Push(State<T> newState)
        {
            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.Enter(_owner);
        }

        public IEnumerator PushAndWait(State<T> newState)
        {
            var oldState = CurrentState;
            Push(newState);
            yield return new WaitUntil(() => CurrentState == oldState);
        }

        public void Pop(bool sfx=true)
        {
            //Debug.Log("POP " + StateStack.Peek().ToString());
            StateStack.Pop();
            CurrentState.Exit(sfx);
            CurrentState = StateStack.Peek();
        }

        public void ChangeState(State<T> newState)
        {
            if (CurrentState != null)
            {
                StateStack.Pop();
                CurrentState.Exit();
            }

            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.Enter(_owner);
        }

        public State<T> GetPrevState()
        {
            return StateStack.ElementAt(1);
        }
    }
}


