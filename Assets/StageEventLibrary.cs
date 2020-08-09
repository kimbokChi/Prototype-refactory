using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEventLibrary : Singleton<StageEventLibrary>
{
    public event Action<Vector2[], Vector2[], Vector2[]> StageClear;

    [SerializeField] private GameObject ItemAndItemBox;

    private void Awake()
    {
        if (StageClear == null)
        {
            StageClear = delegate (Vector2[] T, Vector2[] M, Vector2[] B) { };
        }
        StageClear += CreateItemBox;
    }

    private void CreateItemBox(Vector2[] TopMovePoint, Vector2[] MidMovePoint, Vector2[] BotMovePoint)
    {
        Vector2[] selectMovePoint = new Vector2[3];

        switch (UnityEngine.Random.Range(0,3))
        {
            case 0:
                selectMovePoint = TopMovePoint;
                break;
            case 1:
                selectMovePoint = MidMovePoint;
                break;
            case 2:
                selectMovePoint = BotMovePoint;
                break;
        }

        float xPoint = UnityEngine.Random.Range(selectMovePoint[0].x, selectMovePoint[2].x);

        float yPoint = selectMovePoint[UnityEngine.Random.Range(0, 3)].y + 0.63f;

        Instantiate(ItemAndItemBox, new Vector2(xPoint, yPoint), Quaternion.identity).SetActive(true);
    }

    public void StageClearEvent(Vector2[] TmovePoints, Vector2[] MmovePoints, Vector2[] BmovePoints )
    {
        StageClear.Invoke(TmovePoints, MmovePoints, BmovePoints);
    }
}
