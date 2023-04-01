using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///������ �۾�
///���忡 �ݶ��̴� ������ �ʿ��Ҷ� �����Ǵ� �ݶ��̴�������Ʈ
///Box�� Sphere �ΰ����� �ִ�.
/////////////////////////////////////////////////////////////////////
public class BoxColl : Colliders
{
    private void Awake()
    {
        VirtualStart();
    }

    //private void Start()
    //{
    //    VirtualStart();
    //}

    public override void VirtualStart()
    {
        base.VirtualStart();
        colltype = CharEnumTypes.eCollType.BoxColl;
        Mycollider = GetComponent<BoxCollider>();
    }

    public BoxCollider GetCollider()
    {
        return Mycollider as BoxCollider;
    }

    public override void SetSize(Vector3 size)
    {
        BoxCollider col = Mycollider as BoxCollider;
        col.size = size;
    }
}
