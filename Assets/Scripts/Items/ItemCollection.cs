using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollection : MonoBehaviour
{
    [Header("CollectionBox Section")]
    [SerializeField] private GameObject CollectionBox;
    [SerializeField] private float IntervalY;
    [SerializeField] private float IntervalX;

    [Header("Unlocked Section")]
    [SerializeField] private Transform Unlocked;
    [SerializeField] private Item[] UnlockedItems;

    [Header("Locked Section")]
    [SerializeField] private Transform Locked;
    [SerializeField] private Item[] LockedItems;

    private void Awake()
    {
        var collection = CreateCollectionBox(LockedItems.Length, Locked);

        for (int i = 0; i < LockedItems.Length; i++)
        {
            if (collection[i].GetChild(0).TryGetComponent(out Image image))
            {

                image.sprite = LockedItems[i].Sprite;
            }
        }
        collection = CreateCollectionBox(UnlockedItems.Length, Unlocked);

        for (int i = 0; i < UnlockedItems.Length; i++)
        {
            if (collection[i].GetChild(0).TryGetComponent(out Image image))
            {

                image.sprite = UnlockedItems[i].Sprite;
            }
        }
    }

    private List<Transform> CreateCollectionBox(int itemListLength, Transform parent)
    {
        var list = new List<Transform>();
        int line = (itemListLength / 3) + 1;


        for (int i = 0; i < line; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                var instance = Instantiate(CollectionBox, parent);
                    instance.transform.localPosition = new Vector2(IntervalX * j, IntervalY * i - 100f);

                list.Add(instance.transform);
            }
        }
        return list;
    }
}
