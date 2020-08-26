using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool<T> : MonoBehaviour
{
    private Stack<T> mIsntances;

    private Action<T> InstanceResetMethod;

    public void Init(Action<T> instanceResetMethod)
    {
        mIsntances = new Stack<T>();

        InstanceResetMethod = instanceResetMethod;
    }

    public void Add(T instance)
    {
        mIsntances.Push(instance);

        InstanceResetMethod.Invoke(instance);
    }

    public T Pop() => mIsntances.Pop();
}
