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
    //작성된 curEaseMode 그래프모양 대로 지정한 위치에 지정한 시간동안 움직인다.
    public class Dotween
    {
        public enum Ease
        {
            Linear,
            easeInCubic,
            easeOutCubic,
            easeInOutCubic,
            easeOutCirc,
            easeInCirc,
            easeInOutCirc,
            easeMax
        }

        public enum LoopType
        {
            None = -1,
            Restart,
            Incremental,
            Yoyo
        }

        Ease curEaseMode = Ease.Linear;
        GameObject CurMoveObject = null;


        //루프횟수가 -1이면 무한루프
        public void SetLoop(int loops, Dotween.LoopType loopType = Dotween.LoopType.Restart)
        {

        }


        //x는 시간, 리턴값은 속도
        //https://easings.net/ko# 사이트에서 그래프 모양 확인 가능
        public float getEaseVal(float x)
        {
            //x = 0~1까지
            float y = 0;
            switch (curEaseMode)
            {
                case Ease.Linear:
                    y = x;
                    break;

                case Ease.easeInCubic:
                    y = x * x * x;
                    break;

                case Ease.easeOutCubic:
                    y = 1.0f - Mathf.Pow(1.0f - x, 3.0f);
                    break;

                case Ease.easeInOutCubic:
                    y = x < 0.5 ? 4.0f * x * x * x : 1.0f - Mathf.Pow(-2.0f * x + 2.0f, 3.0f) / 2.0f;
                    break;

                case Ease.easeOutCirc:
                    y = Mathf.Sqrt(1.0f - Mathf.Pow(x - 1.0f, 2.0f));
                    break;

                case Ease.easeInCirc:
                    y = 1 - Mathf.Sqrt(1.0f - Mathf.Pow(x, 2.0f));
                    break;

                case Ease.easeInOutCirc:
                    y = x < 0.5 ? (1.0f - Mathf.Sqrt(1 - Mathf.Pow(2.0f * x, 2.0f))) / 2.0f : (Mathf.Sqrt(1.0f - Mathf.Pow(-2.0f * x + 2.0f, 2.0f)) + 1.0f) / 2.0f;
                    break;

            }
            return y;
        }

        public void SetEase(Ease _ease)
        {
            curEaseMode = _ease;
        }


        //duration 시간동안 목표위치로 이동한다.
        public void DoMove(GameObject obj, Vector3 destpos, float duration)
        {
            Vector3 startpos = obj.transform.position;
            Vector3 directon = destpos - startpos;

            float timeval = 1 / duration;

            float speed = directon.magnitude / duration;

            CoroutineHandler.Start_Coroutine(CorDoMove(obj, destpos, duration));
        }

        public IEnumerator CorDoMove(GameObject obj, Vector3 dest, float duration)
        {
            float startTime = Time.time;
            float countVal = Time.deltaTime;
            //목표시간까지의 움직여야 될 횟수
            float maxCount = duration / Time.deltaTime;

            //0~1까지의 값을 만든다고 할때 1회마다 증가할 값
            float addval = 1 / maxCount;

            float curval = 0;
            float count = 0;

            Vector3 startpos = obj.transform.position;
            //목표까지의 방향과 거리
            Vector3 direction = dest - obj.transform.position;
            float distance = direction.magnitude;
            direction.Normalize();

            Vector3 start = obj.transform.position;

            while (true)
            {
                //지정한 시간이 되면 끝난다.
                if (count >= maxCount)
                {
                    yield break;
                }

                if (curval > 1)
                {
                    yield break;
                }

                obj.transform.position = startpos + (direction * getEaseVal(curval) * distance);
                curval += addval;
                count += countVal;

                yield return new WaitForSeconds(countVal);
            }

        }




    }
}
