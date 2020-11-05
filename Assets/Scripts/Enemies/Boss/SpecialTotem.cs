using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialTotem : MonoBehaviour
{
    [SerializeField] private float TotemPlayTime;
    [SerializeField] private float TotemEffectPlayPoint;
    [SerializeField] private float TotemEffectPlayTime;

    [SerializeField] private GameObject Totem;
    [SerializeField] private GameObject TotemEffect;
    [SerializeField] private Area TotemSkillArea;

    private float DeltaTime
    {
        get => Time.deltaTime * Time.timeScale;
    }

    public void SetAreaEnterAction(Action<GameObject> action)
    {
        TotemSkillArea?.SetEnterAction(action);
    }

    public void CastSkill(Vector2 castPoint)
    {
        transform.position = castPoint;
        
        gameObject.SetActive(true);
        StartCoroutine(SkillAction());
    }

    private IEnumerator SkillAction()
    {
        bool isPlayingTotemEffect = false;

        Totem.SetActive(true);

        for (float i = 0f; i < TotemPlayTime; i += DeltaTime)
        {
            if (!isPlayingTotemEffect) {
                if (i >= TotemEffectPlayPoint)
                {
                    isPlayingTotemEffect = true;

                    TotemEffect.SetActive(true);
                }
            }
            yield return null;
        }
        Totem.SetActive(false);

        for (float i = 0f; i < TotemEffectPlayTime; i += DeltaTime)
        {
            yield return null;
        }
        TotemEffect.SetActive(false);
    }
}
