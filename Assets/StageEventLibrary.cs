using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEventLibrary : Singleton<StageEventLibrary>
{
    public event Action<Vector2[], Vector2[], Vector2[]> StageClear;

    private void Awake()
    {
        if (StageClear == null)
        {
            StageClear = delegate (Vector2[] T, Vector2[] M, Vector2[] B) { };
        }
    }

    public void StageClearEvent(Vector2[] TmovePoints, Vector2[] MmovePoints, Vector2[] BmovePoints )
    {
        StageClear.Invoke(TmovePoints, MmovePoints, BmovePoints);
    }
}
