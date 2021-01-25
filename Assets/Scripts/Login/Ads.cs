﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;


public class Ads : Singleton<Ads>
{
    private static Ads _instance = null;
    public bool isTestMode;

    public static Ads instance
    {
        get
        {
            if (_instance == null)
                return null;
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    void Start()
    {

        LoadFrontAd();
        LoadRewardAd();
    }

    AdRequest GetAdRequest()
    {
        Debug.Log("AdRequest");
        return new AdRequest.Builder().Build();
    }



    #region 전면 광고
    const string frontTestID = "ca-app-pub-3940256099942544/8691691433";
    const string frontID = " ca-app-pub-5708876822263347/3093570612";
    InterstitialAd frontAd;


    void LoadFrontAd()
    {
        frontAd = new InterstitialAd(isTestMode ? frontTestID : frontID);
        Debug.Log(frontAd);
        frontAd.LoadAd(GetAdRequest());
        frontAd.OnAdClosed += (sender, e) =>
        {
            Debug.Log("Ad End");
        };
    }

    public void ShowFrontAd()
    {
        Debug.Log("FrontAd");
        LoadFrontAd();
        frontAd.Show();
    }
    #endregion

    #region 리워드 광고
    const string rewardTestID = "ca-app-pub-3940256099942544/5224354917";
    const string rewardID = "ca-app-pub-5708876822263347/6491913665";
    RewardedAd rewardAd;


    void LoadRewardAd()
    {
        rewardAd = new RewardedAd(isTestMode ? rewardTestID : rewardID);
        rewardAd.LoadAd(GetAdRequest());
        rewardAd.OnUserEarnedReward += (sender, e) =>
        {
        };
    }

    public void ShowRewardAd()
    {
        LoadRewardAd();
        rewardAd.Show();
    }
    #endregion
    public void ClosedADEvent(Action action)
    {
        if (frontAd != null)
        {
            frontAd.OnAdClosed += (a, b) => action.Invoke();
        }
    }
    public void UserEarnedRewardEvent(Action action)
    {
        if (rewardAd != null)
        {
            rewardAd.OnUserEarnedReward += (a, b) => action.Invoke();
        }
    }
}
