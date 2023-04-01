using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////
///¡∂πŒ¿Õ ¿€æ˜
///ΩÃ±€≈Ê
/////////////////////////////////////////////////////////////////////
public class MySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (T)FindObjectOfType(typeof(T));
                //obj = (T)FindObjectOfType(typeof(T));
            }
            if (instance)
            {
                return instance;
            }

            return null;
        }
    }
}
