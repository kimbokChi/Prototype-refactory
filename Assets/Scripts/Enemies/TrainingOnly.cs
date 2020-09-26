using System.Collections;
using UnityEngine;

public class TrainingOnly : EnemyBase
{
    public override void CastBuff(BUFF buffType, IEnumerator castedBuff) => StartCoroutine(castedBuff);

    public override void Damaged(float damage, GameObject attacker) => Debug.Log($"Damage : {damage}");

    public override void IInit() { }

    public override bool IsActive() => false;

    public override void IUpdate() { }

    public override void PlayerEnter(MESSAGE message, Player enterPlayer) { }

    public override void PlayerExit(MESSAGE message) { }

    public override GameObject ThisObject() => gameObject;
}
