using System.Collections;
using UnityEngine;

public class MainCamera : Singleton<MainCamera>
{
    [SerializeField] private Camera ThisCamera;
    [SerializeField] private float OriginCameraScale;

    private IEnumerator mCameraShake;
    private IEnumerator mCameraMove;
    private IEnumerator mCameraZoom;

    private Vector3 mOriginPosition;

    private void Reset()
    {
        if (TryGetComponent(out ThisCamera))
        {
            OriginCameraScale = ThisCamera.orthographicSize;
        }
    }

    private void Start()
    {
        mOriginPosition = transform.position;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 point = ThisCamera.ScreenToWorldPoint(Input.mousePosition);

            ZoomIn(point, 5f, 0.45f, 1f, true);
        }
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

    public void ZoomIn(Vector2 point, float time, float percent, float speed, bool usingTimeScale)
    {
        if (mCameraZoom != null)
        {
            StopCoroutine(mCameraZoom);
        }
        float targetScale = OriginCameraScale * percent;

        StartCoroutine(mCameraZoom = CameraZoomIn(point, time, targetScale, usingTimeScale));

        Move(point, speed);
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
            ThisCamera.orthographicSize = Mathf.Lerp(ThisCamera.orthographicSize, targetScale, i / time);

            yield return null;
        }
        mCameraZoom = null;
    }
}
