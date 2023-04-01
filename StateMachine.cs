using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;


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
///해당 기능은 리플렉션 기능을 이용하기 때문에 성능에 영향을 끼칠 수 있습니다.
///따라서 리플렉션 기능은 오직 초기화 단계에서만 사용이 되도록 하였습니다.
///해당 FSM머신은 개인 프로젝트인 디아블로 모작 프로젝트에서 몬스터의 AI를 만드는데도 활용 하였습니다.
/////////////////////////////////////////////////////////////////////

public enum States
{
    Idle,
    Walk,
    Run,
    Attack,
    Rolling,
    Guard,
    GuardStun,
    OutOfControl,
    AutoMove,
    AreaOfEffect,
}

namespace MyStateMachine
{

    public enum StateTransition
    {
        Safe,
        Overwrite,
    }
    public interface IStateMachine<TDriver>
    {
        MonoBehaviour Component { get; }
        TDriver Driver { get; }
        bool IsInTransition { get; }

        Action GetUpdateAction { get; }
    }

    public class StateMachine<TState, TDriver> : IStateMachine<TDriver> where TState : struct, IConvertible, IComparable where TDriver : class, new()
    {
        private MonoBehaviour component;

        private TDriver rootDriver;

        private bool isInTransition = false;

        public StateMapping<TState, TDriver> curState;
        public StateMapping<TState, TDriver> lastState;
        public StateMapping<TState, TDriver> destState;
        public StateMapping<TState, TDriver> queuedState;


        public IEnumerator enterRoutine;
        public IEnumerator exitRoutine;
        private IEnumerator currentTransition;
        private IEnumerator queuedChange;

        public Dictionary<object, StateMapping<TState, TDriver>> states;
        public Dictionary<TState, TDriver> stateMapping = new Dictionary<TState, TDriver>();

        //리플렉션을 이용해서 
        //해당 클래스의 메소드를 찾아서 매핑을 해준다.
        public StateMachine(MonoBehaviour component)
        {
            this.component = component;

            var enumValues = Enum.GetValues(typeof(TState));

            //해당 객체의 상태의 개수만큼 미리 매핑정보 클래스를 만들어 놓는다.
            states = new Dictionary<object, StateMapping<TState, TDriver>>();
            for (int i = 0; i < enumValues.Length; i++)
            {
                var mapping = new StateMapping<TState, TDriver>(this, (TState)enumValues.GetValue(i)/*, curState*/);
                states.Add(mapping.state, mapping);
            }


            //해당 스크립트의 메소드들을 받아와서
            MethodInfo[] methods = component.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            for (int i = 0; i < methods.Length; i++)
            {
                TState state;
                string evtName;

                //상태이름과 함수 이름을 나눠준다.
                if (!ParseName(methods[i], out state, out evtName))
                {
                    continue; //Skip methods where State_Event name convention could not be parsed
                }

                StateMapping<TState, TDriver> mapping = states[state];
                BindEventsInternal(mapping, component, methods[i], evtName);

                //stateMapping.Add()

                //StateMapping<TState, TDriver> mapping = stateLookup[state];

                //if (eventFieldsLookup.ContainsKey(evtName))
                //{
                //    //Bind methods defined in TDriver
                //    // driver.Foo.AddListener(StateOne_Foo);
                //    FieldInfo eventField = eventFieldsLookup[evtName];
                //    BindEvents(rootDriver, component, state, enumConverter(state), methods[i], eventField);
                //}
                //else
                //{
                //    //Bind Enter, Exit and Finally Methods
                //    BindEventsInternal(mapping, component, methods[i], evtName);
                //}
            }


            //curState = null;
        }

        public void InitMachineRunner()
        {
            var engine = component.GetComponent<StateMachineRunner>();
            if (engine == null) engine = component.gameObject.AddComponent<StateMachineRunner>();
            engine.Initialize<TState>(component);
        }

        static void BindEventsInternal(StateMapping<TState, TDriver> targetState, Component component, MethodInfo method, string evtName)
        {
            switch (evtName)
            {
                case "Enter":
                    //TDriver driver = 
                    //stateMapping.Add()

                    //사용자가 만든 메소드가 코루틴일때
                    if (method.ReturnType == typeof(IEnumerator))
                    {
                        targetState.hasEnterRoutine = true;
                        targetState.EnterRoutine = CreateDelegate<Func<IEnumerator>>(method, component);
                        //targetState.EnterRoutine = Delegate.CreateDelegate(typeof(Func<IEnumerator>),method, component);
                    }
                    else//일반 메소드 일때
                    {
                        targetState.hasEnterRoutine = false;

                        //그냥 이렇게 넣으려고 하면 형변환 때문에 문제가 생기기 때문에 탬플릿 함수를 하나 만들어서 넣어준다.
                        //targetState.EnterCall = Delegate.CreateDelegate(typeof(Action), method, component);
                        targetState.EnterCall = CreateDelegate<Action>(method, component);
                    }

                    break;

                case "Exit":
                    if (method.ReturnType == typeof(IEnumerator))
                    {
                        targetState.hasExitRoutine = true;
                        targetState.ExitRoutine = CreateDelegate<Func<IEnumerator>>(method, component);
                    }
                    else
                    {
                        targetState.hasExitRoutine = false;
                        targetState.ExitCall = CreateDelegate<Action>(method, component);
                    }

                    break;


                case "Update":
                    if (method.ReturnType == typeof(IEnumerator))
                    {
                        targetState.hasUpdateRoutine = true;
                        targetState.UpdateRoutine = CreateDelegate<Func<IEnumerator>>(method, component);
                    }
                    else
                    {
                        targetState.hasUpdateRoutine = false;
                        targetState.UpdateCall = CreateDelegate<Action>(method, component);
                    }

                    break;
                    //case "Finally":
                    //    targetState.Finally = CreateDelegate<Action>(method, component);
                    //    break;
            }
        }

        static V CreateDelegate<V>(MethodInfo method, System.Object target) where V : class
        {
            var ret = (Delegate.CreateDelegate(typeof(V), target, method) as V);

            if (ret == null)
            {
                throw new ArgumentException("Unable to create delegate for method called " + method.Name);
            }

            return ret;
        }

        static bool ParseName(MethodInfo methodInfo, out TState state, out string eventName)
        {
            state = default(TState);
            eventName = null;

            //if (methodInfo.GetCustomAttributes(typeof(CompilerGeneratedAttribute), true).Length != 0)
            //{
            //    return false;
            //}

            string name = methodInfo.Name;
            //이름에서 _ 의 위치를 찾는다.
            int index = name.IndexOf('_');

            //Ignore functions without an underscore
            if (index < 0)
            {
                return false;
            }

            //_의 앞에 있는 문자
            string stateName = name.Substring(0, index);
            //_의 뒤에 있는 문자
            eventName = name.Substring(index + 1);

            try
            {
                state = (TState)Enum.Parse(typeof(TState), stateName);
            }
            catch (ArgumentException)
            {
                //Not an method as listed in the state enum
                return false;
            }

            return true;
        }

        public void ChangeState(TState newState)
        {
            ChangeState(newState, StateTransition.Safe);
        }

        public void ChangeState(TState newState, StateTransition transMode)
        {

            //바뀔 상태가 바인딩이 되어있는지 확인하고
            if (!states.ContainsKey(newState))
            {
                throw new Exception("No state with the name " + newState.ToString() + " can be found. Please make sure you are called the correct type the statemachine was initialized with");
            }

            if (queuedState != null)
                return;
            //
            var nextState = states[newState];

            //상태가 현재 상태와 같으면 그냥 리턴
            if (curState == nextState)
            {
                return;
            }

            //이미 진행중인 프로세스들이 있을때
            if (exitRoutine != null)
            {
                //
                destState = nextState;
            }
            if (enterRoutine != null)
            {
                queuedChange = WaitForPreviousTransition(nextState);
                component.StartCoroutine(queuedChange);
            }



            //현재 상태의 exit 또는 다음 상태의 Enter 함수가 코루틴ㅇ일때
            if ((curState != null && curState.hasExitRoutine) || nextState.hasEnterRoutine)
            {
                isInTransition = true;
                currentTransition = ChangeStateRoutine(nextState);
                component.StartCoroutine(currentTransition);
            }
            else
            {
                destState = nextState;
                if (curState != null)
                {
                    if(curState.ExitCall!=null)
                        curState.ExitCall();
                    //라스트 이벤트
                }

                lastState = curState;
                curState = destState;

                if (curState != null)
                {
                    if(curState.EnterCall!=null)
                        curState.EnterCall();
                    //체인지 이벤트
                }
                isInTransition = false;
            }

            

            //둘다 모두 일반함수일때는 실행시켜주고 상태를 바꿔준다.

            //switch (transition)
            //{
            //    //case StateMachineTransition.Blend:
            //    //Do nothing - allows the state transitions to overlap each other. This is a dumb idea, as previous state might trigger new changes. 
            //    //A better way would be to start the two couroutines at the same time. IE don't wait for exit before starting start.
            //    //How does this work in terms of overwrite?
            //    //Is there a way to make this safe, I don't think so? 
            //    //break;
            //    case StateTransition.Safe:
            //        if (isInTransition)
            //        {
            //            if (exitRoutine != null) //We are already exiting current state on our way to our previous target state
            //            {
            //                //Overwrite with our new target
            //                destinationState = nextState;
            //                return;
            //            }

            //            if (enterRoutine != null) //We are already entering our previous target state. Need to wait for that to finish and call the exit routine.
            //            {
            //                //Damn, I need to test this hard
            //                queuedChange = WaitForPreviousTransition(nextState);
            //                component.StartCoroutine(queuedChange);
            //                return;
            //            }
            //        }

            //        break;
            //    case StateTransition.Overwrite:
            //        if (currentTransition != null)
            //        {
            //            component.StopCoroutine(currentTransition);
            //        }

            //        if (exitRoutine != null)
            //        {
            //            component.StopCoroutine(exitRoutine);
            //        }

            //        if (enterRoutine != null)
            //        {
            //            component.StopCoroutine(enterRoutine);
            //        }

            //        //Note: if we are currently in an EnterRoutine and Exit is also a routine, this will be skipped in ChangeToNewStateRoutine()
            //        break;
            //}

        }

        IEnumerator WaitForPreviousTransition(StateMapping<TState, TDriver> nextState)
        {
            queuedState = nextState; //Cache this so fsm.NextState is accurate;

            while (isInTransition)
            {
                yield return null;
            }

            queuedState = null;

            ChangeState((TState)nextState.state);
        }

        public IEnumerator ChangeStateRoutine(StateMapping<TState, TDriver> newState)
        {
            destState = newState;

            //exit가 코루틴일때
            if (curState.hasExitRoutine)
            {
                exitRoutine = curState.ExitRoutine();
                yield return component.StartCoroutine(exitRoutine);
                exitRoutine = null;
            }
            else
            {
                if(curState.ExitCall!=null)
                    curState.ExitCall();
            }


            lastState = curState;
            curState = destState;

            if (curState.hasEnterRoutine)
            {
                enterRoutine = curState.EnterRoutine();

                yield return component.StartCoroutine(enterRoutine);
                enterRoutine = null;
            }
            else
            {
                if (curState.EnterCall != null)
                    curState.EnterCall();
            }

            isInTransition = false;
        }

        //static void BindEventsInternal(StateMapping<TState, TDriver> targetState, Component component, MethodInfo method, string evtName)
        //{
        //    switch (evtName)
        //    {
        //        case "Enter":
        //            if (method.ReturnType == typeof(IEnumerator))
        //            {
        //                targetState.hasEnterRoutine = true;
        //                targetState.EnterRoutine = CreateDelegate<Func<IEnumerator>>(method, component);
        //            }
        //            else
        //            {
        //                targetState.hasEnterRoutine = false;
        //                targetState.EnterCall = CreateDelegate<Action>(method, component);
        //            }

        //            break;

        //        case "Exit":
        //            if (method.ReturnType == typeof(IEnumerator))
        //            {
        //                targetState.hasExitRoutine = true;
        //                targetState.ExitRoutine = CreateDelegate<Func<IEnumerator>>(method, component);
        //            }
        //            else
        //            {
        //                targetState.hasExitRoutine = false;
        //                targetState.ExitCall = CreateDelegate<Action>(method, component);
        //            }

        //            break;

        //        case "Finally":
        //            targetState.Finally = CreateDelegate<Action>(method, component);
        //            break;
        //    }
        //}


        //static Dictionary<object, StateMapping<TState, TDriver>> CreateStateLookup(StateMachine<TState, TDriver> fsm, Array values)
        //{
        //    var stateLookup = new Dictionary<object, StateMapping<TState, TDriver>>();
        //    for (int i = 0; i < values.Length; i++)
        //    {
        //        var mapping = new StateMapping<TState, TDriver>(fsm, (TState)values.GetValue(i), fsm.GetState);
        //        stateLookup.Add(mapping.state, mapping);
        //    }

        //    return stateLookup;
        //}



        public TState GetCurState()
        {
            return curState.state;
        }

        public TState GetPreState()
        {
            return lastState.state;
        }



        public bool IsInTransition
        {
            get { return isInTransition; }
        }

        public TDriver Driver
        {
            get { return rootDriver; }
        }

        public MonoBehaviour Component
        {
            get { return component; }
        }

        public Action GetUpdateAction 
        { 
            get
            {
                if (curState == null)
                    return null;

                return curState.UpdateCall;
            }
        }
    }
}

