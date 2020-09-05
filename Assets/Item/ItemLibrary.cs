using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ITEM_RATING
{
    COMMON, RARE, EPIC, LEGENDARY
}
public class ItemLibrary : Singleton<ItemLibrary>
{
    public const float    COMMON_PROBABILITY =  0.4f; // 40%
    public const float      RARE_PROBABILITY = 0.35f; // 35%
    public const float      EPIC_PROBABILITY =  0.2f; // 20%
    public const float LEGENDARY_PROBABILITY = 0.05f; //  5%

    [SerializeField]
    private List<Item> mItems;

    private Dictionary<ITEM_RATING, List<Item>> mLibrary;

    private float[] ProbabilityArray;

    private void Awake()
    {
        mLibrary = new Dictionary<ITEM_RATING, List<Item>>();

        mLibrary.Add(ITEM_RATING.COMMON, new List<Item>());
        mLibrary.Add(ITEM_RATING.RARE, new List<Item>());
        mLibrary.Add(ITEM_RATING.EPIC, new List<Item>());
        mLibrary.Add(ITEM_RATING.LEGENDARY, new List<Item>());

        for (int i = 0; i < mItems.Count; ++i)
        {
            mLibrary[mItems[i].RATING].Add(mItems[i]);
        }
        ProbabilityArray = new float[4] 
        {
            COMMON_PROBABILITY, RARE_PROBABILITY, EPIC_PROBABILITY, LEGENDARY_PROBABILITY 
        };
    }

    public Item GetRandomItem()
    {
        ITEM_RATING returnRATING = ITEM_RATING.COMMON;

        float probability = Random.value;

        for (int i = 3; i >= 0; i--)
        {
            if (probability > 1 - ProbabilityArray[i])
            {
                returnRATING = (ITEM_RATING)i; break;
            }
        }
        for (int i = 0; i < 4; i++)
        {
            if (CanGetRatingItem(returnRATING, out Item returnItem)) {
                return returnItem;
            }
            else if (returnRATING.Equals(ITEM_RATING.COMMON)) {
                returnRATING = ITEM_RATING.LEGENDARY;
            }
            else returnRATING--;
        }
        return null;
        #region
        /*
            if (probability <= COMMON_PROBABILITY)
        {
            for (ITEM_RATING RATING = ITEM_RATING.COMMON; !CanGetRatingItem(RATING, out returnItem) && RATING < ITEM_RATING.LEGENDARY; RATING++) { }
        }

        else if (probability - COMMON_PROBABILITY <= RARE_PROBABILITY)
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

        else if (probability - COMMON_PROBABILITY - RARE_PROBABILITY <= EPIC_PROBABILITY)
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

        else if (probability - COMMON_PROBABILITY - RARE_PROBABILITY - EPIC_PROBABILITY <= LEGENDARY_PROBABILITY)
        {
            for (ITEM_RATING RATING = ITEM_RATING.LEGENDARY; !CanGetRatingItem(RATING, out returnItem) && RATING > ITEM_RATING.COMMON; RATING--) { }
        }
        return returnItem;
        */
        #endregion
    }

    private bool CanGetRatingItem(ITEM_RATING RATING, out Item getItem)
    {
        getItem = null;

        if (mLibrary[RATING].Count > 0)
        {
            int itemIndex = Random.Range(0, mLibrary[RATING].Count);

            getItem = mLibrary[RATING][itemIndex];
                      mLibrary[RATING].RemoveAt(itemIndex);
        }
        return getItem != null;
    }
}
