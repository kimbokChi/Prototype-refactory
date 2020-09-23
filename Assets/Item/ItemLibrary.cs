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
    private Item[] Items;

    private Dictionary<ITEM_RATING, List<Item>> mLibrary;

    private float[] mProbabilityArray;

    private void Awake()
    {
        mLibrary = mLibrary ?? new Dictionary<ITEM_RATING, List<Item>>();

        mLibrary.Add(ITEM_RATING.COMMON,    new List<Item>());
        mLibrary.Add(ITEM_RATING.RARE,      new List<Item>());
        mLibrary.Add(ITEM_RATING.EPIC,      new List<Item>());
        mLibrary.Add(ITEM_RATING.LEGENDARY, new List<Item>());

        for (int i = 0; i < Items.Length; ++i)
        {
            mLibrary[Items[i].RATING].Add(Items[i]);
        }
        mProbabilityArray = new float[4] 
        {
            COMMON_PROBABILITY, RARE_PROBABILITY, EPIC_PROBABILITY, LEGENDARY_PROBABILITY 
        };
    }

    public Item GetRandomItem()
    {
        Item getItem = null;

        float probability = Random.value;

        float sum = 0f;

        for (int i = 0; i < 4; i++)
        {
            sum += (mLibrary[(ITEM_RATING)i].Count == 0) ? 0 : mProbabilityArray[i];

            if (sum <= probability)
            {
                ITEM_RATING rating = (ITEM_RATING)i;

                int itemIndex = Random.Range(0, mLibrary[rating].Count);

                getItem = mLibrary[rating][itemIndex];
                          mLibrary[rating].RemoveAt(itemIndex);

            }
        }
        return getItem;
    }
}
