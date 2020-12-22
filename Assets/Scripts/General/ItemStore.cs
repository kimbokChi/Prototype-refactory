using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemStore : MonoBehaviour
{
    [SerializeField] private RegisteredItem RegisteredItem;
    [SerializeField] private ItemShowBlock[] ItemBlocks;

    private bool _IsAlreadyInit = false;
    private ItemShowBlock _SelectedBlock;

    private void Start()
    {
        if (!_IsAlreadyInit)
        {
            var list = ItemLibrary.Instance.GetUnlockedItemListForTest().ToList();

            for (int i = 0; i < 3; i++)
            {
                int index = Random.Range(0, list.Count);

                ItemBlocks[i].SetBlock(list[index]);
                                       list.RemoveAt(index);
            }
            _IsAlreadyInit = true;
        }
    }
    public void BlockSelect(ItemShowBlock block)
    {
        _SelectedBlock = block;
    }
    public void Purchace()
    {
        if (_SelectedBlock != null)
        {
            if (MoneyManager.Instance.SubtractMoney(_SelectedBlock.ItemCost))
            {
                var item = Instantiate(_SelectedBlock.ContainItem);
                    item.transform.position = new Vector2(-10f, 0f);

                Inventory.Instance.AddItem(item);
                item.transform.parent = ItemStateSaver.Instance.transform;
            }
        }
    }
}
