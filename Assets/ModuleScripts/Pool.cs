using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> where T : MonoBehaviour
{
    private T mOriginInstance;

    private Stack<T> mInThePool;

    private List<T> mOutThePool;

    private Action<T> PopMethod;
    private Action<T> AddMethod;

    private Func<T, bool> CanReturnOfPool;

    public void Init(T origin, Action<T> popMethod, Action<T> addMethod, Func<T, bool> canReturnOfPool)
    {
        mOriginInstance = origin;

        mInThePool = new Stack<T>();
        mOutThePool = new List<T>();

        PopMethod = popMethod;
        AddMethod = addMethod;

        CanReturnOfPool = canReturnOfPool;
    }

    public void Update()
    {
        if (CanReturnOfPool != null)
        {
            var returnO = mInThePool.Where(o => CanReturnOfPool(o)).ToList();

            returnO.ForEach(o => Add(o));
            returnO.ForEach(o => mOutThePool.Remove(o));
        }
    }

    public void Add(T instance)
    {
        mInThePool.Push(instance);

        AddMethod?.Invoke(instance);
    }

    public T Pop()
    {
        T instance;

        if (mInThePool.Count == 0)
        {
            mInThePool.Push(instance = MonoBehaviour.Instantiate(mOriginInstance));
        }
        mOutThePool.Add(instance = mInThePool.Pop());

        PopMethod.Invoke(instance);

        return instance;
    }
}
