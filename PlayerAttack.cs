//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerAttack : BaseComponent
//{
//    [SerializeField]
//    CurState curval;

//    public float LastAttackTime;

//    public int AttackNum = 0;
//    public CMoveComponent movecom;

//    [System.Serializable]
//    public class AttackMovementInfo
//    {
//        public int AttackNum;

//        //애니메이션 배속
//        public float animationPlaySpeed;

//        //해당 매니메이션 클립
//        public AnimationClip aniclip;

//        //후딜레이
//        public float MovementDelay;

//        //다음동작으로 넘어가기 위한 시간
//        //해당동작이 끝나고 해당 시간 안에 Attack()함수가 호출되어야지 다음동작으로 넘어간다.
//        public float NextMovementTimeVal;

//        public float damage;

//        public float EffectStartTime;

//        public GameObject Effect;

//        public Transform EffectPosRot;

//        public float movedis;

//        public float movetime;
//    }

//    [SerializeField]
//    public List<PlayerAttack_Information> Attack_InformationList = new List<PlayerAttack_Information>();

//    [SerializeField]
//    public PlayerAttack_Information SkillData;

//    //스킬도 여기서 한번에 처리
//    [System.Serializable]
//    public class SkillInfo
//    {
//        public string SkillName;

//        public int SkillNum;

//        public AnimationClip aniclip;

//        public float animationPlaySpeed;

//        public float MovementDelay;

//        public float NextMovementTimeVal;

//        public float damage;

//        public GameObject Effect;

//        public float EffectStartTime;

//        public Transform EffectPosRot;

//        public float Movedis;

//        public float MoveTime;

//    }

//    [SerializeField]
//    public Collider[] colliders;

//    public AnimationController animator;

//    public AnimationEventSystem eventsystem;

//    public AttackMovementInfo[] attackinfos;

//    public SkillInfo[] skillinfos;

//    public GameObject effectobj;

//    public Transform preparent;

//    public float lastAttackTime = 0;

//    public delegate void Invoker();

//    public AttackManager testAttckmanager;

//    public Transform AttackColliderParent;

//    public AttackManager att;

//    public GameObject Player;

//    IEnumerator coroutine;

//    public Rigidbody rigidbody;
//    void Start()
//    {
//        rigidbody = GetComponent<Rigidbody>();
//        Player = AddressablesController.Instance.find_Asset_in_list("PlayerCharacter(Clone)");
//        //UIManager.Instance.Prefabsload("Inven", UIManager.CANVAS_NUM.player_cavas);
//        att = GetComponentInChildren<AttackManager>();
//        animator = GetComponentInChildren<AnimationController>();
//        eventsystem = GetComponentInChildren<AnimationEventSystem>();

//        for (int i = 0; i < Attack_InformationList.Count; i++)
//        {           
//            att.PlayerAddAttackInfo(Attack_InformationList[i].P_aniclip.name, Attack_InformationList[i].P_movedis, Attack_InformationList[i].P_movetime);
//        }
//        for (int i = 0; i < 1; i++)
//        {
//            att.PlayerAddAttackInfo(SkillData.P_aniclip.name, SkillData.P_movedis, SkillData.P_movetime);
//        }
//    }
//    void Update()
//    {
//        //if (Input.GetMouseButtonDown(0))
//        //{
//        //    Attack();
//        //    return;
//        //}

//        //if (Input.GetKeyDown(KeyCode.Alpha1))
//        //{
//        //    SkillAttack();
//        //    Debug.Log("정답");
//        //    return;
//        //}
//    }
//    public void PlayerHit()
//    {
//        curval.IsAttacking = false;

//        if (coroutine != null)
//        StopCoroutine(coroutine);
        
//    }
//    //공격이 시작된지 일정 시간 뒤에 이펙트를 실행해야 할 때 사용
//    IEnumerator Cor_TimeCounter(float time, Invoker invoker)
//    {
//        float starttime = Time.time;

//        while (true)
//        {
//            if ((Time.time - starttime) >= time)
//            {
//                invoker.Invoke();
//                yield break;
//            }
//            yield return new WaitForSeconds(Time.deltaTime);
//        }
//    }

//    private void OnTriggerEnter(Collider other)
//    {
        
//    }

//    //스킬을 재생해준다.
//    public void SkillAttack()
//    {
//        if (movecom == null)
//        {
//            movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
//            //testAttckmanager.AddComponent(movecom);
//            curval = movecom.curval;
//        }

//        if (curval.IsAttacking)
//            return;

        
//        if (curval.IsAttacking == false)
//            curval.IsAttacking = true;

//        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

//        coroutine = Cor_TimeCounter(SkillData.P_EffectStartTime, SkillCreateEffect);
//        StartCoroutine(coroutine);
//        att.ComboAttackMana(animator, SkillData.P_aniclip.name, SkillData.P_animationPlaySpeed);


//        //float tempMp = Player.GetComponent<PlayableCharacter>().CharacterInfoPanel.MPBar.GetCurValue();

//        //Player.GetComponent<PlayableCharacter>().CharacterInfoPanel.MPBar.SetCurValue(tempMp - 10);
//    }

//    public void CreateEffect()
//    {
//        //att.CreateEffect(SkillData.P_Effect, attackinfos[AttackNum].EffectPosRot, 1.5f, 10);
//        //att.CreateEffect(Attack_InformationList[AttackNum].P_Effect, attackinfos[AttackNum].EffectPosRot, 1.5f, 40);

//        //testAttckmanager.CreateEffect(Attack_InformationList[AttackNum].P_Effect, attackinfos[AttackNum].EffectPosRot, 1.5f , 10);
//        //preparent = testAttckmanager.CreateEffect(Attack_InformationList[AttackNum].P_Effect, attackinfos[AttackNum].EffectPosRot, 1.5f);
//        //preparent = testAttckmanager.CreateEffect(Attack_InformationList[AttackNum].P_Effect, Attack_InformationList[AttackNum].P_EffectPosRot, 1.5f);
//    }
//    public void SkillCreateEffect()
//    {
//        //att.CreateEffect(SkillData.P_Effect, attackinfos[AttackNum].EffectPosRot, 1.5f, 60);             
//    }
//    public void Attack()
//    {
//        if (curval.IsAttacking)
//            return;

//        if (movecom == null)
//        {
//            movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
//            //testAttckmanager.AddComponent(movecom);
//            curval = movecom.curval;
//        }

//        if (curval.IsAttacking == false)
//            curval.IsAttacking = true;

//        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

//        float tempval = Time.time - lastAttackTime;

//        if (tempval <= Attack_InformationList[AttackNum].P_NextMovementTimeVal)
//        {
//            AttackNum = (AttackNum + 1) % (int)CharEnumTypes.eAniAttack.AttackMax;

//        }
//        else
//        {
//            AttackNum = 0;
//        }

//        //StartCoroutine(Cor_TimeCounter(Attack_InformationList[AttackNum].P_EffectStartTime, CreateEffect));


//        coroutine = Cor_TimeCounter(Attack_InformationList[AttackNum].P_EffectStartTime, CreateEffect);
//        StartCoroutine(coroutine);

//        att.ComboAttackMana(animator, Attack_InformationList[AttackNum].P_aniclip.name, Attack_InformationList[AttackNum].P_animationPlaySpeed);
//        //testAttckmanager.ComboAttackMana(animator, Attack_InformationList[AttackNum].P_aniclip.name, Attack_InformationList[AttackNum].P_animationPlaySpeed);

//        rigidbody.velocity = Vector3.zero;
//    }

//    public void AttackTime(float time)
//    {
//        Debug.Log("아부앙" + time);
//        lastAttackTime = time;
//    }
    


//    public override void InitComtype()
//    {
//        p_comtype = CharEnumTypes.eComponentTypes.AttackCom;
//    }
//}
