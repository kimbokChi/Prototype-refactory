using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class GameTester : MonoBehaviour { }

[CustomEditor(typeof(GameTester))]
public class AddItem : Editor
{
    private Item mAddTarget;

    private string mKillAllEnemyExpectionName = "Scarecrow";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            GUILayout.Space(6f);

            GUILayout.Label("Inventory Control", EditorStyles.boldLabel);

            GUILayout.Space(2f);
            GUILayout.Label("Target Item");
            mAddTarget = EditorGUILayout.ObjectField(mAddTarget, typeof(Item), true) as Item;

            if (GUILayout.Button("Set Weapon Item", GUILayout.Height(20f)))
            {
                Inventory.Instance.SetWeaponSlot(mAddTarget);
            }
            if (GUILayout.Button("Add Item", GUILayout.Height(20f)))
            {
                Inventory.Instance.AddItem(mAddTarget);
            }
            if (GUILayout.Button("Clear Inventory", GUILayout.Height(20f)))
            {
                Inventory.Instance.Clear();
            }
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            //==================================================================

            GUILayout.Space(6f);

            GUILayout.Label("Stage Control", EditorStyles.boldLabel);

            GUILayout.Space(2f);
            GUILayout.Label("Expection Enemy Name");
            mKillAllEnemyExpectionName = 
            GUILayout.TextField(mKillAllEnemyExpectionName);

            if (GUILayout.Button("Kill All Enemy", GUILayout.Height(20f)))
            {
                var player  = FindObjectOfType(typeof(Player)) as GameObject;

                var enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();


                foreach (var enemy in enemies)
                {
                   if (enemy.name.Equals(mKillAllEnemyExpectionName)) continue;

                   if (enemy.TryGetComponent(out ICombatable combatable))
                   {
                        combatable.Damaged(float.MaxValue, player);
                   }
                }
            }
        }
    }
}
