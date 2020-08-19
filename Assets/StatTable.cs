using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatTable
{
    public float  MoveSpeed
    {
        get =>  mMoveSpeed;
        set => mIMoveSpeed = value - MoveSpeed;
    }
    public float IMoveSpeed
    {
        get { return mIMoveSpeed; }
    }

    [SerializeField] private float  mMoveSpeed;
    [SerializeField] private float mIMoveSpeed;
}
