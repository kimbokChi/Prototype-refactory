using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemLibrary : Singleton<ItemLibrary>
{
    public const float COMMON_PROBABILITY = 40f;
    public const float RARE_PROBABILITY = 35f;
    public const float EPIC_PROBABILITY = 20f;
    public const float LEGENDARY_PROBABILITY = 5f;

    [SerializeField]
    private List<Item> mItems;

    private Dictionary<ITEM_DATA, Item> mLibrary;

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
