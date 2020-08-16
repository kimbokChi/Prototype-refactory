using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Finger : Singleton<Finger>
{
    private const float PRESS_TIME = 0.8f;

    private float DeltaTime => Time.deltaTime * Time.timeScale;

    [SerializeField]
    private ChargeGauge mChargeGauge;

    private Item mCarryItem;

    private float mCurPressTime;

    private bool mIsGaugeBreak;

    private void Awake()
    {
        mCurPressTime = 0f;
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
            if (mCurPressTime >= PRESS_TIME)
            {
                mChargeGauge.gameObject.SetActive(true);

                mChargeGauge.GaugeUp(0.8f);

                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                mIsGaugeBreak = Vector2.Distance(mousePos, mChargeGauge.transform.position) > mChargeGauge.Scale.x * 0.5f;
            }
            else
            {
                mCurPressTime += Time.deltaTime;
            }
        }
        if ((Input.GetMouseButtonUp(0) && mCurPressTime >= PRESS_TIME) || mIsGaugeBreak)
        {
            mChargeGauge.gameObject.SetActive(false);

            Inventory.Instnace.OnCharge(mChargeGauge.Charge);

            mIsGaugeBreak = false;

            mCurPressTime = 0;
        }
    }

    private void CastBulletTime(float slowMax, float decrease)
    {
        Time.timeScale = Mathf.Max(slowMax, Time.timeScale - decrease * DeltaTime);
    }
}
