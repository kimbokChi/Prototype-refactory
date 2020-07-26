using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGauge : MonoBehaviour
{
    public readonly Vector3 MAX_SCALE = new Vector3(1.8f, 1.8f, 1.8f);

    private float mLerpAmount = 0.0f;

    public float GaugeUp(float accel = 1.0f)
    {
        mLerpAmount = Mathf.Min(mLerpAmount + (Time.deltaTime * accel), 1);

        transform.localScale = Vector3.Lerp(Vector3.zero, MAX_SCALE, mLerpAmount);

        return mLerpAmount;
    }

    private void OnEnable() => mLerpAmount = 0.0f;
}
