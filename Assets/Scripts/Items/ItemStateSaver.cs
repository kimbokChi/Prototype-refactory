﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ItemStateSaver : Singleton<ItemStateSaver>
{
    [SerializeField] private RegisteredItem RegisteredItem;

    private Dictionary<ItemRating, List<Item>> _ItemLibDictionary;

    private List<Item> _UnlockedItemList;

    private Item[] _AccessoryCollection;
    private Item[] _ContainerCollection;

    private Type _WeaponItemType;

    private void Awake()
    {
        _UnlockedItemList = RegisteredItem.UnlockedItemList;

        if (FindObjectsOfType(typeof(ItemStateSaver)).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Discard()
    {
        Destroy(gameObject);
    }

    public void SaveSlotItem(SlotType slotType, Item item, int index)
    {
        switch (slotType)
        {
            case SlotType.Weapon:
                {
                    _WeaponItemType = item.GetType();
                }
                break;

            case SlotType.Container:
                if (_ContainerCollection == null)
                {
                    _ContainerCollection = new Item[Inventory.ContainerSlotCount];
                }
                _ContainerCollection[index] = item;
                break;

            case SlotType.Accessory:
                if (_AccessoryCollection == null)
                {
                    _AccessoryCollection = new Item[Inventory.AccessorySlotCount];
                }
                _AccessoryCollection[index] = item;
                break;
        }
    }
    public Item LoadSlotItem(SlotType slotType, int index)
    {
        switch (slotType)
        {
            case SlotType.Weapon:
                {
                    if (_WeaponItemType != null)
                    {

                        return Instantiate(_UnlockedItemList.First(o => o.GetType().Equals(_WeaponItemType)));
                    }
                }
                return null;

            case SlotType.Container:
                if (_ContainerCollection != null)
                {
                    return _ContainerCollection[index];
                }
                break;

            case SlotType.Accessory:
                if (_AccessoryCollection != null)
                {
                    return _AccessoryCollection[index];
                }
                break;
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
