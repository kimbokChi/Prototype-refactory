using System.Collections;
using UnityEngine;

public static class MathExtension
{
    public static void Range(this ref float value, float min, float max)
    {
        if (value < min) value = min;
        if (value > max) value = max;
    }
}

public class MainCamera : Singleton<MainCamera>
{
    [SerializeField] private Camera ThisCamera;
    [SerializeField] private float OriginCameraScale;

    private IEnumerator mCameraShake;
    private IEnumerator mCameraMove;
    private IEnumerator mCameraZoom;

    private Vector3 mOriginPosition;

    private bool mIsZoomIn;

    private void Reset()
    {
        if (TryGetComponent(out ThisCamera))
        {
            OriginCameraScale = ThisCamera.orthographicSize;
        }
    }

    private void Start()
    {
        mIsZoomIn = false;
        mOriginPosition = transform.position;
    }

    public void Shake()
    {
        Shake(0.25f, 0.8f, true);
    }

    public void Shake(float time, float power, bool usingTimeScale)
    {
        if (mCameraShake != null)
        {
            transform.position = mOriginPosition;
            StopCoroutine(mCameraShake);
        }
        StartCoroutine(mCameraShake = CameraShake(time, power, usingTimeScale));
    }

    public void Move(Vector2 point, float speed = 1f)
    {
        if (mCameraMove != null)
        {
            StopCoroutine(mCameraMove);
        }
        StartCoroutine(mCameraMove = CameraMove(point, speed));
    }

    public void ZoomIn(Vector2 point, float time, float percent, bool usingTimeScale)
    {
        if (mCameraZoom != null)
        {
            StopCoroutine(mCameraZoom);
        }
        float targetScale = OriginCameraScale * percent;

        //
        float range = 8 - targetScale;

        point.x.Range(
            mOriginPosition.x + range * -0.5625f, mOriginPosition.x + range * 0.5625f);

        point.y.Range(
            mOriginPosition.y - range, mOriginPosition.y + range);
        //
        mIsZoomIn = true;
        StartCoroutine(mCameraZoom = CameraZoomIn(point, time, targetScale, usingTimeScale));
    }

    public void ZoomOut(float time, bool usingTimeScale)
    {
        if (mCameraZoom != null)
        {
            StopCoroutine(mCameraZoom);
        }
        mIsZoomIn = false;
        StartCoroutine(mCameraZoom = CameraZoomOut(time, usingTimeScale));
    }

    private IEnumerator CameraShake(float time, float power, bool usingTimeScale)
    {
        float deltaTime = 0f;

        power *= 0.1f;

        for (float i = 0; i < time; i += deltaTime)
        {
            transform.position = mOriginPosition + (Vector3)(Random.insideUnitCircle * power);

            deltaTime = Time.deltaTime;

            if (usingTimeScale)
            {
                deltaTime *= Time.timeScale;
            }
            yield return null;
        }
        transform.position = mOriginPosition;
    }

    private IEnumerator CameraMove(Vector2 point, float speed)
    {
        float lerpAmount = 0f;

        while (lerpAmount < 1f)
        {
            lerpAmount = Mathf.Min(1f, lerpAmount + Time.deltaTime * Time.timeScale * speed);

            transform.position = Vector2.Lerp(transform.position, point, lerpAmount);

            transform.Translate(0, 0, -10f);

            yield return null;
        }
        mOriginPosition = transform.position;
    }

    private IEnumerator CameraZoomIn(Vector2 point, float time, float targetScale, bool usingTimeScale)
    {
        float deltaTime = 0f;

        for (float i = 0; i < time; i += deltaTime)
        {
            deltaTime = Time.deltaTime;

            if (usingTimeScale)
            {
                deltaTime *= Time.timeScale;
            }
            float lerp = i / time;

            if (Mathf.Abs(targetScale - ThisCamera.orthographicSize) < 0.02f)
            {
                transform.position = point;
                transform.Translate(0, 0, -10f);

                ThisCamera.orthographicSize = targetScale;
                yield break;
            }
            transform.position = Vector2.Lerp(transform.position, point, lerp);
            transform.Translate(0, 0, -10f);

            ThisCamera.orthographicSize = Mathf.Lerp(ThisCamera.orthographicSize, targetScale, lerp);

            yield return null;
        }
        mCameraZoom = null;
    }

    private IEnumerator CameraZoomOut(float time, bool usingTimeScale)
    {
        float deltaTime = 0f;

        for (float i = 0; i < time; i += deltaTime)
        {
            deltaTime = Time.deltaTime;

            if (usingTimeScale)
            {
                deltaTime *= Time.timeScale;
            }
            float lerp = i / time;

            if (Mathf.Abs(OriginCameraScale - ThisCamera.orthographicSize) < 0.02f)
            {
                transform.position = mOriginPosition;
                transform.Translate(0, 0, -10f);

                ThisCamera.orthographicSize = OriginCameraScale;
                yield break;
            }
            transform.position = Vector2.Lerp(transform.position, mOriginPosition, lerp);
            transform.Translate(0, 0, -10f);

            ThisCamera.orthographicSize = Mathf.Lerp(ThisCamera.orthographicSize, OriginCameraScale, lerp);

            yield return null;
        }
        mCameraZoom = null;
    }
}
