using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEventLibrary : Singleton<StageEventLibrary>
{
    public event Action<Vector2[][]> StageClear;

    [SerializeField] private GameObject ItemAndItemBox;

    private void Awake()
    {
        if (StageClear == null)
        {
            StageClear = delegate (Vector2[][] LMovePoints) { };
        }
        StageClear += CreateItemBox;
    }

    private void CreateItemBox(Vector2[][] LMovePoints)
    {
        Vector2[] selectMovePoint = LMovePoints[UnityEngine.Random.Range(0, 3)];

        float xPoint = UnityEngine.Random.Range(selectMovePoint[0].x, selectMovePoint[2].x);

        float yPoint = selectMovePoint[UnityEngine.Random.Range(0, 3)].y + 0.63f;

        Instantiate(ItemAndItemBox, new Vector2(xPoint, yPoint), Quaternion.identity).SetActive(true);
    }

    public void StageClearEvent(Vector2[] TopMovePoint, Vector2[] MidMovePoint, Vector2[] BotMovePoint)
    {
        Vector2[][] LMovePoints = new Vector2[3][] { TopMovePoint, MidMovePoint, BotMovePoint };

        StageClear.Invoke(LMovePoints);
    }
}
