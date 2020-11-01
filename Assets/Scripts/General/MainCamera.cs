using System.Collections;
using UnityEngine;

public class MainCamera : Singleton<MainCamera>
{
    private IEnumerator mCameraShake;
    private IEnumerator mCameraMove;

    private Vector3 mOriginPosition;

    private void Start()
    {
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
}
