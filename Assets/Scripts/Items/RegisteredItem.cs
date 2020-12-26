using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.Log("Origin : " + _UnlockedItemList.Count);
            Debug.Log("Clone : " + _UnlockedItemCloneList.Count);

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