using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Area : MonoBehaviour
{
    [SerializeField]
    private Collider2D AreaCollider;

    [SerializeField]
    private string[] mSenseTags;

    public Collider2D GetCollider
    { get => AreaCollider; }

    private Action<GameObject> mEnterAction;
    private Action             mEmptyAction;

    private List<GameObject> mSenseList;

    public void SetScale(float halfScale)
    {
        if (AreaCollider.TryGetComponent(out CircleCollider2D circle))
        {
            circle.radius = halfScale;
        }
        else if(AreaCollider.TryGetComponent(out BoxCollider2D box))
        {
            box.size = Vector2.one * halfScale * 2;
        }
    }

    public void SetEnterAction(Action<GameObject> enterAction)
    {
        mEnterAction = enterAction;
    }
    public void SetEmptyAction(Action emptyAction)
    {
        mEmptyAction = emptyAction;
    }

    private void Reset()
    {
        TryGetComponent(out AreaCollider);
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

        if (mSenseList.Count.Equals(0)) mEmptyAction?.Invoke();
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

    public bool HasAny()
    {
        return mSenseList.Count > 0;
    }
    public bool HasThis(GameObject _this)
    {
        return mSenseList.Any(o => o.Equals(_this));
    }
    public Vector2 CloestTargetPos()
    {
        if (mSenseList.Count == 0)
        {
            return transform.position;
        }
        else
        {
            float Distance(Transform a)
            {
                return Mathf.Abs(a.position.x - transform.position.x);
            }
            GameObject cloestTarget = mSenseList.OrderBy(o => Distance(o.transform)).First();

            return cloestTarget.transform.position;
        }
    }
}
