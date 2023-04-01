using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///실제 Dotween의 Sequence를 구현해보기 위해 만들었습니다.
///묶여있는 Tween들을 순서대로 실행시켜주는 역할을 합니다.
/////////////////////////////////////////////////////////////////////

namespace MyDotween
{
    //여러개의 동작(Tween) 들을 묶어서 한번에 순차적으로 실행 시켜준다.
    public class Sequence
    {
        #region 원형 큐
        ////////////////////////////////////////////////////////////
        /// 원형큐
        ////////////////////////////////////////////////////////////
        int queueSize = 20;
        Tween[] queue;
        int Rear = 0;//맨 마지막원소의 위치 증가시키고 삽입
        int Front = 0;//맨 앞 원소의 한칸 앞

        public bool EnQueue(Tween[] _queue, Tween tween)
        {
            if ((Rear + 1) % queueSize == Front)
            {
                return false;
            }


            Rear = (Rear + 1) % queueSize;
            _queue[Rear] = tween;

            return true;
        }

        public Tween DeQueue(Tween[] _queue)
        {
            if (Front == Rear)
            {
                return null;
            }

            Front = (Front + 1) % queueSize;
            return _queue[Front];

        }

        public Tween Peek(Tween[] _queue)
        {
            if (Front == Rear)
            {
                return null;
            }

            return _queue[(Front + 1) % queueSize];
        }

        ////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////
        #endregion

        //독립적으로 실행되는 트윈들의 리스트
        List<Tween> InsertTweens = new List<Tween>();

        //루프타입
        Dotween.LoopType loopType;
        int loopCount = 0;
        int CurLoopCount = 0;

        //타이머
        CorTimeCounter TimeCounter = new CorTimeCounter();

        //루프를위해 시퀀스 처음 시작할때의 Front Rear를 저장해 놓기 위해 사용
        int sqCount = 0;
        int sqFront = 0;
        int sqRear = 0;
        Vector3 sqStart;

        //생성자
        public Sequence()
        {
            queue = new Tween[queueSize];
            Rear = Front = 0;

        }
        //생성자(size)
        public Sequence(int _QueueSize)
        {
            queueSize = _QueueSize;
            queue = new Tween[_QueueSize];
            Rear = Front = 0;

        }

        //큐에서 하나씩 빼서 실행하는데 뒤에있는 것이 조인설정이 있으면 해당 트윈도 동시 실행
        public void Start()
        {
            //if(sqCount==0)
            //{
            //    sqFront = Front;
            //    sqRear = Rear;
            //}

            if (InsertTweens.Count > 0)
            {
                foreach (Tween a in InsertTweens)
                {
                    CoroutineHandler.Start_Coroutine(TimeCounter.Cor_TimeCounter(a.StartTime, a.Start));
                }
            }

            Tween cur = DeQueue(queue);
            if (cur == null)
                return;

            Tween next = Peek(queue);
            if (next != null)
            {
                if (next.Join != null)
                {
                    next.Start();
                    Debug.Log("시퀀스 조인 실행" + Front);
                    DeQueue(queue);
                }
            }
            else
            {
                Debug.Log("넥스트 널");
            }

            cur.OnEnd(TweenEnd);
            cur.Start();

            //sqCount++;

            Debug.Log("시퀀스 실행" + Front);
        }


        public void TweenEnd()
        {
            Debug.Log("시퀀스 끝");

            //하나의 동작이 끝났으면 앞으로 동작이 남아있는지 확인하고 남은 동작이 없으면 
            //루프타입에 따라 다시 동작을 해준다.
            if (Peek(queue) == null)
            {
                //루프설정 만들 필요

                //if(CurLoopCount>=loopCount)
                //{
                //    CurLoopCount++;
                //    sqCount = 0;
                //    Front = sqFront;
                //    Rear = sqRear;
                //    Start();
                //}
            }
            else
            {
                Start();
            }
        }


        //루프횟수가 -1이면 무한루프
        public Sequence SetLoop(int loops/*루프횟수*/, Dotween.LoopType loopType = Dotween.LoopType.Restart)
        {
            this.loopType = loopType;
            this.loopCount = loops;

            return this;
        }


        //맨 마지막에 추가
        public Sequence Append(Tween tween)
        {
            Debug.Log("Append 추가");
            EnQueue(queue, tween);
            return this;
        }

        //순서와 관계없이 일정 시간 이후에 시작
        public Sequence Insert(float inserttime, Tween tween)
        {
            tween.StartTime = inserttime;
            InsertTweens.Add(tween);
            return this;
        }

        //앞에 추가된 트윈과 동시 시작
        public Sequence Join(Tween tween)
        {
            Debug.Log("Join 추가");
            tween.Join = Peek(queue);
            EnQueue(queue, tween);
            return this;
        }

        //맨 처음에 시작
        public Sequence Prepend(Tween tween)
        {
            int tempslot = 0;
            if (Front == 0)
            {
                tempslot = queueSize - 1;
            }
            else
            {
                tempslot = Front - 1;
            }

            if (tempslot == Rear)
            {
                Debug.LogError("큐가 가득찼습니다! 삽입 불가능!");
                return null;
            }

            queue[Front] = tween;
            Front = tempslot;

            return this;
        }
    }
}

