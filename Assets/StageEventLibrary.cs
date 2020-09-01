using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEventLibrary : Singleton<StageEventLibrary>
{
    #region READ
    /// <summary>
    /// 스테이지를(층)을 클리어한 이후에 발동할 이벤트를 저장합니다.
    /// <para>첫번째 인자는 클리어한 층의 배열이며, 두번째 인자는 증감치를 고려한 이벤트가 발생할 확률 입니다.</para>
    /// </summary>
    #endregion
    public event Action<Vector2[][], float> StageClear;

    [SerializeField] private GameObject ItemAndItemBox;

    [SerializeField][Range(-100f, 100f)] private float mEventProbabilty;

    private void Awake()
    {
        if (StageClear == null)
        {
            StageClear = delegate (Vector2[][] LMovePoints, float probabilty) { };
        }
        StageClear += CreateItemBox;
    }

    private void CreateItemBox(Vector2[][] LMovePoints, float probabilty)
    {
        Vector2[] selectMovePoint = LMovePoints[UnityEngine.Random.Range(0, 3)];

        float xPoint = UnityEngine.Random.Range(selectMovePoint[0].x, selectMovePoint[2].x);

        float yPoint = selectMovePoint[UnityEngine.Random.Range(0, 3)].y + 0.63f;

        Instantiate(ItemAndItemBox, new Vector2(xPoint, yPoint), Quaternion.identity).SetActive(true);
    }

    public void StageClearEvent(Vector2[] TopMovePoint, Vector2[] MidMovePoint, Vector2[] BotMovePoint)
    {
        Vector2[][] LMovePoints = new Vector2[3][] { TopMovePoint, MidMovePoint, BotMovePoint };

        var stageClearEvent = StageClear.GetInvocationList();


        for (int i = 0; i < stageClearEvent.Length; ++i)
        {
            float probabilty = UnityEngine.Random.Range(0f, 100f) + mEventProbabilty;

            stageClearEvent[i].DynamicInvoke(LMovePoints, probabilty);
        }
    }
}
