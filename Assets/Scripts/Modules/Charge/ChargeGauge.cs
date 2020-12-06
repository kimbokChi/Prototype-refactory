using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeGauge : MonoBehaviour
{
    public readonly Vector3 MAX_SCALE = new Vector3(1.8f, 1.8f, 1.8f);

    public event System.Action  OnChargeEvent;
    public event System.Action DisChargeEvent;

    private Player mPlayer;

    public float Charge
    {
        get;
        private set;
    }

    public void GaugeUp(float accel = 1.0f)
    {
        Charge = Mathf.Min(Charge + (Time.deltaTime * accel), 1);

        transform.localScale = Vector3.Lerp(Vector3.zero, MAX_SCALE, Charge);
    }

    private void OnEnable()
    {
        OnChargeEvent?.Invoke();

        if (mPlayer == null) {
            GameObject.FindGameObjectWithTag("Player").TryGetComponent(out mPlayer);
        }
        mPlayer.InputLock(true);
        MainCamera.Instance.ZoomIn(4.5f, 0.9f, true);
    }

    private void OnDisable()
    {
        DisChargeEvent?.Invoke();

        mPlayer.InputLock(false);
        MainCamera.Instance.ZoomOut(2.5f, true);

        transform.localScale = Vector3.zero;

        Charge = 0.0f;
    }
}
