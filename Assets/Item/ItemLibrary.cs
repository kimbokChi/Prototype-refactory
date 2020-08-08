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

    private Dictionary<ITEM_RATING, List<Item>> mLibrary;

    private void Awake()
    {
        mLibrary = new Dictionary<ITEM_RATING, List<Item>>();

        mLibrary.Add(ITEM_RATING.COMMON, new List<Item>());
        mLibrary.Add(ITEM_RATING.RARE, new List<Item>());
        mLibrary.Add(ITEM_RATING.EPIC, new List<Item>());
        mLibrary.Add(ITEM_RATING.LEGENDARY, new List<Item>());

        for (int i = 0; i < mItems.Count; ++i)
        {
            switch (mItems[i].RATING)
            {
                case ITEM_RATING.COMMON:
                    mLibrary[ITEM_RATING.COMMON].Add(mItems[i]);
                    break;

                case ITEM_RATING.RARE:
                    mLibrary[ITEM_RATING.RARE].Add(mItems[i]);
                    break;

                case ITEM_RATING.EPIC:
                    mLibrary[ITEM_RATING.EPIC].Add(mItems[i]);
                    break;

                case ITEM_RATING.LEGENDARY:
                    mLibrary[ITEM_RATING.LEGENDARY].Add(mItems[i]);
                    break;
            }
        }
    }

    public Item GetRandomItem()
    {
        int probability = Random.Range(0, 100);

        int returnIndex;

        Item returnItem = null;

        if (probability < COMMON_PROBABILITY && mLibrary[ITEM_RATING.COMMON].Count > 0)
        {
            returnIndex = Random.Range(0, mLibrary[ITEM_RATING.COMMON].Count);

            returnItem = mLibrary[ITEM_RATING.COMMON][returnIndex];
                         mLibrary[ITEM_RATING.COMMON].RemoveAt(returnIndex);
        }

        else if ((probability - COMMON_PROBABILITY < RARE_PROBABILITY || mLibrary[ITEM_RATING.COMMON].Count <= 0) && mLibrary[ITEM_RATING.RARE].Count > 0)
        {
            returnIndex = Random.Range(0, mLibrary[ITEM_RATING.RARE].Count);

            returnItem = mLibrary[ITEM_RATING.RARE][returnIndex];
                         mLibrary[ITEM_RATING.RARE].RemoveAt(returnIndex);
        }

        else if ((probability - COMMON_PROBABILITY - RARE_PROBABILITY < EPIC_PROBABILITY || mLibrary[ITEM_RATING.RARE].Count <= 0) && mLibrary[ITEM_RATING.EPIC].Count > 0)
        {
            returnIndex = Random.Range(0, mLibrary[ITEM_RATING.EPIC].Count);

            returnItem = mLibrary[ITEM_RATING.EPIC][returnIndex];
                         mLibrary[ITEM_RATING.EPIC].RemoveAt(returnIndex);
        }

        else if (mLibrary[ITEM_RATING.LEGENDARY].Count > 0 && mLibrary[ITEM_RATING.EPIC].Count <= 0)
        {
            returnIndex = Random.Range(0, mLibrary[ITEM_RATING.LEGENDARY].Count);

            returnItem = mLibrary[ITEM_RATING.LEGENDARY][returnIndex];
                         mLibrary[ITEM_RATING.LEGENDARY].RemoveAt(returnIndex);
        }
        return returnItem;
    }

}
