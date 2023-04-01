using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
/////////////////////////////////////////////////////////////////////

//사용되지 않음

[CreateAssetMenu(fileName = "AnimationTransition", menuName = "Scriptable Object/AnimationTransition", order = int.MaxValue)]
public class AnimationTransition : ScriptableObject
{
    [System.Serializable]
    public class TransitionInfo
    {
        public TransitionInfo(AnimationClip _clip, float _exitTime, float _duration, float _offset)
        {
            TransClip = _clip;
            exitTime = _exitTime;
            normalizedTransitionDuration = _duration;
            normalizedTimeOffset = _offset;
        }

        public TransitionInfo(TransitionInfo _info)
        {
            TransClip = _info.TransClip;
            exitTime = _info.exitTime;
            normalizedTransitionDuration = _info.normalizedTransitionDuration;
            normalizedTimeOffset = _info.normalizedTimeOffset;
        }

        public AnimationClip TransClip;

        [Range(0.0f, 1.0f)]
        public float exitTime;//클립 1에서 클립2로 넘어가는 시점 0~1값

        public float normalizedTransitionDuration;//블렌딩 되는 기간

        [Range(0.0f, 1.0f)]
        public float normalizedTimeOffset;
    }

    [Header("주체가 되는 클립")]
    public AnimationClip Clip;

    public TransitionInfo startTransition;

    public TransitionInfo endTransition;

}
