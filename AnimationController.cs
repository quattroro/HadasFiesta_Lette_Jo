using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///애니메이션을 Unity에서 제공해주는 AnimatorController를 사용하지 않고
///직접 Animator.CrossFade() 함수를 이용해서 제어하기 때문에
///다양한 기능들을 제공하기 위한 클래스가 필요해짐
///재생속도, 재생시간, 블렌딩속도 등등을 제어할수 있고
///현재 Animator에 올라와 있는 모든 클립들의 정보를 얻을 수 있고,
///애니메이션이 끝나고 자동으로 지정해준 함수가 호출될 수 도 있게 만들었습니다.
/////////////////////////////////////////////////////////////////////

/*사용할 모든 클립들이 등록되어있는 animation controller 가 연결된 animator 가 필요
  GetAnimationClips()를 이용하여 현재 등록된 클립들을 받아오든지, 직접 입력하던지 해서 재생을 원하는 클립의 이름을 알아와서
  Play(클립이름, 재생속도, 재생시간, 블렌딩속도) 함수를 이용해 재생*/

public class AnimationController : MonoBehaviour
{
    [Header("확인용")]
    public Animator animator;
    public int m_clipsnum;
    //public AnimationClip[] m_clips;
    public string currentplayclipname;
    public string lastplayclipname;

    public float prespeed;
    public float currentSpeed;
    public float currnetPlayTime;
    public float currentBlending;
    public delegate void Invoker();

    public RuntimeAnimatorController perAnimator;

    public Dictionary<string, AnimationClip> m_clips = new Dictionary<string, AnimationClip>();
    public AnimationClip[] m_tempclips;
    public List<AnimationTransition> Transitions = new List<AnimationTransition>();

    // 어드레서블의 Label을 얻어올 수 있는 필드.
    public AssetLabelReference TransitionLable;
    public IList<IResourceLocation> _locations;

    private CorTimeCounter timer = new CorTimeCounter();

    private void Awake()
    {
        if (!TryGetComponent<Animator>(out animator))
        {
            animator = GetComponentInChildren<Animator>();
            if (animator == null)
            {
                //Debug.Log($"{gameObject.name} animator component 없음!");
            }
        }
        m_tempclips = animator.runtimeAnimatorController.animationClips;
        
        //AnimationClip[] temp = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < m_tempclips.Length; i++)
        {
            m_clips.Add(m_tempclips[i].name, m_tempclips[i]);
        }


        LinkTransition();
    }

    public void LinkTransition()
    {
        var ret = Addressables.LoadResourceLocationsAsync(TransitionLable.labelString);
        _locations = ret.WaitForCompletion();

        for (int i=0;i<_locations.Count;i++)
        {
            var temp = Addressables.LoadAssetAsync<AnimationTransition>(_locations[i]);
            AnimationTransition result = temp.WaitForCompletion();
            Transitions.Add(result);
        }
    }

    public bool flag = false;
    public AnimatorTransitionInfo sss;
    public void Play(RuntimeAnimatorController aniGroup)
    {
        flag = true;
        perAnimator = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = aniGroup;
        //StateMachineBehaviour machine;
        //StateMachine mm;
        //AnimatorState sss;
        //AnimatorStateTransition[] transitions = sss.transitions;
        //sss.AddTransition()
    }


    private void Update()
    {
        if(flag)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("end"))
            {
                Debug.Log("끝들어옴");
                animator.runtimeAnimatorController = perAnimator;
            }

        }
    }


    //클립이름, 재생속도 (기본이 1배속), 재생 시간 (재생시간이 0이면 계속 반복), 블렌딩 시간(다음 동작으로 넘어가는데 걸릴 시간) 
    public void Play(string pname, float PlaySpeed = 1.0f, float PlayTime = 0, float blendingtime = 0.2f, CorTimeCounter.Invoker invoker = null)
    {
        //이미 재생중인 클입을 다시 재생 시키려면 Replay를 호출한다.
        if (pname == currentplayclipname)
        {
            return;
        }

        //애니메이션을 교체해야 하는데 이전 애니메이션에 연결정보가 있을때

        //AnimationTransition transition = Transitions.Find(x => x.Clip_1.name == currentplayclipname);
        //if (transition != null)
        //{
        //    float time = m_clips[pname].length;
        //    time = time / PlaySpeed;
        //    time = time * transition.exitTime;

        //    StartCoroutine(timer.Cor_TimeCounter<AnimationTransition>(time, Play, transition));
        //}


        if (PlayTime!=0)
        {
            StartCoroutine(timer.Cor_TimeCounter(PlayTime, Stop));
        }

        if(invoker!=null)
        {
            float time = m_clips[pname].length;
            time = time / PlaySpeed;
            StartCoroutine(timer.Cor_TimeCounter(time, invoker));
        }

        currentBlending = blendingtime;

        SetPlaySpeed(PlaySpeed);

        lastplayclipname = currentplayclipname;
        currentplayclipname = pname;

        animator.CrossFade(pname, blendingtime);

        //AnimationTransition transition = Transitions.Find(x => x.Clip_1.name == currentplayclipname);
        //if (transition != null)
        //{
        //    float time = m_clips[pname].length;
        //    time = time / PlaySpeed;
        //    time = time * transition.exitTime;

        //    StartCoroutine(timer.Cor_TimeCounter<AnimationTransition>(time, Play, transition));
        //}
    }


    private void Play(string clipname, float transitionDuration, int layer, float _normalizedTimeOffset)
    {
        
    }

    private void Play(AnimationTransition.TransitionInfo _transition)
    {
        animator.CrossFade(_transition.TransClip.name, _transition.normalizedTransitionDuration, 0, _transition.normalizedTimeOffset);
    }

    private void Play(AnimationTransition animationTransition)
    {
        //시작

        //중간
        if(animationTransition.Clip.isLooping)
        {
            //animator.is
        }
        else
        {

        }

        //끝
    }

    //클립이름, 재생속도 (기본이 1배속), 재생 시간 (재생시간이 0이면 계속 반복), 블렌딩 시간(다음 동작으로 넘어가는데 걸릴 시간) 
    public void Play<T>(string pname, CorTimeCounter.TInvoker<T> invoker, T val, float PlaySpeed = 1.0f, float PlayTime = 0, float blendingtime = 0.2f)
    {
        //이미 재생중인 클입을 다시 재생 시키려면 Replay를 호출한다.
        if (pname == currentplayclipname)
        {
            return;
        }

        //새로운 애니메이션으로 바꿔야 하는데 이전에 실행중 이였던 애니메이션에 엔딩동작이 있으면
        //AnimationTransition transition = Transitions.Find(x => x.Clip.name == currentplayclipname);
        //if (transition != null && transition.endTransition != null)
        //{
        //    float time = m_clips[pname].length;
        //    time = time / PlaySpeed;
        //    time = time * transition.endTransition.exitTime;

        //    Play(transition.endTransition);

        //    AnimationTransition.TransitionInfo info = new AnimationTransition.TransitionInfo(transition.endTransition);
        //    info.TransClip = m_clips[currentplayclipname];

        //    StartCoroutine(timer.Cor_TimeCounter<AnimationTransition.TransitionInfo>(time, Play, info));
        //}

        if (PlayTime != 0)
        {
            StartCoroutine(timer.Cor_TimeCounter(PlayTime, Stop));
        }

        if (invoker != null)
        {
            float time = m_clips[pname].length;
            time = time / PlaySpeed;
            //time += 1.0f;
            StartCoroutine(timer.Cor_TimeCounter<T>(time, invoker, val));
        }

        currentBlending = blendingtime;

        SetPlaySpeed(PlaySpeed);

        lastplayclipname = currentplayclipname;
        currentplayclipname = pname;


        animator.CrossFade(pname, blendingtime);

        ////시작동작이 있는지 확인하고 있으면 시작동작을 먼저 해준다.
        //AnimationTransition transition = Transitions.Find(x => x.Clip.name == currentplayclipname);
        //if (transition != null && transition.startTransition != null)
        //{
        //    float time = m_clips[pname].length;
        //    time = time / PlaySpeed;
        //    time = time * transition.startTransition.exitTime;

        //    Play(transition.startTransition);

        //    AnimationTransition.TransitionInfo info = new AnimationTransition.TransitionInfo(transition.startTransition);
        //    info.TransClip = m_clips[currentplayclipname];

        //    StartCoroutine(timer.Cor_TimeCounter<AnimationTransition.TransitionInfo>(time, Play, info));
        //}
        //else
        //{
        //    animator.CrossFade(pname, blendingtime);
        //}
        
    }


    public void RePlay()
    {
        animator.CrossFade(currentplayclipname, currentBlending);
    }

    //선택한 클립의 총 길이를 알려준다.
    public float GetClipLength(string pname)
    {
        float time = 0;

        time = m_clips[pname].length;

        return time;
    }

    //현재 애니메이터에 설정되어 있는 클립들의 배열을 받아온다.
    public Dictionary<string, AnimationClip> GetAnimationClips()
    {
        return m_clips;
    }

    //재생속도를 설정한다.
    public void SetPlaySpeed(float PlaySpeed)
    {
        if (animator.speed != PlaySpeed)
            animator.speed = PlaySpeed;
    }

    //현재 애니메이션이 재생되고 있는 속도를 받아온다.
    public float GetPlaySpeed()
    {
        return animator.speed;
    }

    //재생 정지
    public void Stop()
    {
        animator.StopPlayback();
    }

    //재생 일시정지
    public void Pause()
    {
        prespeed = animator.speed;
        animator.speed = 0;
        //animator.CrossFade()
    }

    //다시 재생
    public void Resume()
    {
        if(prespeed!=0)
            animator.speed = prespeed;
        else
            animator.speed = 1.0f;

        prespeed = 0;
    }


    //현재 재생중인 클립인지 확인한다.
    public bool IsNowPlaying(string pname)
    {
        return (currentplayclipname == pname);
    }


    public void SetInteger(string pname,int val)
    {
        animator.SetInteger(pname, val);
    }

    public void SetBool(string pname, bool val)
    {
        animator.SetBool(pname, val);
    }

    public void SetFloat(string pname, float val)
    {
        animator.SetFloat(pname, val);
    }

    public void SetTrigger(string pname)
    {
        animator.SetTrigger(pname);
    }
}
