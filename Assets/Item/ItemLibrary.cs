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
        int probability = Random.Range(1, 101);

        Item returnItem = null;

        if (probability < COMMON_PROBABILITY)
        {
            for (ITEM_RATING RATING = ITEM_RATING.COMMON; !CanGetRatingItem(RATING, out returnItem) && RATING < ITEM_RATING.LEGENDARY; RATING++) { }
        }

        else if (probability - COMMON_PROBABILITY < RARE_PROBABILITY)
        {
            if (!CanGetRatingItem(ITEM_RATING.RARE, out returnItem))
            {
                if (!CanGetRatingItem(ITEM_RATING.EPIC, out returnItem))
                {
                    if (!CanGetRatingItem(ITEM_RATING.LEGENDARY, out returnItem))
                    {
                        CanGetRatingItem(ITEM_RATING.COMMON, out returnItem);
                    }
                }
            }
        }

        else if (probability - COMMON_PROBABILITY - RARE_PROBABILITY < EPIC_PROBABILITY)
        {
            if (!CanGetRatingItem(ITEM_RATING.EPIC, out returnItem))
            {
                if (!CanGetRatingItem(ITEM_RATING.LEGENDARY, out returnItem))
                {
                    if (!CanGetRatingItem(ITEM_RATING.RARE, out returnItem))
                    {
                        CanGetRatingItem(ITEM_RATING.COMMON, out returnItem);
                    }
                }
            }
        }

        else if (probability - COMMON_PROBABILITY - RARE_PROBABILITY - EPIC_PROBABILITY < LEGENDARY_PROBABILITY)
        {
            for (ITEM_RATING RATING = ITEM_RATING.LEGENDARY; !CanGetRatingItem(RATING, out returnItem) && RATING > ITEM_RATING.COMMON; RATING--) { }
        }
        return returnItem;
    }

    private bool CanGetRatingItem(ITEM_RATING RATING, out Item getItem)
    {
        if (mLibrary[RATING].Count > 0)
        {
            int itemIndex = Random.Range(0, mLibrary[RATING].Count);

            getItem = mLibrary[RATING][itemIndex];
                      mLibrary[RATING].RemoveAt(itemIndex);
        }
        else getItem = null;

        return (getItem != null);
    }
}
