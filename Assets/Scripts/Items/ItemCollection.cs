using System.Linq;
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
    [SerializeField] private Transform  Unlocked;
    [SerializeField] private List<Item> UnlockedItems;

    [Header("Locked Section")]
    [SerializeField] private Transform  Locked;
    [SerializeField] private List<Item> LockedItems;

    private void Awake()
    {
        var collection = CreateCollectionBox(LockedItems.Count, Locked);

        for (int i = 0; i < LockedItems.Count; i++)
        {
            if (collection[i].GetChild(0).TryGetComponent(out Image image))
            {

                image.sprite = LockedItems[i].Sprite;
            }
        }
        collection = CreateCollectionBox(UnlockedItems.Count, Unlocked);

        for (int i = 0; i < UnlockedItems.Count; i++)
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

    public List<Item> UnlockedItemList() {

        return UnlockedItems;
    }
}

[CreateAssetMenu(fileName = "RegisteredItem", menuName = "ScriptableObject/RegisteredItem")]
public class RegisteredItem : ScriptableObject
{
    [Space()]
    [SerializeField] private List<Item> _UnlockedItemList;

    [Space()]
    [SerializeField] private List<Item>   _LockedItemList;

    public List<Item> UnlockedItemList
    { get => _UnlockedItemList; }
    public List<Item> LockedItemList
    { get => _LockedItemList; }

    public List<Item> ItemListAll()
    {
        return new List<Item>(_LockedItemList.Union(_UnlockedItemList));
    }
}