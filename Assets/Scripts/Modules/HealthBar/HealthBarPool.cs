using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarPool : Singleton<HealthBarPool>
{
    [SerializeField] private uint HoldingCount; [Space()]

    [SerializeField] private HealthBar OrignHealthBar;
    [SerializeField] private Transform WorldCanvasTransform;

    private Queue<HealthBar> mPool;

    private Dictionary<Transform, HealthBar> mUserList;

    private void Awake()
    {
        mPool = new Queue<HealthBar>();

        mUserList = new Dictionary<Transform, HealthBar>();

        for (int i = 0; i < HoldingCount; i++)
        {
            HealthBar healthBar = Instantiate(OrignHealthBar, WorldCanvasTransform);
            UnUsingHealthBar(healthBar);
        }
    }

    public void UsingHealthBar(float offsetY, Transform master, AbilityTable abilityTable)
    {
        if (mPool.Count == 0) {
            mPool.Enqueue(Instantiate(OrignHealthBar, WorldCanvasTransform));
        }
        HealthBar healthBar = mPool.Dequeue();
        mUserList.Add(master, healthBar);

        healthBar.Init(Vector3.up * offsetY, master, abilityTable);
        healthBar.gameObject.SetActive(true);
    }

    public void UnUsingHealthBar(Transform master)
    {
        if (mUserList.ContainsKey(master))
        {
            HealthBar healthBar = mUserList[master];
                                  mUserList.Remove(master);

            UnUsingHealthBar(healthBar);
        }
    }

    private void UnUsingHealthBar(HealthBar healthBar)
    {
        mPool.Enqueue(healthBar);
                      healthBar.gameObject.SetActive(false);
    }
}
