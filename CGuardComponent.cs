using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///캐릭터의 방어를 담당하는 클래스
/////////////////////////////////////////////////////////////////////
///

public class CGuardComponent : BaseComponent
{
    public CMoveComponent movecom;
    public AnimationController animator;
    public AnimationEventSystem eventsystem;


    //가드에 대한 옵션 변수들
    [Header("============Guard Options============")]
    public float GuardTime;//최대로 가드를 할 수 있는 시간
    public float GuardStunTime;//가드 경직 시간
    //public int BalanceDecreaseVal;
    public AnimationClip GuardStunClip;
    public string GuardStunClipName;
    public AnimationClip GuardClip;
    public GameObject GuardEffect;
    public Transform guardeffectpos;

    public float GuardAngle;

    //내부적으로 동작에 사용되는 변수들
    [Header("============Cur Values============")]
    public int CurGuardGauge;
    public bool nowGuard;
    public float GaugeDownInterval;
    public IEnumerator guardcoroutine;
    public IEnumerator stuncoroutine;
    public delegate void Invoker();
    public bool nowGuardStun;
    
    public float hitangle;

    private CorTimeCounter timer = new CorTimeCounter();
    

    void Start()
    {
        animator = GetComponentInChildren<AnimationController>();
        eventsystem = GetComponentInChildren<AnimationEventSystem>();
    }


    //포커싱 상태에서 가드를 하면 시점이 캐릭터의 정면으로 고정 되어야 한다.
    public void Guard()
    {
        if (movecom == null)
            movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;

        if (movecom.curval.IsGuard)
            return;

        if(PlayableCharacter.Instance.IsFocusingOn)
            movecom.LookAtToLookDir();

        movecom.curval.IsGuard = true;

        movecom.com.animator.Play(GuardClip.name, 2.0f);

        //movecom.LookAtFoward();

        if (guardcoroutine != null)
        {
            StopCoroutine(guardcoroutine);
            guardcoroutine = null;
        }

        guardcoroutine = timer.Cor_TimeCounter(GuardTime, GuardDown);
        StartCoroutine(guardcoroutine);
    }

    //일정 시간 이후에 가드가 끝나야 할때
    public void GuardDown()
    {
        if (movecom == null)
            movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;

        if (!movecom.curval.IsGuard)
            return;

        if (guardcoroutine != null)
        {
            StopCoroutine(guardcoroutine);
            if(stuncoroutine!=null)
            {
                StopCoroutine(stuncoroutine);
                stuncoroutine = null;
            }
            guardcoroutine = null;
        }
            

        movecom.curval.IsGuard = false;
    }

    


    //가드중일때 데미지가 들어왔을때는 이쪽으로 들어온다.
    public void Damaged_Guard(float damage,Vector3 hitpoint,float Groggy)
    {
        //피격위치가 캐릭터 정면 일정 각도 안에 있을때만 가드 성공
        Vector3 front = movecom.com.FpRoot.forward;
        front.y = 0;
        front.Normalize();

        Vector3 hit = hitpoint.normalized;
        hit.y = 0;
        hit.Normalize();

        hitangle = 180 - Mathf.Acos(Vector3.Dot(front, hit)) * 180.0f / 3.14f;


        //스테미나에 따라서 가드 성공 실패 학인
        if (PlayableCharacter.Instance.status.CurStamina >= 10 /*&& hitangle <= GuardAngle*/ && !nowGuardStun) 
        {
            //가드 성공
            PlayableCharacter.Instance.status.StaminaDown(10);
            EffectManager.Instance.InstantiateEffect(GuardEffect, guardeffectpos.position, guardeffectpos.rotation);
            GuardStun();
        }
        else
        {
            //가드 실패
            PlayableCharacter.Instance.Damaged(damage, hitpoint, Groggy);
        }


    }



    //가드넉백상태는 GuardStun 상태로 넘어간다.
    public void GuardStun()
    {
        animator.Play(GuardStunClipName, 1.0f, 0.0f, 0.1f, StunEnd);
        stuncoroutine = timer.Cor_TimeCounter(animator.GetClipLength(GuardStunClipName), StunEnd);
        nowGuardStun = true;
    }

    //스턴 종료
    public void StunEnd()
    {
        nowGuardStun = false;
        movecom.com.animator.Play("Block_Loop", 2.0f);
        stuncoroutine = null; 
    }

    //초기화
    public override void InitComtype()
    {
        p_comtype = CharEnumTypes.eComponentTypes.GuardCom;
    }
}
