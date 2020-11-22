﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ItemRating
{
    Common, Rare, Epic, Legendary
}
public class ItemLibrary : Singleton<ItemLibrary>
{
    [Header("Item Probablities")]
    [SerializeField][Range(0f, 1f)] private float Common;
    [SerializeField][Range(0f, 1f)] private float Rare;
    [SerializeField][Range(0f, 1f)] private float Epic;
    [SerializeField][Range(0f, 1f)] private float Legendary;

    [Header("Registered Items")]
    [SerializeField] private Item[] Items;

    private Dictionary<ItemRating, List<Item>> mLibrary;

    private float[] mProbabilityArray;

    [ContextMenu("Probablity Check")]
    private void ProbablityCheck()
    {
        float sum = Common + Rare + Epic + Legendary;

        Debug.Log($"Probablity Sum : {sum}");

        if (sum != 1f)
            Debug.LogError($"아이템 등장 확률의 총합은 1이 될 것을 권장합니다.");
    }

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
            Common, Rare, Epic, Legendary
        };
    }

    public Item GetRandomItem()
    {
        float sum = 0f;
        float probability = Random.value;

        int invokeCount = 0;

        foreach (var lib in mLibrary)
        {
            // 현재 등급의 아이템에서 반환할 수 있는 아이템이 없다면
            if (lib.Value.Count == 0)
            {
                int division = 
                    mLibrary.ToList().Count(o => o.Value.Count > 0);

                float additive =
                    mProbabilityArray[invokeCount] / division;

                for (int i = 0; i < 4; i++)
                {
                    // 반환할 수 있는 나머지 등급들의 확률을 보정한다
                    if (mLibrary[(ItemRating)i].Count > 0)
                    {
                        mProbabilityArray[i] += additive;
                    }
                }
                // 그리고 mLibrary에서 제거
                mLibrary.Remove(lib.Key);
                continue;
            }
            sum += mProbabilityArray[invokeCount];

            // 반환할 아이템의 등급이 정해졌다면
            if (probability <= sum)
            {
                int index = Random.Range(0, lib.Value.Count);

                var item = lib.Value[index];
                           lib.Value.RemoveAt(index);

                // 그 등급의 아이템 중 무작위 아이템 반환
                return item;
            }
            invokeCount++;
        }
        return null;
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
