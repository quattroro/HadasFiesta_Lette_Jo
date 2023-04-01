using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///캐릭터의 공격을 담당하는 클래스
///기본공격은 여러개의 콤보공격을 수행할 수 있도록 설계
///스킬공격도 같이 해당 클래스에서 처리한다.
/////////////////////////////////////////////////////////////////////

public class CAttackComponent : BaseComponent
{
    CurState curval;
    public int curAttackNum = 0;

    //공격을 하면서 앞으로 나가는 경우가 있기 때문에 moveCom을 가지고 있을 필요가 있다.
    [HideInInspector]
    public CMoveComponent moveCom;

    //무기 콜라이더
    public WeaponCollider weaponcollider;

    //몬스터의 태그
    public string monstertag;

    //타이머
    [HideInInspector]
    public CorTimeCounter timer = new CorTimeCounter();

    //공격이 다음 동작으로 이어질 수 있는지
    public bool IsLinkable = false;

    //선 입력된 다음 공격 정보
    public AttackInfo NextAttackInfo = null;

    //선 입력된 공격 정보가 있는지
    [HideInInspector]
    public bool NextAttack = false;

    //다음 공격정보의 인덱스
    [HideInInspector]
    public int NextAttackNum = -1;

    //기본 공격 정보들
    public List<AttackInfo> attackInfoList;
    //스킬 공격 정보들
    public List<SkillInfo> skillInfoList;


    //스킬 공격 범위 표시
    public GameObject SkillPreviewPlane = null;

    //애니메이션 관련
    [HideInInspector]
    public AnimationController animator;
    [HideInInspector]
    public AnimationEventSystem eventsystem;
    [HideInInspector]
    public GameObject effectobj;
    [HideInInspector]
    public float lastAttackTime = 0;


    [HideInInspector]
    public IEnumerator Effectcoroutine;
    [HideInInspector]
    public IEnumerator Linkcoroutine;
    [HideInInspector]
    public IEnumerator Movecoroutine;

    [System.Serializable]
    public class AreaInfo
    {
        public GameObject AreaObj;
        public string AreaObjName;
        public float DamageLoopTime;
        public float DestroyTime;
    }


    void Start()
    {

        animator = GetComponentInChildren<AnimationController>();
        eventsystem = GetComponentInChildren<AnimationEventSystem>();

        weaponcollider = GetComponentInChildren<WeaponCollider>();
        weaponcollider?.SetCollitionFunction(null, null, MonsterAttack);

        Initsetting();
        AnimationEventsSetting();
        
    }

    void AnimationEventsSetting()
    {
        //초기화 할때 각각의 공격 애니메이션의 이벤트들과 실행시킬 함수를 연결시켜 준다.
        for (int i = 0; i < attackInfoList.Count; i++)
        {


            if (attackInfoList[i].AttackEndTime != 0)
            {
                eventsystem.AddEvent(new KeyValuePair<string, AnimationEventSystem.beginCallback>(attackInfoList[i].aniclipName, AttackMove), attackInfoList[i].movestarttime,
                    new KeyValuePair<string, AnimationEventSystem.midCallback>(attackInfoList[i].aniclipName, AttackEnd), attackInfoList[i].AttackEndTime,
                    new KeyValuePair<string, AnimationEventSystem.endCallback>(attackInfoList[i].aniclipName, IsAttackingEnd), animator.GetClipLength(attackInfoList[i].aniclipName));
            }

        }

        //초기화 할때 각각의 스킬 애니메이션의 이벤트들과 실행시킬 함수를 연결시켜 준다.
        for (int i = 0; i < skillInfoList.Count; i++)
        {

            eventsystem.AddEvent(new KeyValuePair<string, AnimationEventSystem.beginCallback>(skillInfoList[i].aniclipName, AttackMove), skillInfoList[i].MoveStartTime,
                   new KeyValuePair<string, AnimationEventSystem.midCallback>(skillInfoList[i].aniclipName, SkillAttackEnd), skillInfoList[i].AttackEndTime,
                   new KeyValuePair<string, AnimationEventSystem.endCallback>(skillInfoList[i].aniclipName, IsAttackingEnd), animator.GetClipLength(skillInfoList[i].aniclipName));

        }
    }

    void Initsetting()
    {
        NextAttackInfo = null;

        attackInfoList = Resources.Load<AttackInfos>("ScriptableObject/CharacterAttackInfoList").attackInfoList;
        skillInfoList = Resources.Load<SkillInfos>("ScriptableObject/CharacterSkillInfoList").skillInfoList;


        //기본공격 정보 받아옴
        //LoadFile.Read<AttackInfo>(out LoadedAttackInfoDic);
    }


    //몬스터가 중복으로 공격되는것을 막기위함
    List<int> LastMonsterIDList = new List<int>();


    public void AttackMidFunc(string val)
    {

    }

    public void MonsterAttack(Collider collision)
    {
        if (!curval.IsAttacking)
            return;

        if (collision.gameObject.tag == monstertag)
        {
            int nowMonsterID = collision.gameObject.GetInstanceID();
            
            if(LastMonsterIDList.Contains(nowMonsterID))
                return;

            LastMonsterIDList.Add(nowMonsterID);

            if(collision.GetComponent<Battle_Character>()!=null)
                collision.GetComponent<Battle_Character>().Damaged((int)attackInfoList[curAttackNum].damage, this.transform.position);
            
        }

        if (collision.gameObject.tag == "Box")
        {
            collision.GetComponent<Item_Box>().Ending();
        }

    }

    public bool IsPrevAreaOfEffect = false;
    public SkillInfo PrevAreaOfEffectInfo = null;

    public GameObject testGameObject;
    public LayerMask testGroundlayer;

    //스킬을 재생해준다.
    public void SkillAttack(SkillInfo skillinfo)
    {
        if (moveCom == null)
        {
            moveCom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
            curval = moveCom.curval;
        }

        if (PlayableCharacter.Instance.GetState() == PlayableCharacter.States.OutOfControl)
            return;

        //이미 공격중일떄는 스킬 사용이 불가능
        if (curval.IsAttacking)
            return;

        //공격중으로 바꿈
        if (curval.IsAttacking == false)
        {
            moveCom.Stop();
            curval.IsAttacking = true;
        }


        if(skillinfo.AttackType == CharEnumTypes.eAttackType.AreaOfEffect)
        {
            if(IsPrevAreaOfEffect == false)
            {
                Debug.Log("장판스킬");
                //PlayableCharacter.Instance.SetState(PlayableCharacter.States.AreaOfEffect);
                PrevAreaOfEffectInfo = skillinfo;
                IsAttackingEnd("");
                IsPrevAreaOfEffect = true;
            }
            else
            {
                //장판 소환

                //ResourceCreateDeleteManager.Instance.InstantiateObj<GameObject>(skillinfo.AreaObjName);

                animator.Play(skillinfo.aniclip.name, skillinfo.animationPlaySpeed);

                Effectcoroutine = timer.Cor_TimeCounter<string, Vector3, float>
                    (animator.GetClipLength(skillinfo.aniclip.name), CreateEffect, skillinfo.AreaObjName, SkillPreviewPlane.transform.position, skillinfo.AreaObjLifeTime);
                StartCoroutine(Effectcoroutine);

                //SkillPreviewPlane
                //IsAttackingEnd("");
                IsPrevAreaOfEffect = false;
            }
            
        }
        else if(skillinfo.AttackType == CharEnumTypes.eAttackType.Projectile)
        {
            //투사체 소환

        }
        else
        {
            Debug.Log("일반스킬");
            if (skillinfo.Effect != null)
            {
                Effectcoroutine = timer.Cor_TimeCounter<string, Transform, float>
                    (skillinfo.EffectStartTime, CreateEffect, skillinfo.EffectAdressable, skillinfo.EffectPosRot, 1.5f);
                StartCoroutine(Effectcoroutine);
            }

            //EffectManager.Instance.SpawnEffectLooping(skillInfoList[skillnum].Effect, this.transform.position, Quaternion.identity, 2, 10);

            animator.Play(skillinfo.aniclip.name, skillinfo.animationPlaySpeed);
        }


        
    }

    public void NormalAttack(AttackInfo attackinfo)
    {

    }

    //공격 함수
    public void Attack()
    {
        if(IsPrevAreaOfEffect)
        {
            SkillAttack(PrevAreaOfEffectInfo);
            return;
        }

        //필요한 컴포넌트를 받아온다.
        if (moveCom == null)
        {
            moveCom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
            curval = moveCom.curval;
        }

        
        if (PlayableCharacter.Instance.GetState() == PlayableCharacter.States.OutOfControl)
            return;

        //스테미나가 다 떨어졌으면 공격을 못한다.
        if (PlayableCharacter.Instance.status.CurStamina <= 0)
            return;


        //이미 공격 중이고 링크가 불가능하면 공격이 실행되지 않는다.
        if (curval.IsAttacking && !IsLinkable)
        {
            int a = 0;
            //Debug.Log("[Attack]공격나가버림");
            return;
        }



        //이미 공격중이고 링크가 가능하면 다음 공격정보가 있는지 확인한다.
        //이런식으로하면 실제로 다음공격이 실행될때 여기서 걸려버린다. -> 링크 공격이 실행되는 타이밍을 조절하는것으로 해결
        if (curval.IsAttacking && IsLinkable && /*NextAttackNum == -1*/NextAttack == false)
        {
            //Debug.Log("[Attack]선입력들어옴");

            NextAttackNum = (curAttackNum + 1) % attackInfoList.Count;
            NextAttackInfo = attackInfoList[NextAttackNum];

            NextAttack = true;
            return;
        }

        //아직 공격중 이고 선입력이 들어왔는데 또 공격이 들어오면 리턴한다.
        if (curval.IsAttacking && NextAttack == true)
        {
            //if (NextAttackNum == (CurAttackNum + 1) % attackInfoList.Length)
            //    return;

            if (NextAttackNum == (curAttackNum + 1) % attackInfoList.Count)
                return;
        }


        //여기에서 진짜 공격 시작
        CInputComponent input = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.InputCom) as CInputComponent;
        input.GetWASD();
        moveCom.LookAtBody2();

        //공격중으로 바꿈
        if (curval.IsAttacking == false)
        {
            moveCom.Stop();
            curval.IsAttacking = true;
        }

        //이전에 공격했던 시간과 현재 공격이 시작된 시간의 차이를 구한다.
        float tempval = Time.time - lastAttackTime;

        //선입력정보가 없으면 링크가능한지 판단해서 동작을 해준다.
        if (/*NextAttackNum == -1*/NextAttack == false)
        {
            //다음 동작으로 넘어가기 위한
            if (IsLinkable)
            {
                //Debug.Log("링크가능");

                curAttackNum = (curAttackNum + 1) % attackInfoList.Count;
            }
            else
            {
                curAttackNum = 0;
            }
        }
        else//선입력정보가 있으면 해당 공격을 해주고 넘버를 올려준다.
        {
            curAttackNum = NextAttackInfo.attackNum;
        }

        //선입력세팅을 위한 입력은 앞에서 리턴하기 때문에 여기서는 관련 변수들을 초기화 해준다.
        NextAttackInfo = null;
        NextAttackNum = -1;
        NextAttack = false;

        //링크 가능 시간 체크
        Linkcoroutine = timer.Cor_TimeCounter(attackInfoList[curAttackNum].bufferdInputTime_Start, ActiveLinkable);
        StartCoroutine(Linkcoroutine);


        //애니메이션 실행
        animator.Play(attackInfoList[curAttackNum].aniclipName, attackInfoList[curAttackNum].animationPlaySpeed/*,0,attackInfoList[CurAttackNum].StartDelay*/);

        if (attackInfoList[curAttackNum].EffectName != null)
        {
            Effectcoroutine = timer.Cor_TimeCounter<string, Transform, float>
                (attackInfoList[curAttackNum].EffectStartTime, CreateEffect, attackInfoList[curAttackNum].EffectName, attackInfoList[curAttackNum].effectPosRot, 1.5f);
            StartCoroutine(Effectcoroutine);
        }


        //스테미나를 줄여준다.
        PlayableCharacter.Instance.status.StaminaDown(attackInfoList[curAttackNum].StaminaGaugeDown);

    }


    public void ActiveLinkable()
    {
        IsLinkable = true;
        Linkcoroutine = null;
    }

    public void DeActiveLinkable()
    {
        IsLinkable = false;
        Linkcoroutine = null;
    }

    //공격중 움직임이 필요할때 애니메이션의 이벤트를 이용해서 호출됨
    public void AttackMove(string clipname)
    {

        Debug.Log($"공격 움직임 {clipname}");
        for (int i = 0; i < attackInfoList.Count; i++)
        {
            if (attackInfoList[i].aniclipName == clipname)
            {
                moveCom?.FowardDoMove(attackInfoList[i].movedis, attackInfoList[i].movetime);
                return;
            }
        }

        for (int i = 0; i < skillInfoList.Count; i++)
        {
            if (skillInfoList[i].aniclip.name == clipname)
            {
                moveCom?.FowardDoMove(skillInfoList[i].Movedis, skillInfoList[i].MoveTime);
                return;
            }
        }

    }

    //일정 시간 뒤에 일정 시간 동안 일정 거리를 캐릭터의 정면 방향으로 이동할 때 사용
    public void AttackMoveAtTime(float time, float distance, float duration)
    {
        if (Movecoroutine != null)
            StopCoroutine(Movecoroutine);

        Movecoroutine = timer.Cor_TimeCounter<float, float>(time, AttackMoveAtTime, distance, duration);
        StartCoroutine(Movecoroutine);
    }

    public void AttackMoveAtTime(float distance, float duration)
    {
        moveCom.FowardDoMove(distance, duration);
    }

    //공격 이펙트를 생성
    public void CreateEffect(string adressableAdress, Transform posrot, float destroyTime)
    {
        effectobj = EffectManager.Instance.InstantiateEffect(adressableAdress, destroyTime);
        

        effectobj.transform.position = posrot.position;
        effectobj.transform.rotation = posrot.rotation;
        effectobj.transform.localScale = posrot.localScale;

        effectobj.transform.parent = posrot;

        ColliderSpawnManager.Instance.SpawnSphereCollider(effectobj.transform, 10, 5, monstertag, MonsterAttack);
    }

    //공격 이펙트를 생성
    public void CreateEffect(string adressableAdress, Vector3 pos, float destroyTime)
    {
        effectobj = EffectManager.Instance.InstantiateEffect(adressableAdress, destroyTime);

        effectobj.transform.position = pos;

        ColliderSpawnManager.Instance.SpawnSphereCollider(effectobj.transform, 10, 5, monstertag, MonsterAttack);
    }



    //공격애니메이션이 끝나면 해당 함수가 들어온다 공격 애니메이션의 이벤트를 통해 호출됨
    public void AttackEnd(string s_val)
    {
        //Debug.Log("공격 끝남");

        if (effectobj != null)
        {
            effectobj.transform.parent = null;
        }

        //공격이 끝난 후 일정 시간 동안 입력을 넣음으로써 연결 동작 실행 가능
        if (!IsLinkable)
        {
            ActiveLinkable();
        }

        if (Linkcoroutine != null)
            StopCoroutine(Linkcoroutine);

        //공격 끝 이후 연결동작 입력
        Linkcoroutine = timer.Cor_TimeCounter(attackInfoList[curAttackNum].BufferdInputTime_End, DeActiveLinkable);
        StartCoroutine(Linkcoroutine);


        //후딜레이 구현
        animator.Pause();
        StartCoroutine(timer.Cor_TimeCounter(attackInfoList[curAttackNum].recoveryDelay, ChangeState));
        

    }


    //공격애니메이션이 끝나면 해당 함수가 들어온다 공격 애니메이션의 이벤트를 통해 호출됨
    public void SkillAttackEnd(string s_val)
    {
        //Debug.Log("공격 끝남");

        if (effectobj != null)
        {
            effectobj.transform.parent = null;
        }

        //공격이 끝난 후 일정 시간 동안 입력을 넣음으로써 연결 동작 실행 가능
        //if (!IsLinkable)
        //{
        //    ActiveLinkable();
        //}

        //if (Linkcoroutine != null)
        //    StopCoroutine(Linkcoroutine);

        ////공격 끝 이후 연결동작 입력
        //Linkcoroutine = timer.Cor_TimeCounter(attackInfoList[CurAttackNum].BufferdInputTime_End, DeActiveLinkable);
        //StartCoroutine(Linkcoroutine);


        //후딜레이 구현
        animator.Pause();
        StartCoroutine(timer.Cor_TimeCounter(attackInfoList[curAttackNum].recoveryDelay, ChangeState));


    }


    public void ChangeState()
    {
        lastAttackTime = Time.time;

        //마지막으로 공격을 끝내고 돌아가는데 다음 연결 동작정보가 있으면 연결동작을 실행하고 
        //링크 공격을 할때 방향키를 입력하고 있으면 해당 방향으로 공격할 수 있도록 하자
        if (/*NextAttackNum != -1*/NextAttack == true)
        {
            //Debug.Log("[Attack]attackend");
            curval.IsAttacking = false;
            //LastMonsterID = -1;
            LastMonsterIDList.Clear();
            Attack();
        }
        //없으면 현재 실행중인 애니메이션의 마지막에 
        else
        {
            animator.Resume();
            AttackMoveAtTime(attackInfoList[curAttackNum].endmovestarttime, attackInfoList[curAttackNum].endmovedis, attackInfoList[curAttackNum].endmovetime);
        }

        //없으면 복귀동작을 실행한다.
        //else
        //{
        //    animator.Play(attackInfoList[CurAttackNum].endAniclipName, attackInfoList[CurAttackNum].endanimationPlaySpeed, 0, 0.2f, IsAttackingEnd);
        //}

    }

    public void IsAttackingEnd(string val)
    {
        Debug.Log("[Attack] 공격 진짜 마지막 끝");
        //Debug.Log("[Attack]attackend");
        curval.IsAttacking = false;
        LastMonsterIDList.Clear();
    }

    
    public void Damaged_Attacking(float damage, Vector3 hitpoint, float Groggy)
    {
        //넉백이나 넉 다운이 일어나지 않으면 공격이 끊기지 않도록
        float nextGroggy = PlayableCharacter.Instance.status.CurGroggy + Groggy;
        if(nextGroggy>=PlayableCharacter.Instance.status.player_Groggy||Groggy>=PlayableCharacter.Instance.status.player_Stagger_Groggy)
            AttackCutOff();

        PlayableCharacter.Instance.Damaged(damage, hitpoint, Groggy);
    }


    //공격이 중간에 끊겨야 할때
    public void AttackCutOff()
    {
        curval.IsAttacking = false;
        if (Effectcoroutine != null)
        {
            StopCoroutine(Effectcoroutine);
            Effectcoroutine = null;
        }
        if(Linkcoroutine!=null)
        {
            StopCoroutine(Linkcoroutine);
            Linkcoroutine = null;
            DeActiveLinkable();
        }
        if(Movecoroutine!=null)
        {
            StopCoroutine(Movecoroutine);
            Movecoroutine = null;
        }

        curval.IsAttacking = false;

        NextAttack = false;
        NextAttackNum = -1;
    }


    public override void InitComtype()
    {
        p_comtype = CharEnumTypes.eComponentTypes.AttackCom;
    }


    private void Update()
    {
        if(IsPrevAreaOfEffect)
        {
            Ray ray = PlayableCharacter.Instance.GetCamera().ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Physics.Raycast(ray,out hit, 100, testGroundlayer);


            if (hit.collider != null)
            {
                if (SkillPreviewPlane == null)
                    //GameMG.Instance.Resource.Instantiate<GameObject>("SkillPreviewPlane");
                SkillPreviewPlane = ResourceCreateDeleteManager.Instance.InstantiateObj<GameObject>("SkillPreviewPlane");

                SkillPreviewPlane.transform.position = hit.point + new Vector3(0, 0.2f, 0);

                //해당 상태에서 마우스 좌클릭을 누르면 
                if(Input.GetMouseButtonDown(0))
                {
                    //해당 위치에 이펙트 생성
                    //GameMG.Instance.Resource.Instantiate<GameObject>("SkillPreviewPlane");
                }


            }

        }
        else
        {
            if (SkillPreviewPlane != null)
            {
                ResourceCreateDeleteManager.Instance.DestroyObj<GameObject>("SkillPreviewPlane", SkillPreviewPlane);
                SkillPreviewPlane = null;
            }
                
            
        }


    }
}
