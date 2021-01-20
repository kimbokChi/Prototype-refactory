using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemStateSaver : Singleton<ItemStateSaver>
{
    [SerializeField] private RegisteredItem RegisteredItem;

    private Dictionary<ItemRating, List<Item>> _ItemLibDictionary;

    private List<Item> _UnlockedItemList = null;
    private List<Item>   _LockedItemList = null;

    private ItemID[] _AccessoryIDArray;
    private ItemID[] _ContainerIDArray;

    private ItemID _WeaponItemID;

    private void Awake()
    {
        if (FindObjectsOfType(typeof(ItemStateSaver)).Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void  SetUnlockedItem(List<int> idList)
    {
        if (_UnlockedItemList == null) {
            _UnlockedItemList = new List<Item>();
        }   _UnlockedItemList.Clear();
        
        if (_LockedItemList == null) {
            _LockedItemList = new List<Item>();
        }   _LockedItemList.Clear();

        int registerCount = RegisteredItem.Count();
        for (int i = 0; i < registerCount; i++)
        {
            Item instance = RegisteredItem.GetItemInstance((ItemID)i);

            if (idList.Contains(i))
            {
                _UnlockedItemList.Add(instance);
            }
            else
            {
                _LockedItemList.Add(instance);
            }
        }
    }
    public void SetInventoryItem(List<int> idList)
    {

    }
    public void ItemUnlock(params ItemID[] ids)
    {
        for (int i = 0; i < ids.Length; i++)
        {
            for (int j = 0; j < _LockedItemList.Count; j++)
            {
                if (_LockedItemList[j].ID == ids[i])
                {
                    _UnlockedItemList.Add(_LockedItemList[j]);
                    _LockedItemList.RemoveAt(j);
                }
            }
        }
    }

    public void SaveSlotItem(SlotType slotType, Item item, int index)
    {
        SlotItemArrayInit();

        switch (slotType)
        {
            case SlotType.Weapon:
                {
                    if (item == null)
                    {
                        _WeaponItemID = ItemID.None;
                    }
                    else 
                        _WeaponItemID = item.ID;
                }
                break;

            case SlotType.Container:
                {
                    if (item == null)
                    {
                        _ContainerIDArray[index] = ItemID.None;
                    }
                    else
                        _ContainerIDArray[index] = item.ID;
                }
                break;

            case SlotType.Accessory:
                {
                    if (item == null)
                    {
                        _AccessoryIDArray[index] = ItemID.None;
                    }
                    else
                        _AccessoryIDArray[index] = item.ID;
                }
                break;
        }
    }
    public Item LoadSlotItem(SlotType slotType, int index)
    {
        SlotItemArrayInit();
        ItemID loadID = ItemID.None;

        switch (slotType)
        {
            case SlotType.Weapon:
                loadID = _WeaponItemID;
                break;

            case SlotType.Container:
                loadID = _ContainerIDArray[index];
                break;

            case SlotType.Accessory:
                loadID = _AccessoryIDArray[index];
                break;
        }
        return RegisteredItem.GetItemInstance(loadID);
    }
    private void SlotItemArrayInit()
    {
        if (_ContainerIDArray == null)
        {
            _ContainerIDArray = new ItemID[Inventory.ContainerSlotCount];
        }
        if (_AccessoryIDArray == null)
        {
            _AccessoryIDArray = new ItemID[Inventory.ContainerSlotCount];
        }
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


    public void SaveUnlockedItemListForTest(List<Item> unlockedList)
    {
        _UnlockedItemList = unlockedList;
    }
    public bool LoadUnlockedItemListForTest(out List<Item> unlockedList)
    {
        if (_UnlockedItemList == null)
        {
            unlockedList = new List<Item>();
            return false;
        }
        unlockedList = _UnlockedItemList;
        return true;
    }

    public void SaveLockedItemListForTest(List<Item> lockedList)
    {
        _LockedItemList = lockedList;
    }
    public bool LoadLockedItemListForTest(out List<Item> lockedList)
    {
        if (_LockedItemList == null)
        {
            lockedList = new List<Item>();
            return false;
        }
        lockedList = _LockedItemList;
        return true;
    }
}
