using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Area : MonoBehaviour
{
    [SerializeField]
    private string[] mSenseTags;
    
    private List<GameObject> mSenseList;

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
                mSenseList.Add(collision.gameObject);

                break;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        mSenseList.Remove(collision.gameObject);
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
