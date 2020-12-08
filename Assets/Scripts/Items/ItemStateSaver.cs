using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStateSaver : Singleton<ItemStateSaver>
{
    private Dictionary<ItemRating, List<Item>> _ItemLibDictionary;

    private Item[] _AccessoryCollection;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Discard()
    {
        Destroy(gameObject);
    }

    public void SaveAccessoryItem(Item item, int index)
    {
        if (_AccessoryCollection == null)
        {
            _AccessoryCollection = new Item[Inventory.AccessorySlotCount];
        }
        _AccessoryCollection[index] = item;
    }

    public Item LoadAccessoryItem(int index)
    {
        if (_AccessoryCollection != null)
        {
            return _AccessoryCollection[index];
        }
        return null;
    }

    public void SaveLibDictionary(Dictionary<ItemRating, List<Item>> itemLibDictionary)
    {
        _ItemLibDictionary = itemLibDictionary;
    }

    public bool LoadLibDictionary(out Dictionary<ItemRating, List<Item>> itemLibDictionary)
    {
        if (_ItemLibDictionary == null)
        {
            itemLibDictionary = new Dictionary<ItemRating, List<Item>>();

            return false;
        }
        else
        {
            itemLibDictionary = _ItemLibDictionary;

            return true;
        }
    }
}
