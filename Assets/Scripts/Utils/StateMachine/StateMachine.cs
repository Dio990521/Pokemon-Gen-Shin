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
            Debug.Log("Push: " + newState.ToString());
            StateStack.Push(newState);
            CurrentState = newState;
            CurrentState.Enter(_owner);
        }

        public IEnumerator PushAndWait(State<T> newState)
        {
            var oldState = CurrentState;
            Debug.Log("Push and wait: " + newState.ToString());
            Push(newState);
            yield return new WaitUntil(() => CurrentState == oldState);
        }

        public void Pop(bool sfx=true)
        {
            if (StateStack.Count > 0)
            {
                Debug.Log("Pop: " + StateStack.Peek().ToString());
            }
            else
            {
                Debug.Log("Empty Stack cannot pop!!");
            }
            StateStack.Pop();
            CurrentState.Exit(sfx);
            CurrentState = StateStack.Peek();
        }

        public void ChangeState(State<T> newState)
        {
            Debug.Log("Change state: " + newState.ToString());
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


