using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "ItemStatObject", menuName = "ScriptableObject/ItemStatObject")]
public class ItemStatObject : ScriptableObject
{
    private const string JsonTableName = "ItemData";

    private Dictionary<ItemStat, float> _Table;

    [SerializeField] private string JsonLabelName;

    [Header("Data Section")]
    [SerializeField] private float _Range;
    [SerializeField] private float _AttackPower;
    [SerializeField] private float _BeginAttackDelay;
    [SerializeField] private float _AfterAttackDelay;

    [SerializeField] private ItemRating _Rating;
    [SerializeField] private string _NameKR;
    [SerializeField] private int _Cost;

    public float Range => _Range;
    public float AttackPower => _AttackPower;
    public float BeginAttackDelay => _BeginAttackDelay;
    public float AfterAttackDelay => _AfterAttackDelay;
    public ItemRating Rating => _Rating;
    public string NameKR => _NameKR;
    public int Cost => _Cost;

    public void Init()
    {
        _Table = new Dictionary<ItemStat, float>();

        string JsonString(string s)
        {
            return DataUtil.GetDataValue(JsonTableName, "ID", JsonLabelName, s);
        }
        _Range = float.Parse(JsonString("Range"));
        _AttackPower = float.Parse(JsonString("AttackPower"));
        _BeginAttackDelay = float.Parse(JsonString("Begin_AttackDelay"));
        _AfterAttackDelay = float.Parse(JsonString("After_AttackDelay"));

        _Rating = (ItemRating)System.Enum.Parse(typeof(ItemRating), JsonString("Rating"));
        _NameKR = JsonString("NameKR");

        _Cost = int.Parse(JsonString("Cost"));
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ItemStatObject))]
public class ItemStatObjectControler : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(15f);

        if (GUILayout.Button("Set Data From Json", GUILayout.Height(30f)))
        {
            if (Application.isEditor)
            {
                (target as ItemStatObject).Init();
            }
        }
    }
}
#endif