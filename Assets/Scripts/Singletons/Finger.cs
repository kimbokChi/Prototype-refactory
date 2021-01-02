﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SwipeDirection
{
    up, down, right, left
}

public class Finger : Singleton<Finger>
{
    private const float PRESS_TIME = 0.8f;

    private float DeltaTime => Time.deltaTime;

    [SerializeField]
    private ChargeGauge mChargeGauge;
    public  ChargeGauge Gauge
    { get => mChargeGauge; }

    public  Item  CarryItem
    {
        get => mCarryItem;
        set
        {
            if (value == null)
            {
                CarryItemImage.sprite = _EmptySprite;
            }
            else
            {
                CarryItemImage.sprite = value.Sprite;
            }
            mCarryItem = value; 
        }
    }
    private Item mCarryItem;

    private float mCurPressTime;

    private IEnumerator mEOnBulletTime;

    private Sprite _EmptySprite;
    [SerializeField] private Image CarryItemImage;
    [SerializeField] private float SwipeLength;

    private Vector2 mTouchBeganPos;
    private Vector2 mTouchEndedPos;
    private Vector2 mSwipeDirection;

    private void Awake() {
        mCurPressTime = 0f;
        mTouchBeganPos = mTouchEndedPos = mSwipeDirection = Vector2.zero;

        _EmptySprite = CarryItemImage.sprite;
    }

    private void Update()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                {
                    CarryItemImage.transform.position = Input.mousePosition;
                }
                break;

            case RuntimePlatform.Android:
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);

                    CarryItemImage.transform.position = touch.position;
                }
                break;
        }
        transform.SetZ(0);

        bool windowEnable = Inventory.Instance.InventoryWindow.activeSelf;

        if (CarryItemImage.IsActive() != windowEnable)
        {
            CarryItemImage.gameObject.SetActive(windowEnable);
        }

        if (!EventSystem.current.IsPointerInUIObject())
        {
            // Begin Touch
            if (Input.GetMouseButtonDown(0))
            {
                mChargeGauge.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                mChargeGauge.transform.Translate(0, 0, 10);
            }
            // Touuuuuuuuuuuuuch
            else if (Input.GetMouseButton(0))
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
            else InputReleaseCheck();
        }
        else InputReleaseCheck();
    }

    private void InputReleaseCheck()
    {
        if (Input.GetMouseButtonUp(0) && mCurPressTime >= PRESS_TIME)
        {
            Inventory.Instance.OnCharge(mChargeGauge.Charge);

            mChargeGauge.gameObject.SetActive(false);
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

    public bool Swipe(SwipeDirection inputDriection)
    {
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                mTouchBeganPos = t.position;
            }
            if (t.phase == TouchPhase.Moved)
            {
                mTouchEndedPos = t.position;

                if (Vector2.Distance(mTouchBeganPos, mTouchEndedPos) >= SwipeLength)
                {
                    mSwipeDirection = mTouchEndedPos - mTouchBeganPos;

                    mSwipeDirection.Normalize();

                    if (Mathf.Abs(mSwipeDirection.x) > Mathf.Abs(mSwipeDirection.y))
                    {
                        return (mSwipeDirection.x > 0 && SwipeDirection.right == inputDriection) ||
                               (mSwipeDirection.x < 0 && SwipeDirection.left  == inputDriection);
                    }
                    else
                    {
                        return (mSwipeDirection.y > 0 && SwipeDirection.up   == inputDriection) ||
                               (mSwipeDirection.y < 0 && SwipeDirection.down == inputDriection);
                    }
                    
                }
            }
        }
        return false;
    }
}
