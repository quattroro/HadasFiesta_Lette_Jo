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
///
public class SphereColl : Colliders
{
    private void Awake()
    {
        VirtualStart();
    }
    private void Start()
    {
        VirtualStart();
    }

    public override void VirtualStart()
    {
        base.VirtualStart();
        colltype = CharEnumTypes.eCollType.SphereColl;
        Mycollider = GetComponent<SphereCollider>();
    }

    public SphereCollider GetCollider()
    {
        return Mycollider as SphereCollider;
    }

    public override void SetRadious(float radius)
    {
        SphereCollider col = Mycollider as SphereCollider;
        col.radius = radius;
        
    }


}
