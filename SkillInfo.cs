using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///스킬 공격에 대한 정보들
/////////////////////////////////////////////////////////////////////
///
[System.Serializable]
public class SkillInfo
{
    [Tooltip("해당 공격의 타입을 설정한다 (노말, 광역, 투사체, 타겟팅)")]
    public CharEnumTypes.eAttackType AttackType;

    [Tooltip("스킬이름")]
    public string SkillName;

    //스킬 애니메이션
    public AnimationClip aniclip;

    public string aniclipName;

    //스킬 애니메이션 재생속도
    public float animationPlaySpeed;

    [Tooltip("선딜")]
    [Range(0.0f, 10.0f)]
    public float StartDelay;

    //후딜레이
    [Tooltip("후딜")]
    [Range(0.0f, 10.0f)]
    public float RecoveryDelay;

    //데미지
    public float damage;

    //이펙트
    public GameObject Effect;

    public string EffectAdressable;

    //이펙트 생성 시간
    public float EffectStartTime;

    //이펙트 생성 위치
    public Transform EffectPosRot;

    //움직일 거리
    public float Movedis;

    //움직일 시간
    public float MoveTime;

    //움직임 시작 시간
    public float MoveStartTime;

    //공격 끝 시간
    public float AttackEndTime;

    [Tooltip("투사체가 있는 공격일때 투사체의 게임 오브젝트")]
    [SerializeField]
    public string ProjectileObjName;

    [Tooltip("타겟팅공격일때 타겟오브젝트")]
    [SerializeField]
    public string TargetObjName;

    [Tooltip("타겟팅공격일때 타겟오브젝트")]
    [SerializeField]
    public string AreaObjName;


    public float AreaObjLifeTime;
}

