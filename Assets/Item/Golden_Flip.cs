using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Flip : MonoBehaviour, IItem
{
    void IItem.UseItem(ITEM_KEYWORD keyword)
    {
        Debug.Log(keyword.ToString());
    }
}
