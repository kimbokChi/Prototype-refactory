using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class BossDirector : MonoBehaviour
{
    [SerializeField] private PlayableDirector _Director;
    [SerializeField] private Floor _BossFloor;

    [Header("Audio Property")]
    [SerializeField] private AudioClip _BossThema;
    [SerializeField] private AudioSource _AudioSource;

    private void Awake()
    {
        Inventory.Instance.MoveUpDownEvent += (pos, dir) =>
        {
            // 다음 층으로 이동한다면
            if (dir == Direction.Up && pos == UnitizedPosV.BOT)
            {
                // 보스 이전 층에 입장
                if (Castle.Instance.PlayerFloor.FloorIndex == _BossFloor.FloorIndex - 1)
                {
                    Castle.Instance.CanPlayerFloorNotify = false;
                }
                // 보스 층에 입장
                else if (Castle.Instance.PlayerFloor.FloorIndex == _BossFloor.FloorIndex)
                {
                    _Director.Play();
                    Castle.Instance.ForceStopUpdate();
                }
            }
        };
    }
    public void SE_Start()
    {
        _AudioSource.clip = _BossThema;
    }
    public void SE_CameraSetPosition()
    {
        MainCamera.Instance.Move(_BossFloor.transform.localPosition, 100f);
    }
    public void SE_Finish()
    {
        Castle.Instance.CanPlayerFloorNotify = true;
        Castle.Instance.PlayerFloorNotify();

        Castle.Instance.ReStartUpdate();
    }
}
