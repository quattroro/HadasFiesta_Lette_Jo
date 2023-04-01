using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;




/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///게임 실행중 생성되고 파괴되는 모든 리소스들의 생성과 파괴를 담당합니다.
///Addressable Asset System을 사용하여 리소스들을 관리 합니다.
///오브젝트 풀링을 구현하고 오브젝트 풀링기능을 사용할 수 있도록 구현했습니다.
/////////////////////////////////////////////////////////////////////



public class ResourceCreateDeleteManager : Singleton<ResourceCreateDeleteManager>
{
    MyObjectPool.ObjectPoolManager poolManager = new MyObjectPool.ObjectPoolManager();

    Dictionary<string, List<GameObject>> objlist;

    //어드레서블로 로드 & 생성
    public T InstantiateObj<T>(string adressableName) where T:class
    {
        //일단 해당
        var temp = Addressables.LoadAssetAsync<GameObject>(adressableName);
        GameObject result = temp.WaitForCompletion();

        if(result==null)
        {
            Debug.LogError("어드레서블 로드 오류" + adressableName + "존재하지 않음");
            return default(T);
        }

        T resulttype = result.GetComponent<T>();

        if(poolManager.IsPooling(adressableName))//풀링을 하고 있는 객체면 풀링에서 꺼내서 주고
        {
            return poolManager.GetObject<T>(adressableName);
        }
        else//아니면 그냥 생성해준다.
        {
            temp = Addressables.InstantiateAsync(adressableName);
            result = temp.WaitForCompletion();

            if (typeof(T) == typeof(GameObject))
                return result as T;
            else
                return result.GetComponent<T>();
             
        }

    }

    public T LoadObjInfo<T>(string adressableName) where T : class
    {
        //일단 해당
        var temp = Addressables.LoadAssetAsync<GameObject>(adressableName);
        GameObject result = temp.WaitForCompletion();

        if (typeof(T) == typeof(GameObject))
            return result as T;
        else
            return result.GetComponent<T>();
    }

    public void DestroyObj<T>(string adressableName, GameObject obj)
    {
        if (obj == null)
            return;

        if (poolManager.IsPooling(adressableName))//풀링을 하고 있는 객체면 풀링에서 꺼내서 삭제
        {
            poolManager.ReturnObject(adressableName, obj);
        }
        else
        {
            Addressables.ReleaseInstance(obj);
        }
    }

    public void RegistPoolManager<T>(string _adressablename)
    {
        poolManager.CreatePool<T>(_adressablename);
    }

}

namespace MyObjectPool
{
    //해당 타입의 풀들을 만들어서 관리한다.
    public class ObjectPoolManager
    {
        public Dictionary<string, ObjectPool> PoolDic = new Dictionary<string, ObjectPool>();

      

        public bool IsPooling(string adressableName)
        {
            return PoolDic.ContainsKey(adressableName);
        }

        //어드레서블 네임으로 관리
        public void CreatePool<T>(string adressableName, int poolsize = 10)
        {
            ObjectPool pool = null;

            //이미 해당 타입의 풀이 있는지 확인하고 없으면 만들어 준다.
            PoolDic.TryGetValue(adressableName, out pool);

            if (pool == null)
            {
                pool = new ObjectPool(adressableName, typeof(T), poolsize);
                PoolDic.Add(adressableName, pool);
            }

        }

  

        //네임으로 관리
        public T GetObject<T>(string adressableName) where T : class
        {
            ObjectPool pool = null;
            PoolDic.TryGetValue(adressableName, out pool);

            if (pool != null)
            {
                if (typeof(T) == typeof(GameObject))
                    return pool.GetObj() as T;
                else
                    return pool.GetObj().GetComponent<T>();
            }

            Debug.LogError("존재하지 않는 타입");
            return default(T);
        }

        //네임으로 관리
        public void ReturnObject(string adressableName, GameObject obj)
        {
            if (obj == null)
                return;

            ObjectPool pool = null;
            bool flag = PoolDic.ContainsKey(adressableName);
            if (flag)
            {
                PoolDic.TryGetValue(adressableName, out pool);
                pool.ReturnObj(obj);
            }
        }

    }




    //풀은 게임오브젝트로 관리
    //객체는 기본적으로 10개 생성
    public class ObjectPool
    {
        //T _poolObj;
        string _adressableName;
        System.Type _type;
        public Stack<GameObject> _stack;
        int _poolSize = 0;
        Transform _parent;


        public ObjectPool(string adressableName, System.Type type, int poolsize)
        {
            _stack = new Stack<GameObject>();
            //System.Type _type = System.Type.GetType(_Name);
            _type = type;
            _adressableName = adressableName;
            _poolSize = poolsize;


            CreateObj(_poolSize);

        }


        public void CreateObj(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var temp = Addressables.InstantiateAsync(_adressableName);
                var result = temp.WaitForCompletion();
                result.SetActive(false);
                _stack.Push(result);
            }
        }


        public GameObject GetObj()
        {
            GameObject temp = null;

            if (_stack.Count > 0)
                temp = _stack.Pop();

            if (temp == null)
            {
                CreateObj(1);
                temp = _stack.Pop();
            }
            temp.SetActive(true);
            temp.transform.SetParent(null);

            return temp;
        }

        public void ReturnObj(GameObject obj)
        {
            if (obj == null)
                return;

            if (_stack.Contains(obj))
                return;

            obj.SetActive(false);
            _stack.Push(obj);
        }


    }
}
