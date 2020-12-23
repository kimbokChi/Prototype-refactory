using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollection : MonoBehaviour
{
    [SerializeField] private Transform UnlockedSection;
    [SerializeField] private Transform LockedSection;

    [Header("CollectionBox Section")]
    [SerializeField] private GameObject CollectionBox;
    [SerializeField] private float IntervalY;
    [SerializeField] private float IntervalX;

    [Header("Registered Items")]
    [SerializeField] private RegisteredItem RegisteredItem;

    private void Start()
    {
        var lockedList = ItemLibrary.Instance.GetLockedItemListForTest();
        var collection = CreateCollectionBox(lockedList.Count, LockedSection);

        for (int i = 0; i < lockedList.Count; i++)
        {
            if (collection[i].GetChild(0).TryGetComponent(out Image image))
            {

                image.sprite = lockedList[i].Sprite;
            }
        }
        var unlockedList = ItemLibrary.Instance.GetUnlockedItemListForTest();
        collection = CreateCollectionBox(unlockedList.Count, UnlockedSection);

        for (int i = 0; i < unlockedList.Count; i++)
        {
            if (collection[i].GetChild(0).TryGetComponent(out Image image))
            {

                image.sprite = unlockedList[i].Sprite;
            }
        }
    }

    private List<Transform> CreateCollectionBox(int itemListLength, Transform parent)
    {
        var list = new List<Transform>();
        int line = Mathf.Max((itemListLength / 3) + 1, 2);


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