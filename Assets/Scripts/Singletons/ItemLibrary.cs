using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemRating
{
    Common, Rare, Epic, Legendary
}
public class ItemLibrary : Singleton<ItemLibrary>
{
    public const float    COMMON_PROBABILITY =  0.4f; // 40%
    public const float      RARE_PROBABILITY = 0.35f; // 35%
    public const float      EPIC_PROBABILITY =  0.2f; // 20%
    public const float LEGENDARY_PROBABILITY = 0.05f; //  5%

    [SerializeField]
    private Item[] Items;

    private Dictionary<ItemRating, List<Item>> mLibrary;

    private float[] mProbabilityArray;

    private void Awake()
    {
        mLibrary = mLibrary ?? new Dictionary<ItemRating, List<Item>>();

        mLibrary.Add(ItemRating.Common,    new List<Item>());
        mLibrary.Add(ItemRating.Rare,      new List<Item>());
        mLibrary.Add(ItemRating.Epic,      new List<Item>());
        mLibrary.Add(ItemRating.Legendary, new List<Item>());

        for (int i = 0; i < Items.Length; ++i)
        {
            mLibrary[Items[i].Rating].Add(Items[i]);
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
            sum += (mLibrary[(ItemRating)i].Count == 0) ? 0 : mProbabilityArray[i];

            if (sum <= probability)
            {
                ItemRating rating = (ItemRating)i;

                if (mLibrary[rating].Count != 0)
                {
                    int itemIndex = Random.Range(0, mLibrary[rating].Count);

                    getItem = mLibrary[rating][itemIndex];
                              mLibrary[rating].RemoveAt(itemIndex);
                }
            }
        }
        return getItem;
    }

    public Item GetRandomItem(ItemRating rating)
    {
        Item getItem = null;

        if (mLibrary[rating].Count != 0)
        {
            int itemIndex = Random.Range(0, mLibrary[rating].Count);

            getItem = mLibrary[rating][itemIndex];
            mLibrary[rating].RemoveAt(itemIndex);
        }

        return getItem;
    }
}
