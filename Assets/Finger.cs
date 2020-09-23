using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Finger : Singleton<Finger>
{
    private const float PRESS_TIME = 0.8f;

    private float DeltaTime => Time.deltaTime;

    [SerializeField]
    private ChargeGauge mChargeGauge;

    public  Item  CarryItem
    {
        get => mCarryItem;
        set => mCarryItem = value;
    }
    private Item mCarryItem;

    private float mCurPressTime;

    private bool HaveFinger
    {
        get
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            return Vector2.Distance(mousePos, mChargeGauge.transform.position) > mChargeGauge.Scale.x * 0.5f;
        }
    }

    private IEnumerator mEOnBulletTime;

    private void Awake() {
        mCurPressTime = 0f;
    }

    private void Update()
    {
        // Begin Touch
        if (Input.GetMouseButtonDown(0))
        {
            mChargeGauge.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            mChargeGauge.transform.Translate(0, 0, 10);
        }

        // Touuuuuuuuuuuuuch
        if (Input.GetMouseButton(0))
        {
            if (mCurPressTime >= PRESS_TIME)
            {
                mChargeGauge.gameObject.SetActive(true);

                mChargeGauge.GaugeUp(0.8f);

                if (mEOnBulletTime == null)
                {
                    StartCoroutine(mEOnBulletTime = EOnBulletTime(1.5f, 0.45f));
                }
            }
            else
            {
                mCurPressTime += Time.deltaTime;
            }
        }
        // End Touch
        if ((Input.GetMouseButtonUp(0) && mCurPressTime >= PRESS_TIME) || HaveFinger)
        {
            mChargeGauge.gameObject.SetActive(false);

            Inventory.Instance.OnCharge(mChargeGauge.Charge);

            mCurPressTime = 0;

            StartCoroutine(EDisBulletTime(1.75f));
        }
    }

    private IEnumerator EOnBulletTime(float accel, float slowMax)
    {
        float lerpAmount = 0f;

        while (!Input.GetMouseButtonUp(0))
        {
            lerpAmount = Mathf.Min(slowMax, lerpAmount + accel * DeltaTime);

            Time.timeScale = Mathf.Lerp(Time.timeScale, slowMax, lerpAmount);

            yield return null;
        }
        mEOnBulletTime = null;

        yield break;
    }

    private IEnumerator EDisBulletTime(float accel, float origin = 1f)
    {
        float lerpAmount = 0f;

        while (Time.timeScale != origin)
        {
            lerpAmount = Mathf.Min(origin, lerpAmount + accel * DeltaTime);

            Time.timeScale = Mathf.Lerp(Time.timeScale, origin, lerpAmount);

            yield return null;
        }
        yield break;
    }
}
