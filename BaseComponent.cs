using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///������ �۾�
///ĳ������ �پ��� ��ɵ��� ������Ʈ �������� ����
///�̵�, ����, ��� ����� ����� ����
///�߻�Ŭ������ ����
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
