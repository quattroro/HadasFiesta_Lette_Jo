using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///공격 정보 클래스
///스크립터블 오브젝트를 이용해서 수치를 조정하고 
///필요시 csv로 내보내거나 csv를 읽어오는것도 가능하다
/////////////////////////////////////////////////////////////////////

[System.Serializable]
public class AttackInfo
{
    [Tooltip("공격번호")]
    public int attackNum;

    [Tooltip("공격이름")]
    public string attackName;

    [Tooltip("해당 공격의 타입을 설정한다 (노말, 광역, 투사체, 타겟팅)")]
    public CharEnumTypes.eAttackType attackType;

    //해당 매니메이션 클립 이후 공격컴포넌트에서 이름을 이용해 실제 클립을 받아오는것이 필요
    [Tooltip("해당 공격의 애니메이션 클립 이름")]
    public string aniclipName;

    //해당 매니메이션 클립 이후 공격컴포넌트에서 이름을 이용해 실제 클립을 받아오는것이 필요
    [Tooltip("해당 공격의 복귀 애니메이션 클립 이름")]
    public string endAniclipName;

    //공격 중 움직일 거리
    [Tooltip(" 복귀 애니메이션 움직임을 시작할 시간")]
    public float endmovestarttime;

    //공격 중 움직일 거리
    [Tooltip(" 복귀 애니메이션 움직일 거리")]
    public float endmovedis;

    //움직일 시간
    [Tooltip(" 복귀 애니메이션 움직일 시간")]
    public float endmovetime;

    //애니메이션 배속
    [Tooltip("해당 공격의 애니메이션 재생 속도")]
    [Range(0.0f, 10.0f)]
    public float animationPlaySpeed;

    [Tooltip("해당 공격의 복귀 애니메이션 클립 이름")]
    [Range(0.0f, 10.0f)]
    public float endanimationPlaySpeed;

    [Tooltip("선딜")]
    [Range(0.0f, 10.0f)]
    public float startDelay;

    //후딜레이
    [Tooltip("후딜")]
    [Range(0.0f, 10.0f)]
    public float recoveryDelay;

    [Tooltip("다음 동작으로 넘어갈 수 있는 시간")]
    public float bufferdInputTime_Start;

    //다음동작으로 넘어가기 위한 시간
    //해당동작이 끝나고 해당 시간 안에 Attack()함수가 호출되어야지 다음동작으로 넘어간다.
    [Tooltip("연속동작이 있을때 다음 동작으로 들어가기 위한 입력 시간")]
    public float BufferdInputTime_End;

    //데미지
    [Tooltip("공격 데미지")]
    public float damage;

    [Tooltip("공격시 줄어들 스테미나 게이지")]
    public float StaminaGaugeDown;

    //공격 이펙트
    [Tooltip("공격 이펙트")]
    public string EffectName;

    //이펙트 생성 타이밍
    [Tooltip("공격 이펙트 생성 타이밍")]
    public float EffectStartTime;

    //공격 이펙트의 위치
    [Tooltip("공격 이펙트 생성 위치")]
    public Transform effectPosRot;

    [Tooltip("공격 이펙트 파괴 시간")]
    public float EffectDestroyTime;

    //공격 중 움직일 거리
    [Tooltip("공격할때 움직임을 시작할 시간")]
    public float movestarttime;

    //공격 중 움직일 거리
    [Tooltip("공격할때 움직일 거리")]
    public float movedis;

    //움직일 시간
    [Tooltip("공격할때 움직일 시간")]
    public float movetime;

    [Tooltip("투사체가 있는 공격일때 투사체의 게임 오브젝트")]
    public string ProjectileObjName;

    [Tooltip("타겟팅공격일때 타겟오브젝트")]
    public string TargetObjName;

    [Tooltip("공격 끝 지점")]
    public float AttackEndTime;

    ///////////////////////////////////////////////////////////////////////////////////////

}

