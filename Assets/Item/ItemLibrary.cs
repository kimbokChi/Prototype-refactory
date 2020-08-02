using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemLibrary : Singleton<ItemLibrary>
{
    [SerializeField]
    private List<Item> mItems;

    private Dictionary<ITEM_DATA, Item> mLibrary;

    public delegate void UseMoveBegin();
    public event UseMoveBegin WUseMoveBegin;
    public event UseMoveBegin AUseMoveBegin;

    public delegate void UseMoveEnd();
    public event UseMoveEnd WUseMoveEnd;
    public event UseMoveEnd AUseMoveEnd;

    public delegate void UseStruck();
    public event UseStruck WUseStruck;
    public event UseStruck AUseStruck;

    public delegate void UseBeDamaged(ref float damage, GameObject attacker, GameObject victim);
    public event UseBeDamaged WUseBeDamaged;
    public event UseBeDamaged AUseBeDamaged;

    public delegate void UseEnter();
    public event UseEnter WUseEnter;
    public event UseEnter AUseEnter;

    public delegate void UseCharge(float charge);
    public event UseCharge WUseCharge;
    public event UseCharge AUseCharge;

    private void Awake()
    {
        mLibrary = new Dictionary<ITEM_DATA, Item>();

        for (int i = 0; i < mItems.Count; ++i)
        {
            if (!mLibrary.ContainsKey(mItems[i].DATA))
            {
                mItems[i].Init();

                mLibrary.Add(mItems[i].DATA, mItems[i]);
            }
            else
            {
                Debug.LogError($"중복된 아이템이 존재합니다. 중복된 인덱스 : {i}");
            }
        }
    }

    public Item GetRandomItem()
    {
        if (mItems.Count > 0)
        {
            int index = Random.Range(0, mItems.Count);


            Item randomItem = mItems[index];

            mItems.Remove(mItems[index]);


            return randomItem;
        }
        return null;
    }
}
