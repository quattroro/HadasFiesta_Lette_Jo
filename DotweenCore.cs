using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///Dotween의 기능들을 직접 구현해 보았음
///Ease를 이용해 그래프 모양대로 움직임이 가능하고
///Sequence를 이용해서 여러개의 Tween을 하나로 묶어서 차례대로 실행이 가능하도록 제작 하였음
///캐릭터의 자동이동, 생성시킨 콜라이더나, 이펙트들을 자동으로 움직이게 할때 사용됨
/////////////////////////////////////////////////////////////////////


namespace MyDotween
{
    public class DotweenCore
    {
        public delegate void CallBackEvent();

        protected Dotween.Ease curEaseMode = Dotween.Ease.Linear;

        //x는 시간, 리턴값은 속도
        //https://easings.net/ko# 사이트에서 그래프 모양 확인 가능
        public float getEaseVal(float x)
        {
            //x = 0~1까지
            float y = 0;
            switch (curEaseMode)
            {
                case Dotween.Ease.Linear:
                    y = x;
                    break;

                case Dotween.Ease.easeInCubic:
                    y = x * x * x;
                    break;

                case Dotween.Ease.easeOutCubic:
                    y = 1.0f - Mathf.Pow(1.0f - x, 3.0f);
                    break;

                case Dotween.Ease.easeInOutCubic:
                    y = x < 0.5 ? 4.0f * x * x * x : 1.0f - Mathf.Pow(-2.0f * x + 2.0f, 3.0f) / 2.0f;
                    break;

                case Dotween.Ease.easeOutCirc:
                    y = Mathf.Sqrt(1.0f - Mathf.Pow(x - 1.0f, 2.0f));
                    break;

                case Dotween.Ease.easeInCirc:
                    y = 1 - Mathf.Sqrt(1.0f - Mathf.Pow(x, 2.0f));
                    break;

                case Dotween.Ease.easeInOutCirc:
                    y = x < 0.5 ? (1.0f - Mathf.Sqrt(1 - Mathf.Pow(2.0f * x, 2.0f))) / 2.0f : (Mathf.Sqrt(1.0f - Mathf.Pow(-2.0f * x + 2.0f, 2.0f)) + 1.0f) / 2.0f;
                    break;

            }
            return y;
        }

        //duration 시간동안 목표위치로 이동한다.
        public void DoMove(GameObject obj, Vector3 destpos, float duration, CallBackEvent _event = null)
        {
            Vector3 startpos = obj.transform.position;
            Vector3 directon = destpos - startpos;

            float timeval = 1 / duration;

            float speed = directon.magnitude / duration;

            CoroutineHandler.Start_Coroutine(CorDoMove(obj, destpos, duration, _event));
        }

        public IEnumerator CorDoMove(GameObject obj, Vector3 dest, float duration, CallBackEvent _event = null)
        {
            float testtime = Time.time;
            float startTime = Time.time;
            float lastTime = 0;
            float curtime = 0;
            //float countVal = Time.deltaTime;
            //목표시간까지의 움직여야 될 횟수
            //float maxCount = duration / countVal;

            //0~1까지의 값을 만든다고 할때 1회마다 증가할 값
            //float addval = 1 / maxCount;

            float curval = 0;
            float count = 0;

            Vector3 startpos = obj.transform.position;
            //목표까지의 방향과 거리
            Vector3 direction = dest - obj.transform.position;
            float distance = direction.magnitude;
            direction.Normalize();

            Vector3 start = obj.transform.position;

            lastTime = Time.time;
            while (true)
            {
                count++;
                if (count >= 10)
                {
                    int a = 0;
                }
                //curtime += Time.deltaTime;
                curtime += Time.time - lastTime;
                //지정한 시간이 되면 끝난다.
                if (curtime >= duration)
                {
                    obj.transform.position = startpos + (direction * getEaseVal(1) * distance);
                    _event?.Invoke();
                    Debug.Log(Time.time - testtime + "초 걸림");
                    yield break;
                }

                if (curval > 1)
                {
                    obj.transform.position = startpos + (direction * getEaseVal(1) * distance);
                    _event?.Invoke();
                    Debug.Log(Time.time - testtime + "초 걸림");
                    yield break;
                }


                obj.transform.position = startpos + (direction * getEaseVal(curval) * distance);
                curval = curtime / duration;
                lastTime = Time.time;
                //curval += addval;
                //count += countVal;

                yield return new WaitForSeconds(Time.deltaTime);
            }

        }










        //public IEnumerator CorDoMove(GameObject obj, Vector3 dest, float duration, CallBackEvent _event = null)
        //{
        //    float testtime = Time.time;
        //    float startTime = Time.time;
        //    float countVal = Time.deltaTime;
        //    //목표시간까지의 움직여야 될 횟수
        //    float maxCount = duration / countVal;

        //    //0~1까지의 값을 만든다고 할때 1회마다 증가할 값
        //    float addval = 1 / maxCount;

        //    float curval = 0;
        //    float count = 0;

        //    Vector3 startpos = obj.transform.position;
        //    //목표까지의 방향과 거리
        //    Vector3 direction = dest - obj.transform.position;
        //    float distance = direction.magnitude;
        //    direction.Normalize();

        //    Vector3 start = obj.transform.position;

        //    while (true)
        //    {
        //        //지정한 시간이 되면 끝난다.
        //        if (count >= maxCount)
        //        {
        //            _event?.Invoke();
        //            Debug.Log(Time.time - testtime + "초 걸림");
        //            yield break;
        //        }

        //        if (curval > 1)
        //        {
        //            _event?.Invoke();
        //            Debug.Log(Time.time - testtime + "초 걸림");
        //            yield break;
        //        }

        //        obj.transform.position = startpos + (direction * getEaseVal(curval) * distance);
        //        curval += addval;
        //        count += countVal;

        //        yield return new WaitForSeconds(countVal);
        //    }

        //}

    }
}

