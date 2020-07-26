using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Finger : Singleton<Finger>
{
    private Item mCarryItem = null;

    private ChargeGauge mChargeGauge;

    private float mClickTime = 0;
    private const float PRESS_TIME = 0.8f;

    private void Awake()
    {
        transform.GetChild(0).TryGetComponent(out mChargeGauge);
    }

    public void SetCarryItem(Item item)
    {
        mCarryItem = item;
    }
    public void GetCarryItem(out Item item)
    {
        item = mCarryItem;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mChargeGauge.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mChargeGauge.transform.Translate(0, 0, 10);
        }
        if (Input.GetMouseButton(0))
        {
            if (mClickTime >= PRESS_TIME)
            {
                mChargeGauge.gameObject.SetActive(true);

                mChargeGauge.GaugeUp(0.8f);
            }
            else
            {
                mClickTime += Time.deltaTime;
            }
        }
        if (Input.GetMouseButtonUp(0) && mClickTime >= PRESS_TIME)
        {
            mChargeGauge.gameObject.SetActive(false);

            Player player = FindObjectOfType(typeof(Player)) as Player;

            player.mInventory.UseItem(ITEM_KEYWORD.CHARGE);

            Debug.Log($"Charge : {(int)(mChargeGauge.Charge*100)}%");

            mClickTime = 0;
        }
    }
}
