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

[CreateAssetMenu(fileName = "RegisteredItem", menuName = "ScriptableObject/RegisteredItem")]
public class RegisteredItem : ScriptableObject
{
    [Space()]
    [SerializeField] private List<Item> _UnlockedItemList;
                     private List<Item> _UnlockedItemCloneList;
    private bool _HasUnlockedListInit;

    [Space()]
    [SerializeField] private List<Item> _LockedItemList;
                     private List<Item> _LockedItemCloneList;
    private bool _HasLockedListInit;

    public List<Item> UnlockedItemList
    {
        get
        {
            if (!_HasUnlockedListInit)
            {
                _UnlockedItemCloneList = _UnlockedItemList.ToList();

                _HasUnlockedListInit = true;
            }
            return _UnlockedItemCloneList;
        }
    }
    public List<Item> LockedItemList
    {
        get
        {
            if (!_HasLockedListInit)
            {
                _LockedItemCloneList = _LockedItemList.ToList();

                _HasLockedListInit = true;
            }
            return _LockedItemCloneList;
        }
    }

    public List<Item> ItemListAll()
    {
        return new List<Item>(_LockedItemList.Union(_UnlockedItemList));
    }
}