using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///플레이어블 캐릭터의 모든것을 관리하는 클래스 입니다.
///하는일
///1. 컴포넌트들 관리, 기존 ComponentManager가 하던 일을 그대로 실행
///2. 플레이어 데이터를 받아와서 각각의 컴포넌트 들에게 각각 필요한 데이터들을 넘겨준다.
///3. 각종 초기화, 연결 작업들
///4. 외부에서 캐릭터의 변수에 접근이 필요할때 해당 클래스로바로 접근이 가능하도록
/////////////////////////////////////////////////////////////////////

public class PlayableCharacter : MonoBehaviour
{
    //캐릭터의 FSM이 가지는 캐릭터의 상태들
    public enum States
    {
        Idle,
        Walk,
        Run,
        Attack,
        Rolling,
        Guard,
        GuardStun,
        OutOfControl,
        AutoMove,
        AreaOfEffect,
    }


    [Header("================BaseComponent================")]
    public BaseComponent[] components = new BaseComponent[(int)CharEnumTypes.eComponentTypes.comMax];

    public BaseStatus status;

    [Header("================캐릭터 UI================")]
    public UICharacterInfoPanel CharacterUIPanel;

    [Header("================피격 이펙트================")]
    public GameObject HitEffect;
    public string HitEffectAdressableName;
    public EffectManager effectmanager;
    public AnimationClip DeadAnimation;

    [Header("================StateMachine================")]
    public MyStateMachine.StateMachine<States, MyStateMachine.Drive> fsm;
    public States curState;

    CMoveComponent movecom;
    AnimationController animator;

    /*싱글톤*/
    static PlayableCharacter _instance;
    public static PlayableCharacter Instance
    {
        get
        {
            return _instance;
        }
    }

    /*초기화*/
    private void Awake()
    {
        _instance = this;
    }

    public bool ComponentInit()
    {
        BaseComponent[] temp = GetComponentsInChildren<BaseComponent>();
        status = GetComponent<BaseStatus>();


        foreach (BaseComponent a in temp)
        {
            if (a.gameObject.activeSelf)
                components[(int)a.p_comtype] = a;
        }

        if (components[1] == null)
            return false;

        return true;
    }

    bool flag = false;

    /*초기화*/
    private void Start()
    {
        //필요한 매니저들이 존재하는지 확인하고 없으면 만들어준다.
        if (FindObjectOfType<ResourceCreateDeleteManager>() == null)
        {
            GameObject obj = new GameObject(typeof(ResourceCreateDeleteManager).Name);
            obj.AddComponent<ResourceCreateDeleteManager>();
        }

        if (FindObjectOfType<EffectManager>() == null)
        {
            GameObject obj = new GameObject(typeof(EffectManager).Name);
            obj.AddComponent<EffectManager>();
        }

        if (FindObjectOfType<ColliderSpawnManager>() == null)
        {
            GameObject obj = new GameObject(typeof(ColliderSpawnManager).Name);
            obj.AddComponent<ColliderSpawnManager>();
        }

        //FSM 생성 & 초기화
        fsm = new MyStateMachine.StateMachine<States, MyStateMachine.Drive>(this);
        SetState(States.Idle);

        //캐릭터에서 사용하는 컴포넌트들 초기화
        ComponentInit();
        movecom = GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
        animator = GetComponentInChildren<AnimationController>();


        //UI가 존재하지 않는 경우 UI객체를 로드해서 생성시켜 준다.
        if (CharacterUIPanel == null)
        {
            try
            {
                if (UIManager.Instance != null)
                    CharacterUIPanel = UIManager.Instance.Prefabsload(Global_Variable.CharVar.CharacterUI, Canvas_Enum.CANVAS_NUM.player_cavas).GetComponent<UICharacterInfoPanel>();
                else
                    CharacterUIPanel = GameMG.Instance.Resource.Instantiate<UICharacterInfoPanel>(Global_Variable.CharVar.CharacterUI);
            }
            catch (Exception e)
            {
                if (UIManager.Instance == null)
                    CharacterUIPanel = ResourceCreateDeleteManager.Instance.InstantiateObj<UICharacterInfoPanel>(Global_Variable.CharVar.CharacterUI);
                else
                    CharacterUIPanel = UIManager.Instance.Prefabsload(Global_Variable.CharVar.CharacterUI, Canvas_Enum.CANVAS_NUM.player_cavas).GetComponent<UICharacterInfoPanel>();
            }

        }


        //UI 연동 부분
        status.Init(CharacterUIPanel);
        CharacterUIPanel.transform.localPosition = status.player_UIPos;
        if(UIManager.Instance!=null)
        {
            GameObject tempui = UIManager.Instance.Canvasreturn(Canvas_Enum.CANVAS_NUM.start_canvas);
            MainOption mainoption = tempui.GetComponent<MainOption>();
            mainoption.r_invoker = SetReverseMouseRot;
            mainoption.a_invoker = SetCameraColl;
            mainoption.l_invoker = SetOutoFocus;
            mainoption.m_invoker = SetMouseSpeed;

            SetOutoFocus(mainoption.LooKon);
            SetMouseSpeed(mainoption.MouseSensetive);
            SetReverseMouseRot(mainoption.ReverseMouse);
            SetCameraColl(mainoption.AutoeVade);
        }
    }

    //업데이트
    private void Update()
    {
        curState = fsm.GetCurState();
    }

    //자동 이동
    public void AutoMove(Vector3 destpos, float moveTime, CMoveComponent.ActionInvoker invoker = null)
    {
        movecom.AutoMove(destpos, moveTime, invoker);
    }

    //duration 시간동안 목표위치로 이동한다.
    public void DoMove(Vector3 destpos, float duration)
    {
        //movecom.DoMove(destpos, duration);
    }

    //Vector3 방향 * float 거리
    public void Move(Vector3 moveVec,float speed)
    {
        movecom.Move(moveVec, speed);
    }

    //FSM 현재 상태 리턴
    public States GetState()
    {
        return fsm.GetCurState();
    }

    //FSM 이전 상태 리턴
    public States GetLastState()
    {
        return fsm.GetPreState();
    }

    //FSM 상태 변경
    public void SetState(States state)
    {
        fsm.ChangeState(state);
    }

    //자동 포커싱 ON/OFF
    public void SetOutoFocus(bool val)
    {
        OutoFocus = val;
    }

    //마우스 감도 설정
    public void SetMouseSpeed(float val)
    {
        //0~100의 값을 0~5의 값으로 변환해서 넣어준다.
        val = val * 5 * 0.01f;
        movecom.moveoption.RotMouseSpeed = val;
    }

    //마우스 반전 ON/OFF
    public void SetReverseMouseRot(bool val)
    {
        movecom.moveoption.RightReverse = val;
    }

    //카메라 벽 통과 방지 ON/OFF
    public void SetCameraColl(bool val)
    {
        movecom.CameraCollOn = val;
    }

    //UI 생성
    public void CeateUI(GameObject obj)
    {
        CharacterUIPanel = GameObject.Instantiate(obj).GetComponent<UICharacterInfoPanel>();
    }

    /*MyComponent 관련 메소드*/
    public BaseComponent GetMyComponent(CharEnumTypes.eComponentTypes type)
    {
        if(components[(int)type] ==null)
        {
            ComponentInit();
        }

        return components[(int)type];
    }

    //해당 컴포넌트를 비활성화 시켜준다.
    public void InActiveMyComponent(CharEnumTypes.eComponentTypes type)
    {
        if (components[(int)type] == null)
        {
            ComponentInit();
        }

        components[(int)type].enabled = false;
    }

    //해당 컴포넌트를 활성화 시켜준다.
    public void ActiveMyComponent(CharEnumTypes.eComponentTypes type)
    {
        if (components[(int)type] == null)
        {
            ComponentInit();
        }

        components[(int)type].enabled = true;
    }

    //캐릭터의 시점 1인칭 3인칭 에따라 현재 활성화 되어있는 메인 카메라를 리턴해준다.
    public Camera GetCamera()
    {
        //CMoveComponent movecom = GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
        if(movecom==null)
            movecom = GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;

        return movecom.GetCamera();
    }

    /*플레이어 캐릭터 상호작용 메소드*/

    /*플에이어가 공격을 받았을때 해당 함수를 호출
      현재 플레이어의 상태에 따라서 넉백, 가드넉백, 회피 등등의 동작을 결정한다.
      공격을 당했을때 공격을 당한 위치(충돌한 위치)도 함께 넘겨준다.(피격 이펙트를 출력하기 위해)*/
    public void BeAttacked(float damage, Vector3 hitpoint, float Groggy)
    {

        States state = fsm.GetCurState();


        //1. 무조건 공격이 성공하는 상태(Idle, Move, OutOfControl)
        if (state == States.Idle ||
            state == States.Run ||
            state == States.Walk ||
            state == States.OutOfControl)
        {
            Damaged(damage, hitpoint, Groggy);
        }

        //2. 가드중 
        //밸런스게이지가 충분이 남아 있으면 가드에 성공하고 밸런스 게이지를 감소 시킨다.
        //밸런스 게이지가 충분히 남아 있지 않으면 가드에 실패하고 데미지를 입는다.
        else if(state == States.Guard)
        {
            CGuardComponent guardcom = GetMyComponent(CharEnumTypes.eComponentTypes.GuardCom) as CGuardComponent;

            guardcom.Damaged_Guard(damage, hitpoint,Groggy);
        }

        //3. 회피중
        //캐릭터가 회피중이고 무적시간일때는 공격 회피에 성공하고
        //캐릭터가 회피중이지만 무적시간이 아닐때는 회피에 실패하고 데미지를 입는다.
        else if(state == States.Rolling)
        {
            //CMoveComponent movecom = GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;

            if(!movecom.curval.IsNoDamage)
                movecom.Damaged_Rolling(damage, hitpoint,Groggy);

        }

        //4. 공격중
        else if(state == States.Attack)
        {
            //PlayerAttack attackcom = GetMyComponent(CharEnumTypes.eComponentTypes.AttackCom) as PlayerAttack;
            CAttackComponent attackcom = GetMyComponent(CharEnumTypes.eComponentTypes.AttackCom) as CAttackComponent;
            attackcom.Damaged_Attacking(damage, hitpoint, Groggy);
            //Damaged();
        }

    }

    
    public void Damaged(float damage,Vector3 hitpoint, float Groggy)
    {
        EffectManager.Instance.InstantiateEffect(HitEffect, hitpoint);
        //최종 데미지 = 상대방 데미지 - 나의 현재 방어막
        float finaldamage = damage - status.Defense;
        
        status.GroggyUp(Groggy);
        status.CurHP -= finaldamage;
        //SoundManager.Instance.effectSource.GetComponent<AudioSource>().PlayOneShot(SoundManager.Instance.Player_Audio[2]);

        //캐릭터 사망
        //사망 애니메이션 출력하고 씬 재시작 함수 호출
        if (status.CurHP<=0)
        {
            SetState(States.OutOfControl);

            movecom.com.animator.Play(DeadAnimation.name, 1.0f, 0.0f, 0.2f, Restart);
        }

    }

    public void Restart()
    {
        SetState(States.Idle);
        GameData_Load.Instance.ChangeScene(Scenes_Stage.restart_Loading);
    }

    public void Restart(string _val)
    {
        SetState(States.Idle);
        GameData_Load.Instance.ChangeScene(Scenes_Stage.restart_Loading);
    }

    public BaseStatus GetCharacterStatus()
    {
        return status;
    }
    
    //경험치 획득
    public void GetExp(int exp)
    {
        status.CurExp += exp;
    }

    #region OutoFocusing
    //적 탐색 & 포커싱 세팅
    enum eSearchPoint
    {
        Center,
        Top,
        Right,
        Bottom,
        Left,
        SMax
    }

    [System.Serializable]
    public class Battle_Character_Info
    {
        public Battle_Character_Info(GameObject monster, BoxCollider coll)
        {
            _monster = monster;
            _coll = coll;
            _distance = 0;
            _index = -1;
            _isFocused = false;
            _isBlocked = false;
            _searchPoint = new Vector3[5];
            //피봇으로부터 얼마만큼 떨어져 있는지
            Vector3 center = coll.center;
            Vector3 size = coll.size;
            _searchPoint[(int)eSearchPoint.Center] = center;
            _searchPoint[(int)eSearchPoint.Top] = center + new Vector3(0, size.y/2, 0);
            _searchPoint[(int)eSearchPoint.Right] = center + new Vector3(size.x/2, 0, 0);
            _searchPoint[(int)eSearchPoint.Bottom] = center + new Vector3(0, -size.y / 2, 0);
            _searchPoint[(int)eSearchPoint.Left] = center + new Vector3(-size.x / 2, 0, 0);
        }

        public GameObject _monster;
        public BoxCollider _coll;
        public float _distance;
        public int _index;
        public bool _isFocused;
        public bool _isBlocked;
        public Vector3[] _searchPoint;
    }

    public bool _outoFocus = false;
    public bool OutoFocus
    {
        get
        {
            return _outoFocus;
        }
        set
        {
            _outoFocus = value;
            if (value)
            {
                if (MonsterSearchCor==null)
                {
                    MonsterSearchCor = MonsterSearchCoroutine();
                    StartCoroutine(MonsterSearchCor);
                }
                FocusTab();
            }

        }
    }

    public List<Battle_Character_Info> _monsterObject = new List<Battle_Character_Info>();
    public float _monsterSearchTime = 3.0f;
    private float lastsearchTime;

    public int CurMonsterIndex = 0;
    private bool _isFocusingOn = false;
    public bool IsFocusingOn
    {
        get
        {
            return _isFocusingOn;
        }
        set
        {
            _isFocusingOn = value;
            if(value)
            {
                movecom.com.TpCam.parent = movecom.com.TpCamPos2;
                movecom.com.TpCam.localPosition = Vector3.zero;
                movecom.com.TpCam.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                movecom.com.TpCam.parent = movecom.com.TpCamPos;
                movecom.com.TpCam.localPosition = Vector3.zero;
                movecom.com.TpCam.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
    public int CurFocusedIndex = -1;
    public GameObject CurFocusedMonster;
    public IEnumerator MonsterSearchCor = null;

    public LayerMask Bosslayer;



    //일정 시간마다 화면에 있는 몬스터들을 확인해서 거리별로 리스트에 넣는다.
    //해당 몬스터의 콜라이더를 받아온다.
    public IEnumerator MonsterSearchCoroutine()
    {
        GameObject[] temp;
        List<Battle_Character_Info> tempViewMonster = new List<Battle_Character_Info>();
        BoxCollider Coll;
        RaycastHit hit;
        while (true)
        {
            //Debug.Log("[focus]몬스터 탐지 시작");
            tempViewMonster.Clear();

            temp = AddressablesLoadManager.Instance.ActiveObjectReturn<GameObject>().ToArray();
            // = 

            //해당 몬스터가 카메라 안에 있는지 확인
            for (int i = 0; i < temp.Length; i++)
            {
                if(temp[i].CompareTag("Enemy"))
                {
                    Coll = temp[i].GetComponent<BoxCollider>();
                    Vector3 screenPos = GetCamera().WorldToViewportPoint(Coll.bounds.center);
                    if (screenPos.x >= 0 && screenPos.x <= 1 && screenPos.y >= 0 && screenPos.y <= 1 && screenPos.z >= 0)
                    {
                        //Debug.Log(temp[i].gameObject.name + "[focus]화면에 탐지");
                        
                        Battle_Character_Info info = new Battle_Character_Info(temp[i], Coll);
                        tempViewMonster.Add(info);
                    }
                }
            }

            //카메라 안에 있으면 해당 물체로 ray를 쏴서 장애물이 있는지와 거리를 확인한다.
            //Ray는 캐릭터에서 몬스터로 쏘는게 아니고 카메라에서 몬스터로 쏜다
            //몬스터의 중심, 상,하,좌,우 이렇게 쏜다.
            //몬스터의 중심 상하좌우의 스크린 포인트

            for (int i = tempViewMonster.Count - 1; i >= 0; i--)
            {
                //for (int point = (int)eSearchPoint.Center; point < (int)eSearchPoint.SMax; point++)
                //{
                //    //카메라에서 몬스터 쪽으로 레이를 쏜다.
                //    GameObject monster = tempViewMonster[i]._monster;
                //    Vector3 dir = (monster.transform.position + tempViewMonster[i]._searchPoint[point]) - GetCamera().transform.position;

                //    //레이캐스트 발사
                //    if (Physics.Raycast(GetCamera().transform.position, dir, out hit, 100.0f, Bosslayer))
                //    {
                //        //if(hit.transform.gameObject.layer)
                //        //if(!hit.transform.CompareTag("Enemy"))
                //        if (hit.collider == null)
                //        {
                //            //Debug.Log("[focus]몬스터 탐색 지워져버림");
                //            tempViewMonster.RemoveAt(i);
                //            //tempViewMonster.
                //            //tempViewMonster[i]._isBlocked = true;
                //            //tempViewMonster[i]._distance = 0;
                //            break;
                //        }
                //        else
                //        {
                //            //Debug.Log("[focus]몬스터 탐색 안지워짐");
                //            tempViewMonster[i]._distance = hit.distance;
                //        }
                //    }
                //}
                GameObject monster = tempViewMonster[i]._monster;
                tempViewMonster[i]._distance = (monster.transform.position - transform.position).magnitude;
            }

            //거리에 따라 정렬
            _monsterObject = tempViewMonster.OrderBy(x => x._distance).ToList();

            //
            if(_monsterObject.Count<=0)
            {
                IsFocusingOn = false;
                CurFocusedIndex = 0;
                CurFocusedMonster = null;
                MonsterSearchCor = null;
                yield break;
            }

            //탕색과 정렬을 끝냈는데 현재 포커싱 중인 몬스터가 사라졌으면 포커싱을 끝내준다.
            if(IsFocusingOn)
            {
                int index = _monsterObject.FindIndex(x => x._monster == CurFocusedMonster);
                //탐색을 완료 했는데 포커싱 중인 몬스터가 없어졌을때
                if (index == -1)
                {
                    //오토 포커싱 중이면 다른 몬스터가 있으면 그 몬스터로 포커싱을 옮겨주고
                    //아무것도 없으면 그때 끝내준다.
                    if(OutoFocus)
                    {
                        if(_monsterObject.Count>0)
                        {
                            CurFocusedIndex = 0;
                            yield return null;
                        }
                        else//오토 포커싱 중인데 탐색 결과 
                        {
                            //Debug.Log("[focus] 오토포서싱 중일때 탐색결과 몬스터 존재 X");
                            IsFocusingOn = false;
                        }
                    }
                    else
                    {
                        //Debug.Log("[focus] 오토포커싱 아닐때 탐색결과 몬스터 존재 X");
                        IsFocusingOn = false;
                        CurFocusedIndex = 0;
                        CurFocusedMonster = null;
                        MonsterSearchCor = null;
                        yield break;
                    }

                }
                else
                {
                    CurFocusedIndex = index;
                }
            }




            yield return new WaitForSeconds(_monsterSearchTime);
        }

        //MonsterSearchCor = null;
          
    }

    public void FocusTab()
    {

        if(MonsterSearchCor==null)
        {
            MonsterSearchCor = MonsterSearchCoroutine();
            StartCoroutine(MonsterSearchCor);
        }


        if(!IsFocusingOn)
        {
            if (_monsterObject.Count > 0)
            {
                
                IsFocusingOn = true;
                CurFocusedIndex = 0;
                CurFocusedMonster = _monsterObject[0]._monster;
                //Debug.Log(CurFocusedMonster._monster.gameObject.name + "포커싱 시작");
            }
        }
        else
        {
            if(!OutoFocus)
            {
                if (CurFocusedIndex == _monsterObject.Count - 1)
                {
                    //Debug.Log("[focus]포커싱 눌러서 꺼짐");
                    IsFocusingOn = false;
                    StopCoroutine(MonsterSearchCor);
                    MonsterSearchCor = null;
                }
            }
            
                
            CurFocusedIndex = (CurFocusedIndex + 1) % _monsterObject.Count;
            CurFocusedMonster = _monsterObject[CurFocusedIndex]._monster;


        }


    }
    #endregion
  
}
