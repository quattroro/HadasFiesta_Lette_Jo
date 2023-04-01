using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///캐릭터의 다양한 기능들을 컴포넌트 패턴으로 제작
///이동, 공격, 방어 등등의 기능이 존재
///추상클래스로 구현
/////////////////////////////////////////////////////////////////////


public abstract class BaseComponent : MonoBehaviour
{
    private CharEnumTypes.eComponentTypes comtype;

    public CharEnumTypes.eComponentTypes p_comtype
    {
        get
        {
            return comtype;
        }
        set
        {
            comtype = value;
        }
    }
    public abstract void InitComtype();

    public virtual void Init()
    {
        InitComtype();
    }


    public void Awake()
    {
        InitComtype();
    }

}
