using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path;
using UnityEngine;

public class StageEventLibrary : Singleton<StageEventLibrary>
{
    [SerializeField]
    private GameObject ItemBox;

    public event System.Action StageClearEvent;

    private void Awake()
    {
        StageClearEvent += CreateItemBox;
    }

    public void NotifyStageClear()
    {
        StageClearEvent?.Invoke();
    }

    private void CreateItemBox()
    {
        // 0 or 3 or 6
        int floorIndex = Random.Range(0, 3) * 3;

        Vector2 createPointMin = Castle.Instance.GetMovePoint((DIRECTION9)floorIndex);
        Vector2 createPointMax = Castle.Instance.GetMovePoint((DIRECTION9)floorIndex + 2);

        Vector2 createPoint = new Vector2
            (Random.Range(createPointMin.x, createPointMax.x), createPointMin.y + 0.5f);

        Instantiate(ItemBox, createPoint, Quaternion.identity);
    }
}
