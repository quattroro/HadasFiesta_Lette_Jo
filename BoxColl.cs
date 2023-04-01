using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///월드에 콜라이더 생성이 필요할때 생성되는 콜라이더오브젝트
///Box와 Sphere 두가지가 있다.
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
