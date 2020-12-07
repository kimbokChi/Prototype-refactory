using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemStateSaver : Singleton<ItemStateSaver>
{
    private Dictionary<ItemRating, List<Item>> _ItemLibDictionary;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Discard()
    {
        Destroy(gameObject);
    }

    public void SaveLibDictionary(Dictionary<ItemRating, List<Item>> itemLibDictionary)
    {
        _ItemLibDictionary = itemLibDictionary;
    }

    public void LoadLibDictionary(out Dictionary<ItemRating, List<Item>> itemLibDictionary)
    {
        if (_ItemLibDictionary == null)
        {
            itemLibDictionary = new Dictionary<ItemRating, List<Item>>();
        }
        else
            itemLibDictionary = _ItemLibDictionary;
    }
}
