using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{
    private T mOriginInstance;

    private Stack<T> mInOfPool;

    private List<T> mOutOfPool;

    private Action<T> InstanceResetMethod;
    private Func<T, bool> ReturnToPoolMethod;

    public void Init(T origin, Action<T> instanceResetMethod = null, Func<T, bool> returnToPoolMethod = null)
    {
        mOriginInstance = origin;

        mInOfPool = new Stack<T>();

        InstanceResetMethod = instanceResetMethod;

        ReturnToPoolMethod = returnToPoolMethod;
    }

    public void Update()
    {
        if (ReturnToPoolMethod != null)
        {
            int repetition = mOutOfPool.Count;

            for (int i = 0; i < repetition; i++)
            {
                if (ReturnToPoolMethod(mOutOfPool[i]))
                {
                    Add(mOutOfPool[i]);

                    mOutOfPool.RemoveAt(i);
                }
            }
        }
    }

    public void Add(T instance)
    {
        mInOfPool.Push(instance);

        if (InstanceResetMethod != null)
        {
            InstanceResetMethod.Invoke(instance);
        }      
    }

    public T Pop()
    {
        if (mInOfPool.Count == 0)
        {
            T instance;

            InstanceResetMethod(instance = MonoBehaviour.Instantiate(mOriginInstance));

            return instance;
        }
        return mInOfPool.Pop();
    }
}
