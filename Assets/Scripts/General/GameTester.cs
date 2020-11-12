using UnityEngine;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

public class GameTester : MonoBehaviour { }

#if UNITY_EDITOR
[CustomEditor(typeof(GameTester))]
public class AddItem : Editor
{
    private Item mAddTarget;

    private string mKillAllEnemyExpectionName = "Scarecrow";

    private float mGameSpeed = 1f;

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

                var enemies = GameObject.FindGameObjectsWithTag("Enemy").Where(o => o.activeSelf).ToList();


                foreach (var enemy in enemies)
                {
                   if (enemy.name.Equals(mKillAllEnemyExpectionName)) continue;

                   if (enemy.TryGetComponent(out ICombatable combatable))
                   {
                        combatable.Damaged(float.MaxValue, player);
                   }
                }
            }
            //==================================================================
            GUILayout.Space(6f);

            GUILayout.Label("Game Control", EditorStyles.boldLabel);
            GUILayout.Label("Game Speed");
            mGameSpeed = GUILayout.HorizontalSlider(mGameSpeed, 0.1f, 2f);
            GUILayout.Space(14f);

            if (GUILayout.Button("Speed Reset", GUILayout.Height(20f))) {
                mGameSpeed = 1f;
            }
            Time.timeScale = mGameSpeed;
            GUILayout.Space(6f);
        }
    }
}
#endif