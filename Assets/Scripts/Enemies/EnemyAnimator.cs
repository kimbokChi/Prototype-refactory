using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAnim
{
    Idle, Move, Attack, Damaged, Death
}

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField]
    private Animator Animator;

    private int mControlKey;

    private void Reset()
    {
        TryGetComponent(out Animator);
    }

    public void Init()
    {
        mControlKey = Animator.GetParameter(0).nameHash;
    }

    public void ChangeState(EnemyAnim anim)
    {
        Animator.SetInteger(mControlKey, (int)anim);
    }
}
