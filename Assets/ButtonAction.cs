using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAction : MonoBehaviour
{
    [SerializeField]
    private GameObject SwitchingObject;

    public void ActiveSwitching()
    {
        SwitchingObject.SetActive(!SwitchingObject.activeSelf);
    }
}
