using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTotem : MonoBehaviour
{
    private Action mSkill;

    public void SetSkill(Action skill)
    {
        mSkill = skill;
    }

    public void CastSkill()
    {
        mSkill?.Invoke();
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
