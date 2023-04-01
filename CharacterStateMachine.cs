//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

////jo
////현재 캐릭터의 상태를 관리
////
//public class CharacterStateMachine : MySingleton<CharacterStateMachine>
//{

//    public enum States
//    {
//        Idle,
//        Walk,
//        Move,
//        Attack,
//        Rolling,
//        Guard,
//        GuardStun,
//        OutOfControl,
//    }


//    public enum eCharacterState
//    {
//        Idle,//기본
//        Move,//움직임
//        Attack,//공격
//        Rolling,//회피
//        Guard,//가드
//        GuardStun,//그냥 스턴과 가드 중 에서의 스턴을 구분하기 위해(그냥 스턴은 피격 당하지만 가드스턴은 피격당하지 않음)
//        OutOfControl,//스턴, 넉다운 등등

//        //NoDamage,//무적



//        StateMax
//    }

//    [System.Serializable]
//    public class AnimationBlendingTimeSet
//    {
//        public eCharacterState prestate;
//        public eCharacterState changestate;
//        [Range(0.0f, 5.0f)]
//        public float blendtime;
        
//    }

//    //모션의 딜레이는 각각의 모션이 종료할때 각자가 가지고 있도록 한다.

//    public List<AnimationBlendingTimeSet> animationBlendingTimeSets = new List<AnimationBlendingTimeSet>();
//    //curstate
//    public eCharacterState CurState;
//    //
//    public eCharacterState PreState;

//    public AnimationController animator;
//    public CMoveComponent movecom;

//    //상태 변화에 따라 애니메이션을 바꿔준다.
//    public void SetState(eCharacterState state)
//    {
//        if (CurState != state)
//        {
//            //Debug.Log($"{state} 들어옴");
//            PreState = CurState;
//            CurState = state;
//        }
//    }

//    private void Start()
//    {
//        animator = GetComponentInChildren<AnimationController>();
//    }

//    ////이전 상태로 돌아간다.
//    //public void SetPreState()
//    //{
//    //    if (CurState != state)
//    //    {
//    //        //Debug.Log($"{state} 들어옴");
//    //        PreState = CurState;
//    //        CurState = state;
//    //    }
//    //}

//    public eCharacterState GetState()
//    {
//        return CurState;
//    }

//    public eCharacterState GetPreState()
//    {
//        return PreState;
//    }

//}
