using System;
using System.Collections.Generic;
using UnityEngine;

public enum ItemStat
{
    AttackPower, Range,
    Begin_AttackDelay, 
    After_AttackDelay,
    End
}
public class ItemStatTable : MonoBehaviour
{
    public float this[ItemStat stat]
    {
        get {
            if (!_IsAlreadyInit)
            {
                Init();
            }
            return _Table[stat];
        }
    }
    public  ItemRating  Rating
    {
        get
        {
            if (!_IsAlreadyInit)
            {
                Init();
            }
            return _Rating;
        }
    }
    private ItemRating _Rating;

    public  string  NameKR
    {
        get
        {
            if (!_IsAlreadyInit)
            {
                Init();
            }
            return _NameKR;
        }
    }
    private string _NameKR;

    public  int  Cost
    {
        get
        {
            if (!_IsAlreadyInit)
            {
                Init();
            }
            return _Cost;
        }
    }
    private int _Cost;

    private const string JsonTableName = "ItemData";

    private Dictionary<ItemStat, float> _Table;

    private bool _IsAlreadyInit = false;

    [TextArea(1, 1)]
    [SerializeField] private string JsonLabelName;

    private void Init()
    {
        _Table = new Dictionary<ItemStat, float>();

        string JsonString(string s)
        {
            return DataUtil.GetDataValue(JsonTableName, "ID", JsonLabelName, s);
        }

        for (ItemStat i = 0; i < ItemStat.End; i++)
        {
            _Table[i] = float.Parse(JsonString(i.ToString()));
        }
        _Rating = (ItemRating)Enum.Parse(typeof(ItemRating), JsonString("Rating"));
        _NameKR = JsonString("NameKR");

        _Cost = int.Parse(JsonString("Cost"));

        _IsAlreadyInit = true;
    }
}
