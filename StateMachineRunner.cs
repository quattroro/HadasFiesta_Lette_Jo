using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///������ �۾�
///FSM�ӽ� MonsterLoveFSM�� �����ؼ� ����� ��ɵ鸸 
///���ͼ� �۾� �Ͽ����ϴ�.
///����� ���µ��� Ŭ��������
///public enum States
///{
///   Idle,
///   Walk,   
///   Run,
///}
///�̷������� �������ְ�
///�� ���¿� ���ؼ� Enter,Update,Exit�ܰ迡�� �����Ű�� ���� ������
///Idle_Enter(),Idle_Update(),Idle_Exit()��� �̷��� ���Ǹ� ���ָ�
///�Ϲ��Լ�����, �ڷ�ƾ �Լ����� ������� ���� ��ȭ�� ���� �ڵ����� ������ �ݴϴ�.
///�ش� FSM�ӽ��� ���� ������Ʈ�� ��ƺ�� ���� ������Ʈ���� ������ AI�� ����µ��� Ȱ�� �Ͽ����ϴ�.
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

