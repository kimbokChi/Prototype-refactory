using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Area : MonoBehaviour
{
    [SerializeField]
    private string[] mSenseTags;

    private Action<GameObject> mEnterAction;
    private Action             mEmptyAction;

    private List<GameObject> mSenseList;

    public void SetEnterAction(Action<GameObject> enterAction)
    {
        mEnterAction = enterAction;
    }
    public void SetEmptyAction(Action emptyAction)
    {
        mEmptyAction = emptyAction;
    }

    private void Awake()
    {
        mSenseList = mSenseList ?? new List<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mSenseTags.Length; ++i)
        {
            if (collision.CompareTag(mSenseTags[i]))
            {
                mEnterAction?.Invoke(collision.gameObject);

                mSenseList.Add(collision.gameObject);

                break;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        mSenseList.Remove(collision.gameObject);

        if (mSenseList.Count.Equals(0)) mEmptyAction.Invoke();
    }
    public bool TryEnterTypeT<T>(out T enterObject) where T : class
    {
        enterObject = null;

        if (mSenseList.Count > 0)
        {
            if (mSenseList[0].TryGetComponent(out T instance)) {

                enterObject = instance;
            }
        }
        return enterObject != null;
    }

    public T[] GetEnterTypeT<T>() where T : class
    {
        List<T> TContainer = new List<T>();

        for (int i = 0; i < mSenseList.Count; ++i)
        {
            if (mSenseList[i].TryGetComponent(out T Instance))
            {
                TContainer.Add(Instance);

            }
        }
        return TContainer.ToArray();
    }
}
