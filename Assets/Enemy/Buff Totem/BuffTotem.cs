using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTotem : MonoBehaviour, IObject
{
    [SerializeField] private Area mSenseArae;

    public void IInit()
    {
        throw new System.NotImplementedException();
    }

    public bool IsActive()
    {
        return gameObject.activeSelf;
    }

    public void IUpdate()
    {
    }

    public void PlayerEnter(Player enterPlayer) { }
    public void PlayerExit() { }

    public GameObject ThisObject() => gameObject;
}
