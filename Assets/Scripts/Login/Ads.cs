using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;


public class Ads : Singleton<Ads>
{
    private readonly string unitID = "ca - app - pub - 5708876822263347~3868785607";

    private readonly string Test_unitID = " ca-app-pub-5708876822263347/3093570612";

    private readonly string Test_reward = " ca-app-pub-5708876822263347/6491913665";
    private InterstitialAd screednAD;
    private RewardedAd reAd;

    public void mAds()
    {
        InitAD();
        Invoke("Show", 2);
    }
    public void ReAds()
    {
        InitReAD();

        Invoke("Show", 1);

        

    }
    public void InitReAD()
    {
        string id = Debug.isDebugBuild ? Test_unitID : unitID;
        reAd = new RewardedAd(id);
        AdRequest request = new AdRequest.Builder().Build();

        reAd.LoadAd(request);
        reAd.OnAdClosed += (sender, e) => Debug.Log("광고가 닫힘");
        reAd.OnAdLoaded += (sender, e) => Debug.Log("광고가 로드됨");
    }
    public void InitAD()
    {
        string id = Debug.isDebugBuild ? Test_unitID : unitID;
        screednAD = new InterstitialAd(id);
        AdRequest request = new AdRequest.Builder().Build();


        screednAD.LoadAd(request);
        screednAD.OnAdClosed += (sender, e) => Debug.Log("광고가 닫힘");
        screednAD.OnAdLoaded += (sender, e) => Debug.Log("광고가 로드됨");
        
    }

    public void Show()
    {
        StartCoroutine("ShowScreenAd");
    }

    private IEnumerator ShowScreenAd()
    {
        while(!screednAD.IsLoaded())
        {
            yield return null;
        }
        screednAD.Show();
    }

    public void ClosedADEvent(Action action)
    {
        if (screednAD != null)
        {
            screednAD.OnAdClosed += (a, b) => action.Invoke();
        }
    }
    public void UserEarnedRewardEvent(Action action)
    {
        if (reAd != null)
        {
            reAd.OnUserEarnedReward += (a, b) => action.Invoke();
        }
    }
}
