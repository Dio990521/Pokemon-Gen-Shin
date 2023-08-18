using Game.Tool.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PokeGenshinUtils.StateMachine
{
    public class State<T> : MonoBehaviour
    {

        public virtual void Enter(T owner) { }

        public virtual void Execute() { }

        public virtual void Exit(bool sfx=true) { }

    }

}


