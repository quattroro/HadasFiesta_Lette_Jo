using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///FSM머신 MonsterLoveFSM을 참고해서 사용할 기능들만 
///빼와서 작업 하였습니다.
///사용할 상태들을 클래스에서
///public enum States
///{
///   Idle,
///   Walk,   
///   Run,
///}
///이런식으로 정의해주고
///각 상태에 대해서 Enter,Update,Exit단계에서 실행시키고 싶은 동작을
///Idle_Enter(),Idle_Update(),Idle_Exit()등등 이렇게 정의만 해주면
///일반함수인지, 코루틴 함수인지 상관없이 상태 변화에 따라 자동으로 실행해 줍니다.
///해당 FSM머신은 개인 프로젝트인 디아블로 모작 프로젝트에서 몬스터의 AI를 만드는데도 활용 하였습니다.
/////////////////////////////////////////////////////////////////////
///
namespace MyStateMachine
{
    public class Drive
    {
        public Action Update;
    }

    public class StateMachineRunner : MonoBehaviour
    {
        private List<IStateMachine<Drive>> stateMachineList = new List<IStateMachine<Drive>>();

        //private List<StateMachine<TState, Drive>> stateMachineList = new List<StateMachine<TState, Drive>>();
        public void Initialize<TState>(MonoBehaviour component) where TState : struct, IConvertible, IComparable
        {
            var fsm = new StateMachine<TState, Drive>(component);

            stateMachineList.Add(fsm);

        }

        //public void Initialize<TState, TDriver>(StateMachine<TState, TDriver> fsm) where TState : struct, IConvertible, IComparable
        //{
        //    stateMachineList.Add(fsm);
        //}

        void Update()
        {
            //for (int i = 0; i < stateMachineList.Count; i++)
            //{
            //    var fsm = stateMachineList[i];
            //    if (!fsm.IsInTransition && fsm.Component.enabled)
            //    {
            //        fsm.Driver.Update.Invoke();
            //    }
            //}


            for (int i = 0; i < stateMachineList.Count; i++)
            {
                var fsm = stateMachineList[i];
                if (!fsm.IsInTransition && fsm.Component.enabled)
                {
                    if(fsm.GetUpdateAction!=null)
                    {
                        int a = 0;
                    }
                    fsm.GetUpdateAction?.Invoke();
                }
            }
        }

    }
}

