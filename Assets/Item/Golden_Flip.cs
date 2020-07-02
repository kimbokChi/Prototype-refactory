using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golden_Flip : MonoBehaviour, IItem
{
    public void UseItem(ITEM_KEYWORD keyword)
    {
        Debug.Log(keyword.ToString());
    }
}
