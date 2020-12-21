using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStore : MonoBehaviour
{
    [SerializeField] private RegisteredItem RegisteredItem;
    [SerializeField] private ItemShowBlock[] ItemBlocks;

    private bool _IsAlreadyInit = false;

    private void Start()
    {
        if (!_IsAlreadyInit)
        {
            var list = ItemLibrary.Instance.GetUnlockedItemListForTest();

            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, list.Count);

                ItemBlocks[i].SetBlock(list[index]);
                                       list.RemoveAt(index);
            }
            _IsAlreadyInit = true;
        }
    }
    public void Purchace()
    {

    }
}
