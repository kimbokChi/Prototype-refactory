using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ResurrectionWindow : MonoBehaviour
{
    [FormerlySerializedAs("_NextWindow")]
    [SerializeField] private GameObject _ResultWindow;
    [SerializeField] private GameObject _ResurrectionWindow;
    [SerializeField] private Resurrectable _Resurrectable;

    private bool _IsAlreadyEarn = false;

    [ContextMenu("FindPlayer")]
    private void FindPlayer()
    {
        if (_Resurrectable == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");

            Debug.Assert(player.TryGetComponent(out _Resurrectable));
        }
    }
    private void Awake()
    {
        FindPlayer();

        if (_Resurrectable.TryGetComponent(out Player player))
        {
            player.DeathEvent += revertAction =>
            {
                if (!revertAction)
                {
                    if (!_IsAlreadyEarn)
                    {
                        _ResurrectionWindow.SetActive(true);
                    }
                    else
                    {
                        _ResultWindow.SetActive(true);
                    }
                }
            };
        }
    }
    public void ShowAD()
    {
        if (!_IsAlreadyEarn)
        {
            Ads.Instance.ShowRewardAd();
            Ads.Instance.UserEarnedRewardEvent(() =>
            {
                _IsAlreadyEarn = true;
                _ResurrectionWindow.SetActive(false);

                _Resurrectable.Resurrect();
            });
        }
    }
    public void Close()
    {
        _ResurrectionWindow.SetActive(false);

        _ResultWindow.SetActive(true);
    }
}
