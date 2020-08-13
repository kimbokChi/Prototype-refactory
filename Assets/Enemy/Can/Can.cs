using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Can : EnemyBase
{
    public override void Damaged(float damage, GameObject attacker, out GameObject victim)
    {
        victim = gameObject;
    }

    public override void IInit()
    {
    }

    public override void IUpdate()
    {
    }

    public override void PlayerEnter(Player enterPlayer)
    {
        mPlayer = enterPlayer;
    }

    public override void PlayerExit()
    {
        mPlayer = null;
    }

    public override GameObject ThisObject()
    {
        return gameObject;
    }

    public override bool IsActive()
    {
        return gameObject.activeSelf;
    }
}
