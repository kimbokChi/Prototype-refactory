using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBombTotem : MonoBehaviour
{
    public Action<SBombTotem> CastOverAction;

    [SerializeField] private GameObject TotemUser;
    [SerializeField] private GameObject SpecialTotem;

    [Header("Skill Casting")]
    [SerializeField] private float SkillCastingPoint;
    [SerializeField] private float SkillCastingOffset;
    [SerializeField] private float SkillAnimationTime;

    [Header("Special Bomb")]
    [SerializeField] private float Damage;
    [SerializeField] private Area BombArea;

    public void Init()
    {
        void GiveDamage(GameObject target)
        {
            if (target.TryGetComponent(out ICombatable combatable))
            {
                combatable.Damaged(Damage, TotemUser);
            }
        }
        BombArea.SetEnterAction(GiveDamage);
    }
    public void Cast(Vector2 castPoint)
    {
        transform.position = castPoint;
        SpecialTotem.SetActive(true);

        StartCoroutine(SkillCasting());
    }
    private IEnumerator SkillCasting()
    {
        yield return new 
            WaitForSeconds(SkillCastingPoint);

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(SkillCastingOffset);

            BombArea.gameObject.SetActive(true);
            yield return new WaitForSeconds(SkillAnimationTime);
        }
        CastOverAction?.Invoke(this);
    }
}
