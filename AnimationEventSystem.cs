using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///애니메이션 이벤트들을 관리
///각 이벤트들의 대리자에 실행하고자 하는 함수를 AddEvent(beginCallback begin, midCallback mid, endCallback end) 함수를 이용해
/// 연결시켜주면 자동으로 클립들에 이벤트를 생성 해주고 연결해준다. 그러면 해당 애니메이션이 실행되고 해당 이벤트가 실행되면 해당 함수가 실행됨
/////////////////////////////////////////////////////////////////////


public class AnimationEventSystem : MonoBehaviour
{
	AnimationController animator;

	public Dictionary<string, AnimationClip> clips;
	//public AnimationEvent[][] eventlist;
	public List<AnimationEvent[]> eventlist = new List<AnimationEvent[]>();

	public delegate void beginCallback(string s_val);
	public delegate void midCallback(string s_val);
	public delegate void endCallback(string s_val);

    public Dictionary<string, beginCallback> BeginEventInvokers = new Dictionary<string, beginCallback>();
	public Dictionary<string, midCallback> MidEventInvokers = new Dictionary<string, midCallback>();
	public Dictionary<string, endCallback> EndEventInvokers = new Dictionary<string, endCallback>();

    public beginCallback _beginCallback;
	public midCallback _midCallback;
	public endCallback _endCallback;

    public delegate void beginCallbackT<T>(T s_val);
    public delegate void midCallbackT<T>(T s_val);
    public delegate void endCallbackT<T>(T s_val);

    public delegate void CallBackEvent(string s_val);
    public CallBackEvent _Callback;

    public Dictionary<string, List<CallBackEvent>>EventInvokers = new Dictionary<string, List<CallBackEvent>>();

    private void Start()
    {
		animator = GetComponent<AnimationController>();
		clips = animator.GetAnimationClips();
	}

    
    public void AddEvent(KeyValuePair<string, beginCallback> begin, KeyValuePair<string, midCallback> mid, KeyValuePair<string, endCallback> end)
    {
        if (begin.Key != null)
            BeginEventInvokers.Add(begin.Key, begin.Value);
        if (mid.Key != null)
            MidEventInvokers.Add(mid.Key, mid.Value);
        if (end.Key != null)
            EndEventInvokers.Add(end.Key, end.Value);
    }



    //각각의 애니메이션에 실행시킬 이벤트들을 넣어준다.
    public void AddEvent(KeyValuePair<string, beginCallback> begin,float begintime, KeyValuePair<string, midCallback> mid, float midtime, KeyValuePair<string, endCallback> end, float endtime)
    {
        AnimationEvent aevent;
        
        if(animator==null)
            animator = GetComponent<AnimationController>();

        if (begin.Key != null)
        {
            aevent = new AnimationEvent();
            aevent.time = begintime;
            aevent.functionName = "OnBeginEvent";
            aevent.stringParameter = begin.Key;
            if (!animator.m_clips.ContainsKey(begin.Key))
            {
                Debug.Log($"AnimationEventSystem 오류 키값이 존재하기 않음 {begin.Key}");
            }
            animator.m_clips[begin.Key].AddEvent(aevent);

            BeginEventInvokers.Add(begin.Key, begin.Value);
        }

        if (mid.Key != null)
        {
            aevent = new AnimationEvent();
            aevent.time = midtime;
            aevent.functionName = "OnMidEvent";
            aevent.stringParameter = mid.Key;

            if(!animator.m_clips.ContainsKey(mid.Key))
            {
                Debug.Log($"AnimationEventSystem 오류 키값이 존재하기 않음 {mid.Key}");
            }
            animator.m_clips[mid.Key].AddEvent(aevent);

            MidEventInvokers.Add(mid.Key, mid.Value);
        }
            
        if (end.Key != null)
        {
            aevent = new AnimationEvent();
            aevent.time = endtime;
            aevent.functionName = "OnEndEvent";
            aevent.stringParameter = end.Key;
            if (!animator.m_clips.ContainsKey(end.Key))
            {
                Debug.Log($"AnimationEventSystem 오류 키값이 존재하기 않음 {end.Key}");
            }
            animator.m_clips[end.Key].AddEvent(aevent);

            EndEventInvokers.Add(end.Key, end.Value);
        }
        
    }

    public struct AnimationEventInfo
    {
        string _clipName;
        AnimationEvent _event;
    }

   
    public void OnAnimationEvent(string s_val)
    {

    }

    public void OnBeginEvent(string s_val)
    {
        //_beginCallback?.Invoke(s_val);


        if (BeginEventInvokers.TryGetValue(s_val, out _beginCallback))
        {
            _beginCallback.Invoke(s_val);
            //_beginCallback.
        }
    }

    public void OnMidEvent(string s_val)
    {
        //_midCallback?.Invoke(s_val);
        if (MidEventInvokers.TryGetValue(s_val, out _midCallback))
        {
            _midCallback.Invoke(s_val);
        }
    }

    public void OnEndEvent(string s_val)
    {
        //_endCallback?.Invoke(s_val);
        if (EndEventInvokers.TryGetValue(s_val, out _endCallback))
        {
            _endCallback.Invoke(s_val);
        }
    }
}

