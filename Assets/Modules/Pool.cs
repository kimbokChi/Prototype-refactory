﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour where T : MonoBehaviour
{
    private T mOriginInstance;

    private Stack<T> mIsntances;

    private Action<T> InstanceResetMethod;
    private Func<T, bool> InstanceFilter;

    public void Init(T origin, Action<T> instanceResetMethod = null, Func<T, bool> instanceFilter = null)
    {
        mOriginInstance = origin;

        mIsntances = new Stack<T>();

        InstanceResetMethod = instanceResetMethod;

        InstanceFilter = instanceFilter;
    }

    public bool OnInstanceFilter(out T[] instances)
    {
        if (InstanceFilter == null)
        {
            instances = null; return false;

        }
        List<T> filteringThings = new List<T>();

        foreach (var item in mIsntances)
        {
            if (InstanceFilter.Invoke(item))
            {
                filteringThings.Add(item);
            }
        }
        instances = filteringThings.ToArray();

        return instances.Length > 0;
    }

    public void Add(T instance)
    {
        mIsntances.Push(instance);

        if (InstanceResetMethod != null)
        {
            InstanceResetMethod.Invoke(instance);
        }      
    }

    public T CreatePop()
    {
        mIsntances.Push(Instantiate(mOriginInstance));

        InstanceResetMethod(mIsntances.Peek());

        return mIsntances.Peek();
    }

    public bool Pop(out T instance)
    {
        if (mIsntances.Count == 0)
        {
            instance = default(T); return false;
        }
        instance = mIsntances.Pop();

        return true;
    }
}