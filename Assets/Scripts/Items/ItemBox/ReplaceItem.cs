using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemBox))]
public class ReplaceItem : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemBox itemBox = (ItemBox)target;
        if (GUILayout.Button("Replace Item"))
        {
            itemBox.ReplaceItem();
        }
    }
}