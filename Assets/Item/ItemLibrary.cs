using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemLibrary : Singleton<ItemLibrary>
{
    [SerializeField]
    private List<Item> mItems = new List<Item>();

    private Dictionary<ITEM_DATA, Item> mLibrary = new Dictionary<ITEM_DATA, Item>();



    private System.Random mRandom = new System.Random();


    private void Awake()
    {
        for(int i = 0; i < mItems.Count; ++i)
        {
            if (!mLibrary.ContainsKey(mItems[i].DATA))
            {
                mItems[i].Init();

                mLibrary.Add(mItems[i].DATA, mItems[i]);
            }
            else
            {
                Debug.LogWarning($"중복된 아이템이 존재합니다. 중복된 인덱스 : {i}");
            }
        }
    }

    public Item GetRandomItem()
    {
        if (mItems.Count > 0)
        {
            int index = mRandom.Next(0, mItems.Count);


            Item randomItem = mItems[index];

            mItems.Remove(mItems[index]);


            return randomItem;
        }
        return null;
    }
}
