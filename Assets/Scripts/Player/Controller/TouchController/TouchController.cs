using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchController : MonoBehaviour
{
    private const float NeedMovingLength = 30;

    private Coroutine _MoveRoutine;
    private TouchPhase _CurrentPhase;
    private Vector2 _BeganInputPoint;

    private Player _Player;

    public bool IsMobilePlatform()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                return true;
            default:
                return false;
        }
    }
    public bool HasInput(TouchPhase phase)
    {
        if (IsMobilePlatform())
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == phase)
                {
                    return EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                }
            }
            return false;
        }
        else
        {
            switch (phase)
            {
                case TouchPhase.Began:
                    {
                        return Input.GetMouseButtonDown(0);
                    }
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    {
                        return Input.GetMouseButton(0);
                    }
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    {
                        return Input.GetMouseButtonUp(0);
                    }
            }
        }
        return false;
    }
    public Vector2 InputPosition()
    {
        if (IsMobilePlatform())
        {
            return Input.GetTouch(0).position;
        }
        else
        {
            return Input.mousePosition;
        }
    }

    private void Start()
    {
        _MoveRoutine = new Coroutine(this);
        _CurrentPhase = TouchPhase.Ended;

        _Player = FindObjectOfType<Player>();
    }
    private void Update()
    {
        CurrentPhaseCheck();

        switch (_CurrentPhase)
        {
            case TouchPhase.Began:
                {
                    _BeganInputPoint = InputPosition();
                }
                break;
            case TouchPhase.Moved:
                {
                    Vector2 inputPosition = InputPosition();

                    if (Vector2.Distance(inputPosition, _BeganInputPoint) >= NeedMovingLength)
                    {
                        Vector2 direction = (inputPosition - _BeganInputPoint).normalized;
                        _BeganInputPoint = inputPosition;

                        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
                        {
                            if (direction.x > 0)
                            {
                                _MoveRoutine.StartRoutine(Move(Direction.Right));
                            }
                            else
                            {
                                _MoveRoutine.StartRoutine(Move(Direction.Left));
                            }
                        }
                        else
                        {
                            if (direction.y > 0)
                            {
                                _Player.MoveOrder(Direction.Up);
                            }
                            else
                            {
                                _Player.MoveOrder(Direction.Down);
                            }
                        }
                    }
                }
                break;
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Ended:
                break;
            case TouchPhase.Canceled:
                break;
            default:
                break;
        }
    }
    private void CurrentPhaseCheck()
    {
        for (TouchPhase phase = TouchPhase.Began; (int)phase < 5; phase++)
        {
            if (HasInput(phase))
            {
                _CurrentPhase = phase;
                break;
            }
        }
    }
    private IEnumerator Move(Direction direction)
    {
        _Player.MoveOrder(direction);

        while (_CurrentPhase == TouchPhase.Moved) { yield return null; }

        _Player.MoveStop();

        _MoveRoutine.Finish();
    }
}
