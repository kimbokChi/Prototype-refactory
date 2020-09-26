using UnityEngine;
using UnityEditor;

public class GameTester : MonoBehaviour { }

[CustomEditor(typeof(GameTester))]
public class AddItem : Editor
{
    private Item mAddTarget;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            GUILayout.Space(6f);

            GUILayout.Label("Select Item", EditorStyles.boldLabel);
            mAddTarget = EditorGUILayout.ObjectField(mAddTarget, typeof(Item), true) as Item;

            if (GUILayout.Button("Add Item", GUILayout.Height(20f)))
            {
                Inventory.Instance.AddItem(mAddTarget);
            }
            if (GUILayout.Button("Clear Inventory", GUILayout.Height(20f)))
            {
                Inventory.Instance.Clear();
            }
        }
    }
}
