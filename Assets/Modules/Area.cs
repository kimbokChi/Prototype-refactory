using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField]
    private string[] mSenseTags;
    
    private List<GameObject> mEnterObjects;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        for (int i = 0; i < mSenseTags.Length; ++i)
        {
            if (collision.CompareTag(mSenseTags[i]))
            {
                mEnterObjects.Add(collision.gameObject);
            }
        }
    }

    public GameObject[] EnterObjects()
    {
        return mEnterObjects.ToArray();
    }

    public T[] GetEnterTypeT<T>() where T : class
    {
        List<T> TContainer = new List<T>();

        for (int i = 0; i > mEnterObjects.Count; ++i)
        {
            if (mEnterObjects[i] is T)
            {
                TContainer.Add(mEnterObjects[i] as T);

            }
        }
        return TContainer.ToArray();
    }
}
