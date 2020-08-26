﻿using System;
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
        mOutOfPool = new List<T>();

        InstanceResetMethod = instanceResetMethod;

        ReturnToPoolMethod = returnToPoolMethod;
    }

    public void Update()
    {
        if (ReturnToPoolMethod != null)
        {
            for (int i = 0; i < mOutOfPool.Count; i++)
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
        T instance;

        if (mInOfPool.Count == 0)
        {
            mInOfPool.Push(instance = MonoBehaviour.Instantiate(mOriginInstance));

            InstanceResetMethod(instance);
        }
        mOutOfPool.Add(instance = mInOfPool.Pop());

        return instance;
    }
}
