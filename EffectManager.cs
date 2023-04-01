using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///일반공격, 스킬공격등을 위해
///이펙트를 공간상에 생성할 수 있는 기능이 필요해서 제작 하였음
/////////////////////////////////////////////////////////////////////


public class EffectManager : MySingleton<EffectManager>
{

    public Dictionary<int, GameObject> CurEffects = new Dictionary<int, GameObject>();

    public List<GameObject> Effects = new List<GameObject>();

    public CorTimeCounter timer = new CorTimeCounter();

    public IEnumerator cor;

    public MyDotween.Dotween dotween = new MyDotween.Dotween();

    //한번만 실행하고 사라진다.
    public GameObject SpawnEffectOneLoop(string adressableAdress, Vector3 pos, Quaternion rotation)
    {
        GameObject copyeffect = InstantiateEffect(adressableAdress);
        ParticleSystem[] particles = null;
        particles = copyeffect.GetComponentsInChildren<ParticleSystem>();

        copyeffect.transform.position = pos;
        copyeffect.transform.rotation = rotation;

        float maxduration = 0;
        foreach(ParticleSystem particle in particles)
        {
            if(particle.main.duration>maxduration)
            {
                maxduration = particle.main.duration;
            }
        }

        cor = timer.Cor_TimeCounter(maxduration, GameObject.Destroy, copyeffect);
        StartCoroutine(cor);

        return copyeffect;
    }

    //한번만 재생하고 사라진다.
    public GameObject SpawnEffectOneLoop(string adressableAdress, Transform posrot)
    {
        GameObject copyeffect = InstantiateEffect(adressableAdress);
        ParticleSystem[] particles = null;
        particles = copyeffect.GetComponentsInChildren<ParticleSystem>();

        copyeffect.transform.position = posrot.position;
        copyeffect.transform.rotation = posrot.rotation;

        float maxduration = 0;
        foreach (ParticleSystem particle in particles)
        {
            if (particle.main.duration > maxduration)
            {
                maxduration = particle.main.duration;
            }
        }

        cor = timer.Cor_TimeCounter(maxduration, GameObject.Destroy, copyeffect);
        StartCoroutine(cor);

        return copyeffect;
    }

    //일정 주기로 재시작
    public GameObject SpawnEffectLooping(string adressableAdress, Vector3 pos, Quaternion rotation, float _duration, float destroyTime)
    {
        GameObject copyeffect = InstantiateEffect(adressableAdress);
        ParticleSystem[] particles = null;
        particles = copyeffect.GetComponentsInChildren<ParticleSystem>();

        copyeffect.transform.position = pos;
        copyeffect.transform.rotation = rotation;

        cor = timer.Cor_TimeCounterLoop<GameObject>(destroyTime, GameObject.Destroy, Restart, 3, copyeffect, copyeffect);
        StartCoroutine(cor);

        return copyeffect;
    }

    public void Restart(GameObject effect)
    {
        ParticleSystem[] particles = null;
        particles = effect.GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem particle in particles)
        {
            particle.Play();
        }
    }


    public void SetLoop(GameObject effect, bool flag)
    {

    }

    



    //기본 스폰
    public GameObject InstantiateEffect(string adressableAdress)
    {
        GameObject copy = ResourceCreateDeleteManager.Instance.InstantiateObj<GameObject>(adressableAdress);
        copy.transform.parent = null;

        return copy;
    }

    //사라질 시간
    public GameObject InstantiateEffect(string adressableAdress, float DestroyTime)
    {
        GameObject copy = InstantiateEffect(adressableAdress);
        //copy.transform.parent = null;
        //CurEffects.Add(copy.GetInstanceID(), copy);
        cor = timer.Cor_TimeCounter<string, GameObject>(DestroyTime, DestroyEffect, adressableAdress, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치
    public GameObject InstantiateEffect(string adressableAdress, Vector3 pos, float DestroyTime = 1.0f)
    {
        GameObject copy = InstantiateEffect(adressableAdress);
        copy.transform.position = pos;
        cor = timer.Cor_TimeCounter<string, GameObject>(DestroyTime, DestroyEffect, adressableAdress, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치, 회전, 파괴시간
    public GameObject InstantiateEffect(string adressableAdress, Vector3 pos, Quaternion rotation, float DestroyTime=1.0f)
    {
        GameObject copy = InstantiateEffect(adressableAdress);
        copy.transform.position = pos;
        copy.transform.rotation = rotation;
        cor = timer.Cor_TimeCounter<string, GameObject>(DestroyTime, DestroyEffect, adressableAdress, copy);
        StartCoroutine(cor);
        return copy;
    }



    //transform, 파괴시간
    public GameObject InstantiateEffect(string adressableAdress, Transform posrot, float DestroyTime = 1.0f)
    {
        GameObject copy = InstantiateEffect(adressableAdress);
        copy.transform.position = posrot.position;
        copy.transform.rotation = posrot.rotation;
        cor = timer.Cor_TimeCounter<string, GameObject>(DestroyTime, DestroyEffect, adressableAdress, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치, 파괴시간, 부모transform
    public GameObject InstantiateEffect(string adressableAdress, Vector3 pos, float DestroyTime, Transform parent)
    {
        GameObject copy = InstantiateEffect(adressableAdress);
        copy.transform.position = pos;
        copy.transform.parent = parent;
        cor = timer.Cor_TimeCounter<string, GameObject>(DestroyTime, DestroyEffect, adressableAdress, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치, 회전, 파괴시간, 부모transform
    public GameObject InstantiateEffect(string adressableAdress, Vector3 pos, Quaternion rotation, float DestroyTime, Transform parent)
    {
        GameObject copy = InstantiateEffect(adressableAdress);
        copy.transform.position = pos;
        copy.transform.parent = parent;
        copy.transform.rotation = rotation;
        cor = timer.Cor_TimeCounter<string, GameObject>(DestroyTime, DestroyEffect, adressableAdress, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위지, 크기, 회전, 파괴시간, 부모transform
    public GameObject InstantiateEffect(string adressableAdress, Vector3 pos, Vector3 size, Quaternion rotation, float DestroyTime, Transform parent)
    {
        GameObject copy = InstantiateEffect(adressableAdress);
        copy.transform.position = pos;
        copy.transform.localScale = size;
        copy.transform.rotation = rotation;

        copy.transform.parent = parent;
        cor = timer.Cor_TimeCounter<string, GameObject>(DestroyTime, DestroyEffect, adressableAdress, copy);
        StartCoroutine(cor);
        return copy;
    }

    //해당 이펙트의 부모transform을 설정 null 가능
    public void SetParent(GameObject effectobj, Transform parent)
    {
        GameObject effect;
        CurEffects.TryGetValue(effectobj.GetInstanceID(), out effect);

        if (effect == null)
        {
            //Debug.Log($"{this.name} not exist effect");
            return;
        }

        effect.transform.parent = parent;
    }

    public void DestroyEffect(string adressableAdress, GameObject obj)
    {
        ResourceCreateDeleteManager.Instance.DestroyObj<GameObject>(adressableAdress, obj);
        //GameMG.Instance.Resource.Destroy<GameObject>(obj);
    }

    public void DoMove(GameObject effect, Vector3 dest, float duration, MyDotween.Dotween.Ease ease = MyDotween.Dotween.Ease.Linear)
    {
        dotween.SetEase(ease);
        dotween.DoMove(effect, dest, duration);
    }

    public void DoMove(MyDotween.Sequence sequence)
    {
        sequence.Start();
    }



    #region 이전버전
    //기본 스폰
    public GameObject InstantiateEffect(GameObject effect)
    {
        GameObject copy = GameObject.Instantiate(effect);
        
        //GameObject copy = ResourceCreateDeleteManager.Instance.InstantiateObj<GameObject>(adressableAdress);
        copy.transform.parent = null;
        CurEffects.Add(copy.GetInstanceID(), copy);
        return copy;
    }

    //사라질 시간
    public GameObject InstantiateEffect(GameObject effect, float DestroyTime)
    {
        GameObject copy = InstantiateEffect(effect);
        copy.transform.parent = null;
        CurEffects.Add(copy.GetInstanceID(), copy);
        cor = timer.Cor_TimeCounter<GameObject>(DestroyTime, DestroyEffect, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치
    public GameObject InstantiateEffect(GameObject effect, Vector3 pos, float DestroyTime = 1.0f)
    {
        GameObject copy = InstantiateEffect(effect);
        copy.transform.position = pos;
        cor = timer.Cor_TimeCounter<GameObject>(DestroyTime, DestroyEffect, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치, 회전, 파괴시간
    public GameObject InstantiateEffect(GameObject effect, Vector3 pos, Quaternion rotation, float DestroyTime = 1.0f)
    {
        GameObject copy = InstantiateEffect(effect);
        copy.transform.position = pos;
        copy.transform.rotation = rotation;
        cor = timer.Cor_TimeCounter<GameObject>(DestroyTime, DestroyEffect, copy);
        StartCoroutine(cor);
        return copy;
    }

    //transform, 파괴시간
    public GameObject InstantiateEffect(GameObject effect, Transform posrot, float DestroyTime = 1.0f)
    {
        GameObject copy = InstantiateEffect(effect);
        copy.transform.position = posrot.position;
        copy.transform.rotation = posrot.rotation;
        cor = timer.Cor_TimeCounter<GameObject>(DestroyTime, DestroyEffect, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치, 파괴시간, 부모transform
    public GameObject InstantiateEffect(GameObject effect, Vector3 pos, float DestroyTime, Transform parent)
    {
        GameObject copy = InstantiateEffect(effect);
        copy.transform.position = pos;
        copy.transform.parent = parent;
        cor = timer.Cor_TimeCounter<GameObject>(DestroyTime, DestroyEffect,  copy);
        StartCoroutine(cor);
        return copy;
    }

    //위치, 회전, 파괴시간, 부모transform
    public GameObject InstantiateEffect(GameObject effect, Vector3 pos, Quaternion rotation, float DestroyTime, Transform parent)
    {
        GameObject copy = InstantiateEffect(effect);
        copy.transform.position = pos;
        copy.transform.parent = parent;
        copy.transform.rotation = rotation;
        cor = timer.Cor_TimeCounter<GameObject>(DestroyTime, DestroyEffect, copy);
        StartCoroutine(cor);
        return copy;
    }

    //위지, 크기, 회전, 파괴시간, 부모transform
    public GameObject InstantiateEffect(GameObject effect, Vector3 pos, Vector3 size, Quaternion rotation, float DestroyTime, Transform parent)
    {
        GameObject copy = InstantiateEffect(effect);
        copy.transform.position = pos;
        copy.transform.localScale = size;
        copy.transform.rotation = rotation;

        copy.transform.parent = parent;
        cor = timer.Cor_TimeCounter<GameObject>(DestroyTime, DestroyEffect, copy);
        StartCoroutine(cor);
        return copy;
    }

    public void DestroyEffect(GameObject obj)
    {
        GameObject.Destroy(obj);
    }

    #endregion
}