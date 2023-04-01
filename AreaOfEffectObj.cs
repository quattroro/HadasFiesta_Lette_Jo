using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///장판 생성 공격오브젝트
///공격판정은 추가 예정
/////////////////////////////////////////////////////////////////////

public class AreaOfEffectObj : MonoBehaviour
{
    //데미지 판정 주기
    public float DamageTime;

    public string MonsterTag;

    public SphereColl coll;

    public void InitSetting(float _damageTime, string _monsterTag, SphereColl _coll)
    {

    }

    private float lastTime;

    //IEnumerator 
    public void Update()
    {
        if ((Time.time - lastTime) >= DamageTime)
        {
            lastTime = Time.time;



        }
    }

}
