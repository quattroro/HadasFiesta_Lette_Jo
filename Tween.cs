using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///Dotween의 기능들을 직접 구현해 보았습니다.
///해당 Tween단독으로도 사용이 가능 하지만 
///Sequence에 여러가지 Tween을 묶어서 하나하나 차례대로 실행이 되도록 할 수도 있습니다.
/////////////////////////////////////////////////////////////////////

namespace MyDotween
{
    public class Tween : DotweenCore
    {
        public float StartTime = -1;

        public Tween Join;
        public Vector3 destpos;
        public GameObject TargetObj;
        public float Duration;
        public Dotween.LoopType LoopType;
        //public Dotween.Ease Ease;

        //public delegate void CallBackEvent();

        private CallBackEvent startevent;
        private CallBackEvent endevent;

        //public Tween()
        //{
        //    Join = null;
        //    destpos = Vector3.zero;
        //    TargetObj = null;
        //    Duration = 0;
        //    LoopType = Dotween.LoopType.Restart;
        //    curEaseMode = Dotween.Ease.Linear;
        //    StartTime = -1;
        //}


        public enum TweenType
        {
            Move,
            Scale,
            Rotate,
            LocalMove
        }


        public Tween(GameObject _target, Vector3 _dest, float _duration, Dotween.Ease ease = Dotween.Ease.Linear)
        {
            StartTime = -1;
            Join = null;
            destpos = _dest;
            TargetObj = _target;
            Duration = _duration;
            LoopType = Dotween.LoopType.None;
            curEaseMode = ease;
        }

        //확장 예정
        //public static Tween DoMove()
        //{

        //}

        //public static Tween DoScale()
        //{


        //}

        //public static Tween DoRotate()
        //{


        //}


        public void Start()
        {
            if (startevent != null)
                startevent();

            Debug.Log("트윈 실행");
            DoMove(TargetObj, destpos, Duration, End);
        }


        public void End()
        {
            Debug.Log("트윈 끝");
            if (endevent != null)
                endevent();


        }

        //동작이 실행될때 실행될 이벤트
        public void OnStart(CallBackEvent _event)
        {
            startevent += _event;
        }

        //동작이 끝날때 실행될 이벤트
        public void OnEnd(CallBackEvent _event)
        {
            endevent += _event;
        }

        //일정 시간 이후에 시작
        public void Start(float StartTime)
        {

        }
    }




}

