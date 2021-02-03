using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어에게 이동을 지시할 수 있어야 한다.
// 플레이어의 이동은 플레이어 내부적으로 처리한다.
// 플레이어의 이동 방식을 변경해야한다. MovePoint는 움직일 수 있는 지점을 제한하는 용도로 사용하자.
// 또한 위/아래로 이동할 때에는 y값만 MovePoint로 부터 불러오도록..
// 일단 공격 버튼부터 만들자. 이동은 좀 걸릴것 같으니까.

public class PlayerController : MonoBehaviour
{
    [SerializeField] private SubscribableButton _AttackButton;

    private Player _Player;

    private bool _IsAlreadyInit = false;
    private Coroutine _WaitCharge;

    private void Awake()
    {
        if (!_IsAlreadyInit)
        {
            _WaitCharge = new Coroutine(this);

            var find = GameObject.FindGameObjectWithTag("Player");
            if (find != null)
            {
                if (find.TryGetComponent(out _Player))
                {
                    _AttackButton.ButtonAction += state =>
                    {
                        switch (state)
                        {
                            case ButtonState.Down:

                                _WaitCharge.StartRoutine(WaitForStartCharging());
                                break;

                            case ButtonState.Up:

                                if (!_WaitCharge.IsFinished())
                                {
                                    _Player.AttackOrder();

                                    _WaitCharge.StopRoutine();
                                }
                                else
                                {
                                    Finger.Instance.EndCharging();
                                }
                                break;
                        }
                    };
                }
            }
            _IsAlreadyInit = true;
        }
    }

    private IEnumerator WaitForStartCharging()
    {
        for (float i = 0f; i < Finger.PRESS_TIME; i += Time.deltaTime)
        {
            yield return null;
        }
        _WaitCharge.Finish();

        Finger.Instance.StartCharging();
    }
}
