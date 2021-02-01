using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResurrectionWindow : MonoBehaviour
{
    [SerializeField] private GameObject _NextWindow;
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
    public void ShowAD()
    {
        if (!_IsAlreadyEarn)
        {
            Ads.Instance.ShowRewardAd();
            Ads.Instance.UserEarnedRewardEvent(() =>
            {
                _IsAlreadyEarn = true;
                gameObject.SetActive(false);

                _Resurrectable.Resurrect();
            });
        }
    }
    public void Close()
    {
        gameObject.SetActive(false);
        _NextWindow.SetActive(true);
    }
}
