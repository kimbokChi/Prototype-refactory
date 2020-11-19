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
    private const string JsonTableName = "ItemData";
    public ItemRating Rating
    { get; private set; }

    public  Dictionary<ItemStat, float>  Table
    {
        get
        {
            if (_Table == null)
            {
                Init();
            }
            return _Table;
        }
    }
    private Dictionary<ItemStat, float> _Table;

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
        Rating = (ItemRating)Enum.Parse(typeof(ItemRating), JsonString("Rating"));
    }
}
