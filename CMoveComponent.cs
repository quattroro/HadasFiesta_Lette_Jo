using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///캐릭터의 이동을 담당하는 클래스
///경사로에서의 이동, 구르기, 걷기,달리기 절벽 떨어짐 등등의 움직임들 담당한다.
///또한 자동이동에서 사용할 자동이동 기능도 포함 되어 있다.
/////////////////////////////////////////////////////////////////////


public class CMoveComponent : BaseComponent
{
    public enum Direction
    {
        Front,
        Right,
        Left,
        Back
    }

    CheckAround checkaround;
    public override void InitComtype()
    {
        p_comtype = CharEnumTypes.eComponentTypes.MoveCom;
    }

    [System.Serializable]
    public class Com
    {
        public Transform CharacterRoot = null;

        public Transform TpCamRig = null;
        public Transform TpCam = null;
        public Transform TpCamPos = null;
        public Transform TpCamPos2 = null;

        public Transform FpRoot = null;
        public Transform FpCamRig = null;
        public Transform FpCam = null;

        public Transform CharacterTransform = null;
        public Rigidbody CharacterRig = null;

        public CapsuleCollider CapsuleCol = null;

        public AnimationController animator = null;
    }

    [System.Serializable]
    public class MoveOption
    {
        [Header("==================이동 관련 변수들==================")]
        
        public float RotMouseSpeed = 10f;
        
        public bool RightReverse = false;



        
        [Range(0.0f,1.0f)]
        public float RotSpeed = 0.5f;
        
        public float MoveSpeed;
        
        public float RunSpeed;
        
        public float MinAngle;
        
        public float MaxAngle;
       
        public float Gravity;//중력값(프레임단위로 증가시켜줄 값)
        
        public float JumpPower = 120;//점프를 하면 해당 값으로 curgravity값을 바꿔준다.
        
        public float JumpcoolTime = 1f;
        
        public LayerMask GroundMask;
        
        public float MaxSlop = 70;
        
        public float SlopAccel;//(중력값과 같이 미끌어질때 점점증가될 값)

        public float MaxStep;

        public float RunningStaminaVal;

        [Header("==================계단 이동 관련 변수들==================")]

        public float StepHeight;//올라갈 수 있는 계단 높이

        public float StepCkeckDis;//눈높이에서 해당 위치만큼 이동한 곳에서 수직아래로 레이를 쏜다.

        public float StepWeightVal = 3000;//계단을 올라가기 위한 가중치 연속적인 계단을 올라갈때 움직임 속도가 빨라지는 

        

        [Header("==================회피 관련 변수들==================")]
        public AnimationClip RollingClip;

        public float RollingClipPlaySpeed = 2.3f;

        public float RollingDistance = 80;

        public float RollingTime;

        [Header("무적 시작 시간")]
        public float RollingNoDamageStartTime;
        [Header("무적 유지 시간(회피동작이 끝나면 자동으로 끊김)")]
        public float RollingNoDamageTime;

        //public float RollingFreeDamageTime;

        public float NextRollingTime = 0.1f;

        public float RollingStaminaDown = 20.0f;

        
        [Header("==================피격 관련 변수들==================")]
        public AnimationClip KnockDownClip;

        public AnimationClip KnockBackClip;

        public float KnockDownTime;

        public float KnockBackTime;

        [Header("==================가드 중 이동 관련 변수들==================")]
        public string GuardFrontMoveClip;
        public string GuardRightMoveClip;
        public string GuardLeftMoveClip;
        public string GuardBackMoveClip;

        public float GuardMoveSpeed;

        [Header("==================카메라 관련 변수들==================")]
        public float FPMaxZoomIn = 60.0f;
        public float FPMaxZoomOut = 90.0f;

        public float TPMaxZoomIn = 60.0f;
        public float TPMaxZoomOut = 90.0f;

        public float ScrollSpeed = 500.0f;
    }

    //[HideInInspector]
    public Vector2 MouseMove = Vector2.zero;
    //[HideInInspector]
    public Vector3 MoveDir = Vector3.zero;
    //[HideInInspector]
    public Vector3 WorldMove = Vector3.zero;
    //[HideInInspector]
    public float CurGravity;//현재 벨로시티의 y값
    //[HideInInspector]
    public Com com = new Com();
    //[HideInInspector]
    public CurState curval = new CurState();

    public MoveOption moveoption = new MoveOption();
    [HideInInspector]
    public CInputComponent inputcom = null;
    [HideInInspector]
    public float RollingStartTime;
    [HideInInspector]
    public AnimationEventSystem eventsystem;
    [HideInInspector]
    public CorTimeCounter timecounter = new CorTimeCounter();
    [HideInInspector]
    public float lastRollingTime;
    [HideInInspector]
    public float lastRunningTime;
    [HideInInspector]
    public Vector3 Capsuletopcenter => new Vector3(transform.position.x, transform.position.y + com.CapsuleCol.height - com.CapsuleCol.radius, transform.position.z);
    [HideInInspector]
    public Vector3 Capsulebottomcenter => new Vector3(transform.position.x, transform.position.y + com.CapsuleCol.radius, transform.position.z);

    public float CurStepWeight = 0;

    public float CharacterHeight => com.CapsuleCol.height;

    [HideInInspector]
    public delegate void Invoker(string s_val);
    [HideInInspector]
    public delegate void ActionInvoker();

    private IEnumerator RollingNoDamageStartCor = null;
    private IEnumerator RollingNoDamageEndCor = null;

    //[Header("============TestVals============")]

    //public Vector3 updown;
    //public float xnext;

    //public Vector3 rightleft;
    //public float ynext;

    //public Transform playerChestTr;


    public Vector3 teststart;
    public Vector3 testend;

    //public Transform testcube;

    void Start()
    {
        //초기화들
        if (TryGetComponent<CheckAround>(out checkaround) == false)
        {
            gameObject.AddComponent<CheckAround>();
            TryGetComponent<CheckAround>(out checkaround);
        }
        eventsystem = GetComponentInChildren<AnimationEventSystem>();
        com.CharacterRig = GetComponent<Rigidbody>();
        com.CapsuleCol = GetComponent<CapsuleCollider>();
        com.animator = GetComponentInChildren<AnimationController>();

        

        //사용하는 애니메이션들에 이벤트들을 연결해 줍니다.
        eventsystem.AddEvent(new KeyValuePair<string, AnimationEventSystem.beginCallback>(null, null),0.0f,
                             new KeyValuePair<string, AnimationEventSystem.midCallback>(moveoption.KnockDownClip.name, KnockDownPause),1.0f,
                             new KeyValuePair<string, AnimationEventSystem.endCallback>(moveoption.KnockDownClip.name, KnockDownEnd),com.animator.GetClipLength(moveoption.KnockDownClip.name));

        eventsystem.AddEvent(new KeyValuePair<string, AnimationEventSystem.beginCallback>(null, null), 0.0f,
                             new KeyValuePair<string, AnimationEventSystem.midCallback>(null, null), 1.0f,
                             new KeyValuePair<string, AnimationEventSystem.endCallback>(moveoption.KnockBackClip.name, KnockBackEnd), com.animator.GetClipLength(moveoption.KnockBackClip.name));

        
        //기본 옵션 초기화들
        ChangePerspective();
        ShowCursor(false);
        LookAtFoward();



        //if (playerChestTr == null)
        //    playerChestTr = com.animator.animator.GetBoneTransform(HumanBodyBones.Head);
    }

    public void Stop()
    {
        com.CharacterRig.velocity = new Vector3(0, 0, 0);
    }

    public void GuardMove(Direction dir)
    {
        Vector3 tempmove;
        Vector3 tempdir;
        if (dir == Direction.Front)
        {
            tempdir = new Vector3(0, 0, 1);
            com.animator.Play(moveoption.GuardFrontMoveClip, 1.5f);
        }
        else if (dir == Direction.Left)
        {
            tempdir = new Vector3(-1, 0, 0);
            com.animator.Play(moveoption.GuardLeftMoveClip, 1.5f);
        }
        else if (dir == Direction.Right)
        {
            tempdir = new Vector3(1, 0, 0);
            com.animator.Play(moveoption.GuardRightMoveClip, 1.5f);
        }
        else
        {
            tempdir = new Vector3(0, 0, -1);
            com.animator.Play(moveoption.GuardBackMoveClip, 1.5f);
        }
        tempmove = com.TpCamRig.TransformDirection(tempdir);

        tempmove.y = 0;

        Move(tempmove, moveoption.GuardMoveSpeed);
    }

    

    IEnumerator automovestopcor = null;

    public void AutoMove(Vector3 destpos, float moveTime, ActionInvoker invoker = null)
    {
        Vector3 startpos = this.transform.position;
        Vector3 directon = destpos - startpos;

        PlayableCharacter.Instance.SetState(PlayableCharacter.States.AutoMove);

        StartCoroutine(CorDoMove(destpos,moveTime, invoker));
        automovestopcor = timecounter.Cor_TimeCounter(moveTime, AutoMoveStop);
        StartCoroutine(automovestopcor);
    }


    public void AutoMoveStop()
    {
        PlayableCharacter.Instance.SetState(PlayableCharacter.States.Idle);
        if (automovestopcor != null)
        {
            StopCoroutine(automovestopcor);
            automovestopcor = null;
        }
    }

    //해당 지점으로 이동한다.
    public void DoMoveDir(Vector3 dest, Invoker invoker = null)
    {
        Vector3 startpos = transform.position;
        Vector3 direction = dest - startpos;


    }



    //duration 시간동안 목표위치로 이동한다.
    public void FowardDoMove(float distnace, float duratoin)
    {
        Vector3 direction = com.FpRoot.forward * distnace;
        Vector3 dest = transform.position + direction;

        StartCoroutine(CorDoMove(transform.position, dest, duratoin));
    }


    public Camera GetCamera()
    {
        if (curval.IsFPP)
            return com.FpCam.GetComponent<Camera>();
        else
            return com.TpCam.GetComponent<Camera>();
    }



    //걷기면 걷기, 달리기면 달리기 둘 중에 선택해서 움직이고 
    public IEnumerator CorDoMove(Vector3 start, Vector3 dest, Invoker invoker = null)
    {
        teststart = start;
        testend = dest;

        Vector3 direction = dest - start;
        float distance = direction.magnitude;
        direction.Normalize();


        float startTime = Time.time;
        float lastTime = Time.time;
        float curTime = 0.0f;

        while (true)
        {

            Vector3 curdest = dest - transform.position;
            


            curTime += Time.time - lastTime;

            if (PlayableCharacter.Instance.GetState()== PlayableCharacter.States.OutOfControl)
            {
                if (invoker != null)
                    invoker.Invoke("");

                Move(new Vector3(0, 0, 0),0);
                yield break;
            }

            if (!curval.IsFowordBlock)
                Move(direction, moveoption.RunSpeed /** (curTime/duration)*/);

            lastTime = Time.time;
            yield return null;
        }

    }

    //걷기면 걷기, 달리기면 달리기 둘 중에 선택해서 움직이고 
    public IEnumerator CorDoMove(Vector3 start, Vector3 dest, float duration, Invoker invoker = null)
    {
        //float runtime = 0.0f;
        teststart = start;
        testend = dest;

        Vector3 direction = dest - start;
        float distance = direction.magnitude;
        direction.Normalize();


        float startTime = Time.time;
        float lastTime = Time.time;
        float curTime = 0.0f;

        while (true)
        {
            Vector3 curdest = dest - transform.position;



            curTime += Time.time - lastTime;

            if (PlayableCharacter.Instance.GetState() == PlayableCharacter.States.OutOfControl)
            {
                if (invoker != null)
                    invoker.Invoke("");

                Move(new Vector3(0, 0, 0), 0);
                yield break;
            }



            if (curTime >= duration)
            {
                if (invoker != null)
                    invoker.Invoke("");


                Move(new Vector3(0, 0, 0), 0);

                yield break;
            }

            if (!curval.IsFowordBlock)
                Move(direction, distance*100);

            lastTime = Time.time;
            yield return null;
        }

    }



    //목적지까지 이동
    public IEnumerator CorDoMove(Vector3 dest, float maxTime, ActionInvoker invoker = null)
    {

        float startTime = Time.time;
        float lastTime = Time.time;
        float curTime = 0.0f;

        Vector3 curPosition = transform.position;
        curPosition.y = 0;
        dest.y = 0;

        Vector3 curDirection = dest - curPosition;

        

        LookAtBody(curDirection);

        while (true)
        {
            curPosition = transform.position;
            curPosition.y = 0;

            curDirection = dest - curPosition;
            

            curTime += Time.time - lastTime;

            if (PlayableCharacter.Instance.GetState() == PlayableCharacter.States.OutOfControl||
                PlayableCharacter.Instance.GetState() != PlayableCharacter.States.AutoMove)
            {
                if (automovestopcor != null)
                {
                    StopCoroutine(automovestopcor);
                    automovestopcor = null;
                }
                    

                Move(new Vector3(0, 0, 0), 0);
                yield break;
            }

            if (curDirection.magnitude <= 0.1f)
            {
                PlayableCharacter.Instance.SetState(PlayableCharacter.States.Idle);

                if (invoker != null)
                    invoker.Invoke();

                if (automovestopcor != null)
                {
                    StopCoroutine(automovestopcor);
                    automovestopcor = null;
                }

                Move(new Vector3(0, 0, 0), 0);

                yield break;
            }


            if (!curval.IsFowordBlock)
                Move(curDirection.normalized, moveoption.RunSpeed /** (curTime/duration)*/);

            lastTime = Time.time;
            yield return null;
        }

    }



    bool temptrigger = false;

    //움직임값을 계산해준다.
    public void UpdateMove()
    {
        //이동이 가능한지 판단
        if (!curval.CheckMoveAble())
        {
            WorldMove.x = 0;
            WorldMove.z = 0;
            return;
        }
        if(MoveDir.sqrMagnitude<=0)
        {
            Move(Vector3.zero, 0);
        }
        if(curval.IsGuard)
        {
            return;
        }
        

        MoveDir.Normalize();


        //y성분은 버린다.
        WorldMove = com.TpCamRig.TransformDirection(MoveDir);
        WorldMove.y = 0;
        
        WorldMove.Normalize();

        float speed = (curval.IsRunning && PlayableCharacter.Instance.status.CurStamina - moveoption.RunningStaminaVal >= 0) ? moveoption.RunSpeed : moveoption.MoveSpeed;
        //float speed = (curval.IsRunning && PlayableCharacter.Instance.status.CurStamina  > 0) ? moveoption.RunSpeed : moveoption.MoveSpeed;

        if (WorldMove.magnitude > 0)
        {
            int a = 0;
            a = 10;
            temptrigger = true;
        }


        Move(WorldMove, speed);
    }

    Vector3 templastmovevec;

    public Vector3 slopVector;

    
    //움직일 방향을 넣어주면 지형을 판단해서 다음 프레임에 어떤 방향으로 움직여야 하는지 넘겨준다.
    //해당 방향으로 Front, Ground 체크를 한다.
    //만약 미끌어지는 지면 위에 올라와 있으면 반대 방향으로 가도록 한다.
    public Vector3 MoveCal(Vector3 movedir)
    {
        movedir = Quaternion.AngleAxis(-curval.CurGroundSlopAngle, curval.CurGroundCross) * movedir;//경사로에 의한 y축 이동방향

        //경사로에 있을때
        if (curval.IsOnTheSlop)
        {
            //움직일 수 없는 각도의 경사로 위에 있을때 (미끌어질때)
            if (curval.IsSlip)
            {
                //올라가던지 내려가던지 무조건 경사로의 아래 방향으로 경사로를 따라서 미끌어지도록
                curval.CurHorVelocity = Vector3.up;

                slopVector = Quaternion.AngleAxis(-(curval.CurGroundSlopAngle + 90), curval.CurGroundCross) * curval.CurHorVelocity;
                curval.MoveAccel = moveoption.SlopAccel;
                curval.CurHorVelocity = Quaternion.AngleAxis(-(curval.CurGroundSlopAngle + 90), curval.CurGroundCross) * curval.CurHorVelocity * curval.MoveAccel;
                return (curval.CurHorVelocity + curval.CurVirVelocity);
            }
            //움직일 수 있는 경사로 움직일때
            else
            {
                //올라가는 중 일때는 경사로의 각도에 따라서 움직이는 속도가 느려지도록 하고
                //내려가는 중 일때는 경사로의 각도에 따라서 움직이는 속도가 빨라지도록 한다.
                curval.CurHorVelocity = movedir;

                return ((curval.CurHorVelocity + curval.CurVirVelocity));
            }
        }
        //경사로가 아닐때
        else
        {
            return new Vector3(movedir.x, CurGravity, movedir.y);
        }
    }


    //움직일 방향과 거리를 넣어주면 현재 지형에 따라서 움직여 준다.
    //기울어진 지형과 계단에서의 움직임 처리 제작 필요
    public void Move(Vector3 MoveVal , float speed)
    {
        //바닥이 있는지 없는지만 확인하고 바닥이 없을때만 내려간다.
        if(curval.CheckStepAble())
        {
            if(CurStepWeight>=moveoption.StepWeightVal)
            {
                this.transform.position = curval.CurStepPos;
                CurStepWeight = 0;
                return;
            }
            else
            {
                CurStepWeight += MoveVal.magnitude;
            }
        }

        MoveVal = Quaternion.AngleAxis(-curval.CurGroundSlopAngle, curval.CurGroundCross) * MoveVal;//경사로에 의한 y축 이동방향

        //경사로에 있을때
        if (curval.IsOnTheSlop)
        {
            //움직일 수 없는 각도의 경사로 위에 있을때 (미끌어질때)
            if (curval.IsSlip)
            {
                //올라가던지 내려가던지 무조건 경사로의 아래 방향으로 경사로를 따라서 미끌어지도록
                curval.CurHorVelocity = Vector3.up;

                slopVector = Quaternion.AngleAxis(-(curval.CurGroundSlopAngle+90), curval.CurGroundCross) * curval.CurHorVelocity;
                curval.MoveAccel = moveoption.SlopAccel;
                curval.CurHorVelocity = Quaternion.AngleAxis(-(curval.CurGroundSlopAngle + 90), curval.CurGroundCross) * curval.CurHorVelocity * curval.MoveAccel;
                com.CharacterRig.velocity = ((curval.CurHorVelocity + curval.CurVirVelocity) * 10.0f * Time.deltaTime);

            }
            //움직일 수 있는 경사로 움직일때
            else
            {
                //올라가는 중 일때는 경사로의 각도에 따라서 움직이는 속도가 느려지도록 하고
                //내려가는 중 일때는 경사로의 각도에 따라서 움직이는 속도가 빨라지도록 한다.
                curval.CurHorVelocity = MoveVal;

                com.CharacterRig.velocity = ((curval.CurHorVelocity + curval.CurVirVelocity) * speed * Time.deltaTime);

            }
        }
        //경사로가 아닐때
        else
        {
            MoveVal = MoveVal * speed * Time.deltaTime;
            com.CharacterRig.velocity = new Vector3(MoveVal.x, CurGravity, MoveVal.z);
        }

    }



    //모든 회전이 완료된 다음에 동작해야 한다.
    //x,z축의 움직임을 담당 y축의 움직임은 따로 관리
    public void HorVelocity()
    {
        //CurHorVelocity = com.FpCamRig.forward;


        if (curval.IsSlip)
        {
            //움직임을 현재 바닥 경사각의 -로 해서 회전을 시킴
        }

        curval.CurHorVelocity = Quaternion.AngleAxis(-curval.CurGroundSlopAngle, curval.CurGroundCross) * curval.CurHorVelocity;//이럭식으로 벡터를 회전시킬 수 있다. 역은 성립하지 않는다.

    }

    //땅 위에 없음면 땅을 만날때까지 떨어진다.
    //CurGravity를 누적된 증가값에 따라 증가시켜 준다. 해당 중력값은 Move함수에서 y축 방향 움직임으로 사용된다.
    public void Falling()
    {
        float deltacof = Time.deltaTime * 10f;


        if (curval.IsGrounded)
        {
            if (curval.IsJumping)
                curval.IsJumping = false;

            CurGravity = 0;
            moveoption.Gravity = 1;
        }
        else
        {
            moveoption.Gravity += 0.098f;
            CurGravity -= deltacof * moveoption.Gravity;
        }
    }

    public void Jump()
    {
        if (Time.time >= curval.LastJump + moveoption.JumpcoolTime)
        {
            curval.LastJump = Time.time;
            curval.IsJumping = true;
            CurGravity = moveoption.JumpPower;
        }

    }

    private void ShowCursorToggle()
    {
        curval.IsCursorActive = !curval.IsCursorActive;
        ShowCursor(curval.IsCursorActive);
    }

    private void ShowCursor(bool value)
    {
        Cursor.visible = value;
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void KnockDown()
    {
        //이미 넉다운 중일때 해당 함수가 다시 들어오면 그냥 리턴
        if(curval.IsKnockDown)
        {
            return;
        }

        curval.IsKnockDown = true;

        com.animator.Play(moveoption.KnockDownClip.name);
    }

    public void KnockDownPause(string s_val)
    {
        
        if (com.animator.GetPlaySpeed() != 0.0f)
        {
            com.animator.Pause();
            StartCoroutine(timecounter.Cor_TimeCounter(moveoption.KnockDownTime, KnockDownPause));
        }
        else
        {
            com.animator.Resume();
        }

    }

    public void KnockDownEnd(string s_val)
    {
        curval.IsKnockDown = false;
    }


    public void KnockBack()
    {
        //이미 넉백 중 일때 해당 함수가 다시 들어오면 다시 넉백 실행
        if(curval.IsKnockBack)
        {
            com.animator.Play(moveoption.KnockBackClip.name);
            return;
        }

        curval.IsKnockBack = true;

        com.animator.Play(moveoption.KnockBackClip.name);
    }

    public void KnockBackEnd(string s_val)
    {
        curval.IsKnockBack = false;
    }


    public void Damaged_Rolling(float damage,Vector3 hitpoint, float Groggy)
    {
        //회피 중 피격 당했을때 무적상태인지 아닌지 판단
        if (/*curval.IsRolling && */curval.IsNoDamage)
        {
            return;
        }
        else
        {
            float nextGroggy = PlayableCharacter.Instance.status.CurGroggy + Groggy;
            if (nextGroggy >= PlayableCharacter.Instance.status.player_Groggy || Groggy >= PlayableCharacter.Instance.status.player_Stagger_Groggy)
                RollingOver();
            PlayableCharacter.Instance.Damaged(damage, hitpoint,Groggy);
        }
    }


    //구르기
    //무적시간은 처음 구르기가 시작된 시점부터 카운트한다.
    public void Rolling()
    {
        if (!curval.CheckRollingAble())
            return;

        //이미 구르고 있으면 구르지 못한다.
        if (Time.time - lastRollingTime <= moveoption.NextRollingTime) 
            return;



        //스테미나가 0이 아니면 실행 가능
        if (PlayableCharacter.Instance.status.CurStamina > 0)
        {
            PlayableCharacter.Instance.status.StaminaDown(moveoption.RollingStaminaDown);
        }
        else
        {
            return;
        }
 
        curval.IsRolling = true;
        com.animator.Play(moveoption.RollingClip.name, moveoption.RollingClipPlaySpeed, 0.0f, 0.2f, RollingOver);

        
        Vector3 tempmove = this.transform.position + com.FpRoot.forward * moveoption.RollingDistance;

        RollingNoDamageStartCor = timecounter.Cor_TimeCounter(moveoption.RollingNoDamageStartTime, ActivateNoDamage);
        StartCoroutine(RollingNoDamageStartCor);
        

        RollingStartTime = Time.time;

        FowardDoMove(moveoption.RollingDistance, moveoption.RollingTime);
    }

    public void RollingOver()
    {
        lastRollingTime = Time.time;
        curval.IsRolling = false;
        curval.IsNoDamage = false;

        if (RollingNoDamageStartCor != null)
        {
            StopCoroutine(RollingNoDamageStartCor);
            RollingNoDamageStartCor = null;
        }

        if (RollingNoDamageEndCor!=null)
        {
            StopCoroutine(RollingNoDamageEndCor);
            RollingNoDamageEndCor = null;
        }
            
    }

    public void ActivateNoDamage()
    {
        curval.IsNoDamage = true;
        RollingNoDamageStartCor = null;

        RollingNoDamageEndCor = timecounter.Cor_TimeCounter(moveoption.RollingNoDamageTime, DeActivateNoDamage);
        StartCoroutine(RollingNoDamageEndCor);
    }

    public void DeActivateNoDamage()
    {
        curval.IsNoDamage = false;
        RollingNoDamageEndCor = null;
    }

    
    public void Rotation()
    {
        //1 인칭 일때
        //fp root로 좌우회전
        //fp cam rig로 상하회전
        if (curval.IsFPP)
        {
            RotateFP();
        }
        else//3 인칭 일때
        //fp root로 좌우회전
        //tp cam rig로 좌우 및 상하회전
        {
            RotateTP();
            RotateTPFP();
        }
    }

    public float testyRotPrev;
    public float testyRotNext;

    //1인칭일때회전 3인칭은 놔두고 1인칭 캐릭터만 회전시켜 준다.
    public void RotateFP()
    {
        float xRotPrev = com.FpRoot.localEulerAngles.y;
        float xRotNext = xRotPrev + MouseMove.x * Time.deltaTime * 50f * moveoption.RotMouseSpeed * ((moveoption.RightReverse) ? -1 : 1);

        //xnext = xRotNext;
        //if (xRotNext > 180f)
        //    xRotNext -= 360f;

        float yRotPrev = com.FpCamRig.localEulerAngles.x;
        testyRotPrev = yRotPrev;
        float yRotNext = yRotPrev + MouseMove.y * Time.deltaTime * 50f * moveoption.RotMouseSpeed /** ((moveoption.RightReverse) ? -1 : 1)*/;
        testyRotNext = yRotNext;

        if (yRotNext >= 90)
            yRotNext = yRotPrev;
        if (yRotNext <= 10)
            yRotNext = yRotPrev;


        com.FpRoot.localEulerAngles = Vector3.up * xRotNext;
        com.FpCamRig.localEulerAngles = Vector3.right * yRotNext;

    }


    //3인칭일때
    public void RotateTP()
    {
        float xRotPrev = com.TpCamRig.localEulerAngles.y;
        float xRotNext = xRotPrev + MouseMove.x * Time.deltaTime * 50f * moveoption.RotMouseSpeed * ((moveoption.RightReverse) ? -1 : 1);


        float yRotPrev = com.TpCamRig.localEulerAngles.x;
        float yRotNext = yRotPrev + MouseMove.y * Time.deltaTime * 50f * moveoption.RotMouseSpeed /** ((moveoption.RightReverse) ? -1 : 1)*/;

        testyRotPrev = yRotPrev;
        testyRotNext = yRotNext;



        if (yRotNext == 0 && xRotNext == 0)
            return;

        com.TpCamRig.localEulerAngles = new Vector3(yRotNext, xRotNext, 0);
    }

    

    //이떄는 마우스로움직이는게아니고 키보드 입력에 따라서 회전 해야 하기때문에 따로 만듦
    public void RotateTPFP()
    {
        float nextRotY = 0;
        Vector3 tempworldmove = com.TpCamRig.TransformDirection(MoveDir);
        
        float curRotY = com.FpRoot.localEulerAngles.y;

        if (tempworldmove.sqrMagnitude != 0)
            nextRotY = Quaternion.LookRotation(tempworldmove, Vector3.up).eulerAngles.y;

        if (!curval.IsMoving) nextRotY = curRotY;

        if (nextRotY - curRotY > 180f) nextRotY -= 360f;
        else if (curRotY - nextRotY > 180f) nextRotY += 360f;

        com.FpRoot.eulerAngles = Vector3.up * Mathf.Lerp(curRotY, nextRotY, moveoption.RotSpeed);
    }


    //3인칭 카메라가 정면방향을 바라보도록 회전
    public void LookAtFoward()
    {
        Vector3 foward = com.FpCamRig.forward;

        Vector3 rot = Quaternion.LookRotation(foward, Vector3.up).eulerAngles;

        Vector3 temp = com.TpCamRig.eulerAngles;

        com.TpCamRig.eulerAngles = new Vector3(rot.x, rot.y, rot.z);
    }

    //3인칭 카메라가 해당 위치를 바라보도록 회전
    public void LookAt(Vector3 lookpos)
    {
        //캐릭터 눈높이에서 
        Vector3 dir = lookpos - com.TpCamRig.transform.position;
        Vector3 rot = Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
        com.TpCamRig.eulerAngles = new Vector3(rot.x, rot.y, rot.z);

    }

    //캐릭터가 해당 방향을 바라보도록 
    public void LookAtBody(Vector3 lookdir)
    {
        Vector3 rot = Quaternion.LookRotation(lookdir, Vector3.up).eulerAngles;

        Vector3 temp = com.TpCamRig.eulerAngles;

        com.FpRoot.eulerAngles = new Vector3(rot.x, rot.y, rot.z);
    }

    public void LookAtBody2()
    {
        Vector3 tempworldmove;
        if (MoveDir != Vector3.zero)
            tempworldmove = com.TpCamRig.TransformDirection(MoveDir);
        else
            tempworldmove = com.TpCamRig.forward;

        Vector3 rot = Quaternion.LookRotation(tempworldmove, Vector3.up).eulerAngles;

        Vector3 temp = com.TpCamRig.eulerAngles;

        com.FpRoot.eulerAngles = new Vector3(rot.x, rot.y, rot.z);
    }

    public void LookAtToLookDir()
    {
        Vector3 lookdir = com.TpCamRig.forward;

        Vector3 rot = Quaternion.LookRotation(lookdir, Vector3.up).eulerAngles;

        Vector3 temp = com.TpCamRig.eulerAngles;

        com.FpRoot.eulerAngles = new Vector3(rot.x, rot.y, rot.z);
    }


    //줌인 
    public void ZoomIn(float scroll)
    {
        if (scroll >=0)
            return;
        Camera curcam;
        Camera cCam = null;

        if(curval.IsFPP)
        {
            curcam = com.FpCam.GetComponent<Camera>();
           
            if (curcam.fieldOfView <= moveoption.FPMaxZoomIn)
            {
                curcam.fieldOfView = moveoption.FPMaxZoomIn;
                return;
            }



        }
        else
        {
            curcam = com.TpCam.GetComponent<Camera>();
            cCam = com.TpCam.Find("CCam").GetComponent<Camera>();

            if (curcam.fieldOfView <= moveoption.TPMaxZoomIn)
            {
                curcam.fieldOfView = moveoption.TPMaxZoomIn;
                cCam.fieldOfView = moveoption.TPMaxZoomIn;
                return;
            }


        }

        scroll = scroll * moveoption.ScrollSpeed * Time.deltaTime;

        curcam.fieldOfView += scroll;
        if (cCam != null)
            cCam.fieldOfView += scroll;
    }

    //줌 아웃
    public void ZoomOut(float scroll)
    {
        if (scroll<=0)
            return;

        Camera curcam;
        Camera cCam = null;

        if (curval.IsFPP)
        {
            curcam = com.FpCam.GetComponent<Camera>();

            if (curcam.fieldOfView >= moveoption.FPMaxZoomOut)
            {
                curcam.fieldOfView = moveoption.FPMaxZoomOut;
                return;
            }
        }
        else
        {
            curcam = com.TpCam.GetComponent<Camera>();
            cCam = com.TpCam.Find("CCam").GetComponent<Camera>();

            if (curcam.fieldOfView >= moveoption.TPMaxZoomOut)
            {
                curcam.fieldOfView = moveoption.TPMaxZoomOut;
                cCam.fieldOfView = moveoption.TPMaxZoomOut;
                return;
            }
        }

        scroll = scroll * moveoption.ScrollSpeed * Time.deltaTime;

        curcam.fieldOfView += scroll;
        if(cCam!=null)
            cCam.fieldOfView += scroll;
    }

    public void Focusing()
    {
        PlayableCharacter tempinstance = PlayableCharacter.Instance;
        if (tempinstance.IsFocusingOn)
        {
            int index = tempinstance._monsterObject.FindIndex(x => x._monster == tempinstance.CurFocusedMonster);
            if (index != -1)
            {
                if(tempinstance.CurFocusedMonster!=null)
                {
                    //그냥 캐릭터 시점과 y 높이가 같도록
                    Vector3 pos = tempinstance._monsterObject[index]._coll.bounds.center + new Vector3(0, tempinstance._monsterObject[index]._coll.bounds.extents.y, 0);
                    pos.y = com.TpCamRig.position.y;
                    
                    LookAt(pos);
                }
                
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(teststart, testend);
    }
    void ChangePerspective()
    {
        curval.IsFPP = !curval.IsFPP;
        com.FpCam.gameObject.SetActive(curval.IsFPP);
        com.TpCam.gameObject.SetActive(!curval.IsFPP);

        if (curval.IsFPP)
        {
            startCamPos = com.FpCam.position - com.TpCamRig.position;
            
            camDistance = startCamPos.magnitude;
        }
        else
        {
            startCamPos = com.TpCam.position - com.TpCamRig.position;
            startCampos_Focusing = com.TpCamPos2.position - com.TpCamRig.position;
            camDistance = startCamPos.magnitude;
            camDistance_Focusing = startCampos_Focusing.magnitude;
        }
           

    }


    void Update()
    {
        Falling();
        Rotation();
        UpdateMove();
        Focusing();
        CameraCollision();

        //달리는 중일떄 1초마다 스테미나를 줄여준다.
        if (curval.IsMoving&&curval.IsRunning&& PlayableCharacter.Instance.status.CurStamina >= moveoption.RunningStaminaVal)
        {
            if (Time.time - lastRunningTime >= 1.0f)
            {
                lastRunningTime = Time.time;

                //PlayableCharacter.Instance.status.CurStamina -= moveoption.RunningStaminaVal;
                PlayableCharacter.Instance.status.StaminaDown(moveoption.RunningStaminaVal);
            }
            
        }
        

        Debug.DrawRay(this.transform.position, curval.CurHorVelocity + curval.CurVirVelocity,Color.red);
        Debug.DrawRay(this.transform.position, slopVector, Color.blue);
    }

    public bool CameraCollOn = false;
    public Vector3 startCamPos;//카메라가 캐릭터로부터 어느정도 거리에 떨어져 있는지 초기값
    public Vector3 startCampos_Focusing;
    public float camDistance;
    public float camDistance_Focusing;

    public void CameraCollision()
    {
        Quaternion rot = Quaternion.LookRotation(com.TpCamRig.forward, Vector3.up);
        Vector3 startpos = (PlayableCharacter.Instance.IsFocusingOn) ? startCampos_Focusing : startCamPos;
        Vector3 dir = (rot * startpos).normalized;

        if (CameraCollOn)
        {
            //캐릭터에서 본래 카메라 위치 방향

            Ray ray = new Ray(Capsuletopcenter, dir);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, camDistance))
            {
                //Debug.Log("카메라 충돌");
                GetCamera().transform.position = hit.point;
            }
            else
            {
                GetCamera().transform.position = com.TpCamRig.position + (dir * camDistance);
            }

        }

    }

    //public float testLookY;
    //public float testLookZ;
    //public float testaxisY;
    //public float testaxisZ;
    //private void LateUpdate()
    //{
    //    //상대좌표
    //    Vector3 rot = Quaternion.LookRotation(com.TpCamRig.forward, -playerChestTr.right/*Vector3.up*/).eulerAngles;

    //    testLookY = rot.y;
    //    testLookZ = rot.x;
    //    testaxisY = com.FpRoot.eulerAngles.y;
    //    testaxisZ = com.FpRoot.eulerAngles.z;

    //    if (testaxisY == 0)
    //    {
    //        if (rot.y >= 180)
    //        {
    //            testLookY = rot.y - 360;
    //        }
    //    }
    //    else if(testaxisY >= 360 - 42 || testaxisY <= 85)
    //    {
    //        if (rot.y >= 180)
    //        {
    //            testLookY = rot.y - 360;
    //        }

    //        if(testaxisY>=180)
    //        {
    //            testaxisY -= 360;
    //        }

    //        testLookY = testLookY - testaxisY;
    //    }
    //    else
    //    {
    //        testLookY = rot.y - testaxisY;

    //    }


    //    if (testaxisZ == 0)
    //    {
    //        if (rot.x >= 180)
    //        {
    //            testLookZ = rot.x - 360;
    //        }
    //    }
    //    else if(testaxisZ >= 360 - 33 || testaxisZ <= 35)
    //    {
    //        if (rot.x >= 180)
    //        {
    //            testLookZ = rot.x - 360;
    //        }

    //        if (testaxisZ >= 180)
    //        {
    //            testaxisZ -= 360;
    //        }

    //        testLookZ = testLookZ - testaxisZ;
    //    }
    //    else
    //    {
    //        testLookZ = rot.x - testaxisZ;

    //    }

    //    if(testLookY>=130|| testLookY<=-93)
    //    {
    //        playerChestTr.eulerAngles = new Vector3(playerChestTr.eulerAngles.x, -90, 270);
    //        return;
    //    }

    //    rot.y = GetMinMaxRange(testLookY, -42, 85) + testaxisY;
    //    rot.x = GetMinMaxRange(testLookZ, -33, 35) + testaxisZ;

    //    playerChestTr.eulerAngles = new Vector3(playerChestTr.eulerAngles.x, rot.y - 90, (-rot.x + 270));

    //}

    //float GetMinMaxRange(float val, float min, float max)
    //{
    //    if (val < min)
    //        return min;
    //    if (val > max)
    //        return max;

    //    return val;
    //}

}
