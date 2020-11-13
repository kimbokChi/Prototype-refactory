using System;
using System.Linq;
using System.Collections;
using UnityEngine;

public class SBombTotemSkill : MonoBehaviour
{
    public Action<SBombTotemSkill> CastOverAction;

    [SerializeField] 
    private SBombTotem   SBombTotem;
    private SBombTotem[] SBombTotems;

    private Player mPlayer;

    public void Init(Player player)
    {
        SBombTotems = new SBombTotem[3];

        for (int i = 0; i < SBombTotems.Length; i++)
        {
            SBombTotems[i] = Instantiate(SBombTotem);
            SBombTotems[i].Init();
        }
        mPlayer = player;
    }
    public void Cast()
    {
        StartCoroutine(SkillCasting());
    }

    private IEnumerator SkillCasting()
    {
        Vector2 CastPoint()
        {
            float playerY = Castle.Instance.GetMovePoint(mPlayer.GetDIRECTION9()).y + 1.1f;

            return new Vector2(mPlayer.transform.position.x, playerY);
        }
        SBombTotems[0].Cast(CastPoint());

        for (int i = 1; i < 3; i++)
        {
            yield return new WaitForSeconds(0.3f);

            SBombTotems[i].Cast(CastPoint());
        }
        yield return new WaitUntil(() => SBombTotems.All(o => o.IsOver));

        CastOverAction?.Invoke(this);
    }
}
