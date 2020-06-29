using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public  static T  Instnace
    {
        get
        {
            if(mInstnace == null)
            {
                mInstnace = FindObjectOfType<T>() as T;

                if(mInstnace == null)
                {
                    mInstnace = new GameObject(typeof(T).ToString(), typeof(T)) as T;
                }
                DontDestroyOnLoad(mInstnace);
            }
            return mInstnace;
        }
    }
    private static T mInstnace;
}
