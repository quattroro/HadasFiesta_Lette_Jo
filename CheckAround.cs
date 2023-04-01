using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///캐릭터가 현재 바닥에 닿아있는지, 캐릭터의 앞이 벽으로 막혀있는지
///캐릭터가 현재 서있는 바닥의 각도, 앞의 벽의 각도, 앞에 있는 벽이 계단인지 등등을 검사한다.
///FixedUpdate를 통해 수행된다
/////////////////////////////////////////////////////////////////////

public class CheckAround : MonoBehaviour
{
    CurState curval;
    CMoveComponent movecom;


    public CapsuleCollider CapsuleCol = null;
    public Vector3 Capsuletopcenter => new Vector3(transform.position.x, transform.position.y + CapsuleCol.height - CapsuleCol.radius, transform.position.z);
    public Vector3 Capsulebottomcenter => new Vector3(transform.position.x, transform.position.y + CapsuleCol.radius, transform.position.z);


    
    Vector3 temppos;

    private void Awake()
    {
        CapsuleCol = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        if(movecom==null)
        {
            movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
            curval = movecom.curval;
        }
        //tempcube = GameObject.Find("tempcube");
        //testnavagent = GetComponent<NavMeshAgent>();
    }


    public float testhitangle;

    //앞을 검사한다.
    public void CheckFront()
    {
        if (movecom == null)
        {
            movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
            curval = movecom.curval;
        }
        RaycastHit[] hits;
        curval.CurFowardSlopAngle = 0;
        curval.IsFowordBlock = false;


        curval.IsStep = false;



        hits = Physics.CapsuleCastAll(Capsuletopcenter, Capsulebottomcenter, CapsuleCol.radius - 0.1f, movecom.com.FpRoot.forward,  0.3f/*, LayerMask.GetMask("Wall")*/);
        

        if (hits.Length>0)
        {
            foreach(RaycastHit hit in hits)
            {
                //전방 검사에서 무언가에 막혀있고 막고있는 물체가 벽이면 계단검사를 수행하지 않는다.
                if(hit.transform.gameObject.layer == LayerMask.NameToLayer("Wall"))
                {
                    curval.CurFowardSlopAngle = Vector3.Angle(hit.normal, Vector3.up);
                    if (curval.CurFowardSlopAngle >= 70.0f)
                    {
                        curval.IsFowordBlock = true;
                    }
                    
                    return;
                }//if(Layer(Wall))


                //전방에 막혀있는 물체가 땅이라면 전방의 경사만 체크하고 리턴한다.
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    curval.CurFowardSlopAngle = Vector3.Angle(hit.normal, Vector3.up);
                    if (curval.CurFowardSlopAngle >= 70.0f)
                    {
                        curval.IsFowordBlock = true;
                    }
                    return;
                }//if(Layer(Ground))


                //정방에 막혀있는 물체가 계달일때
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Stairs"))
                {
                    //앞이 계단이면 가로막혀있는 계단의 높이를 구한다.
                    Vector3 pos = movecom.Capsuletopcenter + (movecom.com.FpRoot.forward * movecom.moveoption.StepCkeckDis);
                    RaycastHit hit2;
                    Vector3 direction = this.transform.position + (movecom.com.FpRoot.forward * movecom.moveoption.StepCkeckDis) - movecom.Capsuletopcenter;

                    Ray ray = new Ray(movecom.Capsuletopcenter, direction);

                    //Debug.DrawLine(ray.origin, ray.origin + ray.direction * movecom.CharacterHeight, Color.red);
                    bool falg = Physics.Raycast(ray, out hit2, movecom.CharacterHeight, LayerMask.GetMask("Stairs"));

                    if (falg)
                    {
                        testhitangle = Vector3.Angle(hit2.normal, Vector3.up);
                        if (Vector3.Angle(hit2.normal, Vector3.up) == 0)
                        {
                            curval.IsStep = true;
                            curval.CurStepHeight = hit2.point.y - transform.position.y;
                            curval.CurStepPos = hit2.point;
                        }
                        Debug.DrawLine(ray.origin, hit2.point, Color.yellow);
                    }
                }//if(Layer(Stairs))


            }//foreach

        }//if(hits)
    }

    //public void CheckFoward()
    //{
    //    if (movecom == null)
    //    {
    //        movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
    //        curval = movecom.curval;
    //    }
    //    RaycastHit hit;
    //    curval.CurFowardSlopAngle = 0;
    //    curval.IsFowordBlock = false;
    //    //Vector3 temp = new Vector3(WorldMove.x, 0, WorldMove.z);
    //    //temp = com.FpRoot.forward /*+ Vector3.down*/;
    //    //NavMesh.Raycast()
    //    bool cast = Physics.CapsuleCast(Capsuletopcenter, Capsulebottomcenter, CapsuleCol.radius - 0.2f, movecom.com.FpRoot.forward, out hit, 0.3f);
    //    if (cast)
    //    {
    //        Debug.DrawLine(Capsulebottomcenter, hit.point, Color.cyan);
    //        curval.CurFowardSlopAngle = Vector3.Angle(hit.normal, Vector3.up);
    //        if (curval.CurFowardSlopAngle >= 70.0f)
    //        {
    //            curval.IsFowordBlock = true;
    //        }
    //    }
    //}


    //바닥을 검사한다.
    public void CheckGround()
    {
        if (movecom == null)
        {
            movecom = PlayableCharacter.Instance.GetMyComponent(CharEnumTypes.eComponentTypes.MoveCom) as CMoveComponent;
            curval = movecom.curval;
        }
        curval.IsGrounded = false;
        curval.IsSlip = false;
        curval.IsOnTheSlop = false;
        curval.CurGroundSlopAngle = 0;


        if (Time.time >= curval.LastJump + 0.2f)//점프하고 0.2초 동안은 지면검사를 하지 않는다.
        {
            //RaycastHit hit;

            NavMeshHit navhit;

            RaycastHit[] hits;

            temppos = new Vector3(this.transform.position.x, this.transform.position.y  - 10, this.transform.position.z);
            //tempcube.transform.position = temppos;

            //bool cast = testnavagent.Raycast(temppos, out navhit);
            //bool cast = NavMesh.Raycast(this.transform.position + new Vector3(0,2,0), temppos, out navhit, NavMesh.GetAreaFromName("Walkable"));
            //Debug.DrawLine(this.transform.position + new Vector3(0, 2, 0), temppos, cast ? Color.red : Color.blue);
            //bool cast = Physics.SphereCast(Capsulebottomcenter, CapsuleCol.radius, Vector3.down, out hit, CapsuleCol.radius-0.15f,LayerMask.GetMask("Ground"));
            hits = Physics.SphereCastAll(Capsulebottomcenter, CapsuleCol.radius, Vector3.down, CapsuleCol.radius - 0.15f);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Stairs") || hit.transform.gameObject.layer == LayerMask.NameToLayer("Ground"))
                    {
                        Debug.DrawLine(Capsulebottomcenter, hit.point, Color.blue);

                        curval.IsGrounded = true;
                        curval.CurGroundPoint = hit.point;
                        curval.CurGroundNomal = hit.normal;
                        curval.CurGroundSlopAngle = Vector3.Angle(hit.normal, Vector3.up);

                        curval.CurFowardSlopAngle = Vector3.Angle(hit.normal, movecom.com.FpRoot.forward) - 90f;

                        if (curval.CurGroundSlopAngle > 1.0f)
                        {
                            curval.IsOnTheSlop = true;
                            if (curval.CurGroundSlopAngle >= movecom.moveoption.MaxSlop)
                            {
                                curval.IsSlip = true;
                            }
                        }
                        curval.CurGroundCross = Vector3.Cross(curval.CurGroundNomal, Vector3.up);
                    }
                }
            }
        }

    }

    

    private void FixedUpdate()
    {
        CheckFront();
        CheckGround();

    }

}
