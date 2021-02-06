using System;
using System.Collections.Generic;
using UnityEngine;
// Include Backend
using BackEnd;
using static BackEnd.SendQueue;
//  Include GPGS namespace
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;


public class BackEndServerManager : Singleton<BackEndServerManager>
{
    private static BackEndServerManager instance;   // 인스턴스
    public bool isLogin { get; private set; }   // 로그인 여부

    private string tempNickName;                        // 설정할 닉네임 (id와 동일)
    public string myNickName { get; private set; } = string.Empty;  // 로그인한 계정의 닉네임
    public string myIndate { get;  set; } = string.Empty; 
    public string mIndate { get;  set; } = string.Empty;
    public string InDate { get;  set; } = string.Empty;// 로그인한 계정의 inDate
    private Action<bool, string> loginSuccessFunc = null;


    BackendReturnObject bro;

    private const string BackendError = "statusCode : {0}\nErrorCode : {1}\nMessage : {2}";
    // Start is called before the first frame update
    
    void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
        // 모든 씬에서 유지
        DontDestroyOnLoad(this.gameObject);
    }


    public void SendDataToServerSchema(string _Table )
    {
        Where where = new Where();
        Debug.Log("쿠르르");
       
        Param _Param = new Param();
        Debug.Log(_Param);

        where.Equal("gamerIndate", mIndate);
        _Param.Add("IAP", IAP.Instance.APP);
        _Param.Add("Gold", MoneyManager.Instance.Money);
        _Param.Add("Kill", GameLoger.Instance.KillCount);
        _Param.Add("Time", GameLoger.Instance.ElapsedTime.ToString());
       
        Enqueue(Backend.GameSchemaInfo.Get, _Table, where, 1, (BackendReturnObject Getbro) =>
        {
          
            Debug.Log("커넥트");

            if (Getbro.IsSuccess())
            {
             Enqueue(Backend.GameSchemaInfo.Update, _Table, Getbro.GetReturnValuetoJSON()["rows"][0]["inDate"]["S"].ToString(), _Param, (BackendReturnObject Updatebro) =>
             {
              if(Updatebro.IsSuccess())
                 Debug.Log("Update");
                 else
                     Debug.Log("falil update"+ Updatebro);
             });
            }
            else
            {
               Enqueue(Backend.GameSchemaInfo.Insert, _Table, _Param, (BackendReturnObject Insertbro) =>
                {
                  if(Insertbro.IsSuccess())
                    Debug.Log("insert");
                  else
                     Debug.Log("falil insert"+ Insertbro);
                });
            }
        });
    }

    public static BackEndServerManager GetInstance()
    {
        if (instance == null)
        {
            Debug.LogError("BackEndServerManager 인스턴스가 존재하지 않습니다.");
            return null;
        }

        return instance;
    }

    void Start()
    {

#if UNITY_ANDROID
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
            .Builder()
            .RequestServerAuthCode(false)
            .RequestIdToken()
            .Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;

        PlayGamesPlatform.Activate();
#endif
        isLogin = false;
        try
        {
            Backend.Initialize(() =>
            {
                if (Backend.IsInitialized)
                {
#if UNITY_ANDROID
                    Debug.Log("GoogleHash - " + Backend.Utils.GetGoogleHash());
#endif
                    // 비동기 함수 큐 초기화
                    StartSendQueue(true);
                }
                else
                {
                    Debug.Log("뒤끝 초기화 실패");
                }
            });
        }
        catch (Exception e)
        {
            Debug.Log("[예외]뒤끝 초기화 실패\n" + e.ToString());
        }
    }

    void OnApplicationQuit()
    {


        Param param = new Param();
        param.Add("Gold", MoneyManager.Instance.Money);

        SendDataToServerSchema("Player");
        Debug.Log("OnApplicationQuit");
        StopSendQueue();

    }

    void OnApplicationPause(bool isPause)
    {
        Debug.Log("OnApplicationPause : " + isPause);
        if (isPause == false)
        {
            ResumeSendQueue();
        }
        else
        {
            PauseSendQueue();
        }
    }

    
    public void BackendTokenLogin(Action<bool, string> func)
    {
        Debug.Log("전");
    Enqueue(Backend.BMember.LoginWithTheBackendToken ,callback =>
        {
            if (callback.IsSuccess())
            {
                OnStage();
                OnItem();
                OnLogined();
                Debug.Log("토큰 로그인 성공");
                loginSuccessFunc = func;
               
            }
            else
            {
                Debug.Log("토큰 로그인 실패\n" + callback.ToString());
                func(false, string.Empty);
                SceneLoader.Instance.SceneLoad(1);
            }
        });
    }

    public void OnStage()
    {
        bro = Backend.BMember.GetUserInfo();
        mIndate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
        if (bro.IsSuccess())
        {
            Where param = new Where();
            param.Equal("gamerIndate", mIndate);
            Backend.GameInfo.GetPrivateContents("STAGE", (callback1) =>
            {
                if (callback1.IsSuccess())
                {

                    if (callback1.IsSuccess())
                    {
                        GameLoger.Instance.SetStageUnlock(Convert.ToInt32(callback1.Rows()[0]["STAGE"]["L"]["N"].ToString()));




                        Debug.Log("정보 불러오기 성공" + callback1);


                    }
                    else
                    {

                        Debug.Log("정보 불러오기 실패" + callback1);

                    }
                }    
            });
        }
        else
            Debug.Log(bro + "812");
    }


    public void OnItem()
    {
        bro = Backend.BMember.GetUserInfo();
        mIndate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
        if (bro.IsSuccess())
        {
            Where param = new Where();
            param.Equal("gamerIndate", mIndate);
            Backend.GameInfo.GetPrivateContents("ITem", (callback1) =>
            {
                if (callback1.IsSuccess())
                {
                    List<int> nowlist = new List<int>();

                    for (int i = 0; i < callback1.Rows()[0]["ItemList"]["L"].Count; i++)
                    {
                        nowlist.Add(Convert.ToInt32(callback1.Rows()[0]["ItemList"]["L"][i]["N"].ToString()));
                    }


                   ItemStateSaver.Instance.SetUnlockedItem(nowlist); 

                    Debug.Log("정보 불러오기 성공" + callback1);
              

                }
                else
                { 

                    Debug.Log("정보 불러오기 실패"+callback1);
                    
                }
            });
        }
        else
            Debug.Log(bro + "812");
    }
    public void OnLogined()
    {
        bro = Backend.BMember.GetUserInfo();
        mIndate = bro.GetReturnValuetoJSON()["row"]["inDate"].ToString();
        if (bro.IsSuccess())
        {
            Where param = new Where();
            param.Equal("gamerIndate", mIndate);
            Backend.GameSchemaInfo.Get("Player", param, 1, callback1 =>
            {
                if (callback1.IsSuccess())
                {
                    Debug.Log(callback1.GetReturnValuetoJSON().ToJson());
                    Debug.Log(callback1.GetReturnValuetoJSON().ToJson());

                    GameLoger.Instance.RecordMoney(int.Parse(callback1.Rows()[0]["Gold"]["N"].ToString()));
                    IAP.Instance.AiP(bool.Parse(callback1.Rows()[0]["Gold"]["N"].ToString()));


                    Debug.Log("정보 불러오기 성공"+callback1);
                    SceneLoader.Instance.SceneLoad(2);

                }
                else
                {
                   
                    Debug.Log("정보 불러오기 실패");
                    SceneLoader.Instance.SceneLoad(1);
                }
            });
        }
        else
            Debug.Log(bro + "812");
    }
    // 커스텀 로그인


    public void GoogleAuthorizeFederation(Action<bool, string> func)
    {
#if UNITY_ANDROID
        // 이미 gpgs 로그인이 된 경우
        if (Social.localUser.authenticated == true)
        {
            var token = GetFederationToken();
            if (token.Equals(string.Empty))
            {
                Debug.LogError("GPGS 토큰이 존재하지 않습니다.");
                func(false, "GPGS 인증을 실패하였습니다.\nGPGS 토큰이 존재하지 않습니다.");
                return;
            }

            Enqueue(Backend.BMember.AuthorizeFederation, token, FederationType.Google, "gpgs 인증", callback =>
            {
                if (callback.IsSuccess())
                {
                    Debug.Log("GPGS 인증 성공");
                    loginSuccessFunc = func;

                    OnPrevBackendAuthorized();
                    return;
                }

                Debug.LogError("GPGS 인증 실패\n" + callback.ToString());
                func(false, string.Format(BackendError,
                    callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
            });
        }
        // gpgs 로그인을 해야하는 경우
        else
        {
            Social.localUser.Authenticate((bool success) =>
            {
                if (success)
                {
                    var token = GetFederationToken();
                    if (token.Equals(string.Empty))
                    {
                        Debug.LogError("GPGS 토큰이 존재하지 않습니다.");
                        func(false, "GPGS 인증을 실패하였습니다.\nGPGS 토큰이 존재하지 않습니다.");
                        return;
                    }

                    Enqueue(Backend.BMember.AuthorizeFederation, token, FederationType.Google, "gpgs 인증", callback =>
                    {
                        if (callback.IsSuccess())
                        {
                            Debug.Log("GPGS 인증 성공");
                            loginSuccessFunc = func;

                            OnPrevBackendAuthorized();
                            SceneLoader.Instance.SceneLoad(2);
                            return;
                        }

                        Debug.LogError("GPGS 인증 실패\n" + callback.ToString());
                        func(false, string.Format(BackendError,
                            callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
                    });
                }
                else
                {
                    Debug.LogError("GPGS 로그인 실패");
                    func(false, "GPGS 인증을 실패하였습니다.\nGPGS 로그인을 실패하였습니다.");
                    return;
                }
            });
        }
#endif
    }

    private string GetFederationToken()
    {
#if UNITY_ANDROID
        if (!PlayGamesPlatform.Instance.localUser.authenticated)
        {
            Debug.LogError("GPGS에 접속되어있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
            return string.Empty;
        }
        // 유저 토큰 받기
        string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
        tempNickName = PlayGamesPlatform.Instance.GetUserDisplayName();
        Debug.Log(tempNickName);
        return _IDtoken;

#else
        return string.Empty;
#endif
    }

    public void UpdateNickname(string nickname, Action<bool, string> func)
    {
        Enqueue(Backend.BMember.UpdateNickname, nickname, bro =>
        {
            // 닉네임이 없으면 매치서버 접속이 안됨
            if (!bro.IsSuccess())
            {
                Debug.LogError("닉네임 생성 실패\n" + bro.ToString());
                func(false, string.Format(BackendError,
                    bro.GetStatusCode(), bro.GetErrorCode(), bro.GetMessage()));
                return;
            }
            loginSuccessFunc = func;
            OnBackendAuthorized();
            SceneLoader.Instance.SceneLoad(2);
        });
    }
    private void OnPrevBackendAuthorized()
    {
        isLogin = true;

        OnBackendAuthorized();
    }
    private void OnBackendAuthorized()
    {
        Enqueue(Backend.BMember.GetUserInfo, callback =>
        {
            if (!callback.IsSuccess())
            {
                Debug.LogError("유저 정보 불러오기 실패\n" + callback);
                loginSuccessFunc(false, string.Format(BackendError,
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
                return;
            }
            Debug.Log("유저정보\n" + callback);

            var info = callback.GetReturnValuetoJSON()["row"];
            if (info["nickname"] == null)
            {
               
                return;
            }
            myNickName = info["nickname"].ToString();
            myIndate = info["inDate"].ToString();

           
        });
    }
    void Update()
    {
        SendQueue.Poll();
        
    }

    public void GuestLogin(Action<bool, string> func)
    {
        Enqueue(Backend.BMember.GuestLogin, callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("게스트 로그인 성공");
                loginSuccessFunc = func;
             
                bro = Backend.GameSchemaInfo.Insert("Player");
               Param param1 = new Param();
                param1.Add("gamerIndate", mIndate);
                Backend.GameSchemaInfo.Update("Player", bro.GetInDate(), param1);
                OnPrevBackendAuthorized();
                return;
            }

            Debug.Log("게스트 로그인 실패\n" + callback);
            func(false, string.Format(BackendError,
                callback.GetStatusCode(), callback.GetErrorCode(), callback.GetMessage()));
        });
    }
  
    // Update is called once per frame
 
}
