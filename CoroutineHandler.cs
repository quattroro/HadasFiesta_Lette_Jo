using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///조민익 작업
///MonoBehaviour를 상속받지 않아도 코루틴을 사용하게 해주기 위한 함수
/////////////////////////////////////////////////////////////////////

public class CoroutineHandler : MonoBehaviour
{
    private static MonoBehaviour monoinstance;

    [RuntimeInitializeOnLoadMethod]
    private static void Initializer()
    {
        monoinstance = new GameObject("CoroutimeHandler").AddComponent<CoroutineHandler>();
        DontDestroyOnLoad(monoinstance.gameObject);
    }

    public static Coroutine Start_Coroutine(IEnumerator cor)
    {
        return monoinstance.StartCoroutine(cor);
    }


    public static void Stop_Coroutine(Coroutine cor)
    {
        if (monoinstance != null)
        {
            monoinstance.StopCoroutine(cor);
        }
    }


}
