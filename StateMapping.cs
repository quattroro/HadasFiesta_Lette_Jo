using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///FSM머신에서 사용자가 만들어준 함수들을 미리 매핑해놓는 클래스 입니다.
///
/////////////////////////////////////////////////////////////////////

namespace MyStateMachine
{
    public class StateMapping<TState, TDriver> where TState : struct, IConvertible, IComparable where TDriver : class, new()
    {
        public TState state;

        public bool hasEnterRoutine = false;
        public Action EnterCall = null;
        public Func<IEnumerator> EnterRoutine = null;

        public bool hasExitRoutine = false;
        public Action ExitCall = null;
        public Func<IEnumerator> ExitRoutine = null;

        public bool hasUpdateRoutine = false;
        public Action UpdateCall = null;
        public Func<IEnumerator> UpdateRoutine = null;

        private Func<TState> stateProviderCallback;
        private StateMachine<TState, TDriver> fsm;

        public StateMapping(StateMachine<TState, TDriver> fsm, TState state, Func<TState> stateProvider)
        {
            this.fsm = fsm;
            this.state = state;
            stateProviderCallback = stateProvider;
        }

        public StateMapping(StateMachine<TState, TDriver> fsm, TState state)
        {
            this.fsm = fsm;
            this.state = state;
            //stateProviderCallback = stateProvider;
        }
    }
}

